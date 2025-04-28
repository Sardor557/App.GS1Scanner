using Android.Widget;
using AndroidX.Camera.Core;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
using Microsoft.Maui.Handlers;
using Android.Gms.Tasks;
using Xamarin.Google.MLKit.Vision.BarCode;
using Xamarin.Google.MLKit.Vision.Barcode.Common;
using System;
using Android.Runtime;
using Xamarin.Google.MLKit.Vision.Common;
using App.Utils;
using Android.Util;

namespace App.GS1Scanner.Platforms.Android
{
    public delegate void BarcodeDetectedEventHandler(object sender, string code);

    public sealed class MlKitScannerHandler : ViewHandler<MlKitScanner, FrameLayout>
    {
        public MlKitScannerHandler() : base(ViewHandler.ViewMapper, ViewHandler.ViewCommandMapper) { }

        protected override FrameLayout CreatePlatformView()
        {
            var host = new FrameLayout(Context);
            InitAsync(host);
            return host;
        }

        private void InitAsync(FrameLayout host)
        {
            var previewView = new PreviewView(Context);
            host.AddView(previewView,
                new FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MatchParent,
                    FrameLayout.LayoutParams.MatchParent));

            var future = ProcessCameraProvider.GetInstance(Context);
            var cameraProvider = (ProcessCameraProvider)future.Get();

            var selector = new CameraSelector.Builder()
                           .RequireLensFacing(CameraSelector.LensFacingBack)
                           .Build();

            var analysis = new ImageAnalysis.Builder()
                           .SetBackpressureStrategy(
                               ImageAnalysis.StrategyKeepOnlyLatest)
                           .Build();

            var options = new BarcodeScannerOptions.Builder()
                          .SetBarcodeFormats(Barcode.FormatDataMatrix)
                          .Build();

            var scanner = BarcodeScanning.GetClient(options);

            analysis.SetAnalyzer(ContextCompat.GetMainExecutor(Context),
                new Analyzer(proxy =>
                {
                    try
                    {
                        var img = InputImage.FromMediaImage(
                                  proxy.Image,
                                  proxy.ImageInfo?.RotationDegrees ?? 0);

                        scanner.Process(img)
                           .AddOnSuccessListener(
                               new OnSuccessListener(obj =>
                               {
                                   var list = obj.JavaCast<JavaList>();
                                   for (int i = 0; i < list.Size(); i++)
                                   {
                                       var bc = list.Get(i).JavaCast<Barcode>();
                                       var raw = bc.RawValue;
                                       if (!string.IsNullOrEmpty(raw))
                                           VirtualView.RaiseCode(raw);
                                   }
                               }))
                           .AddOnCompleteListener(
                               new OnCompleteListener(_ => proxy.Close()));
                    }
                    catch { proxy.Close(); }
                }));

            try
            {
                var preview = new Preview.Builder().Build();
                preview.SetSurfaceProvider(
                    ContextCompat.GetMainExecutor(Context),
                    previewView.SurfaceProvider);

                cameraProvider.UnbindAll();

                var lifecycleOwner = (ILifecycleOwner)Context;
                cameraProvider.BindToLifecycle(lifecycleOwner, selector, preview, analysis);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"error: {ex.GetAllMessages()} stack: {ex.GetStackTrace(5)}");
            }
        }

        private sealed class Analyzer : Java.Lang.Object, ImageAnalysis.IAnalyzer
        {
            readonly Action<IImageProxy> _callback;
            public Analyzer(Action<IImageProxy> cb) => _callback = cb;

            /* 1. Метод analyze  */
            void ImageAnalysis.IAnalyzer.Analyze(IImageProxy image)
                => _callback(image);

            /* 2.  ЯВНАЯ реализация свойства интерфейса */
            Size ImageAnalysis.IAnalyzer.DefaultTargetResolution => null;  // null = дай любое подходящее разрешение
        }

        private sealed class OnSuccessListener : Java.Lang.Object, IOnSuccessListener
        {
            readonly Action<Java.Lang.Object> _cb;
            public OnSuccessListener(Action<Java.Lang.Object> cb) => _cb = cb;
            public void OnSuccess(Java.Lang.Object result) => _cb(result);
            public Size DefaultTargetResolution => null;
        }

        private sealed class OnCompleteListener :
            Java.Lang.Object, IOnCompleteListener
        {
            readonly Action<Task> _cb;
            public OnCompleteListener(Action<Task> cb) => _cb = cb;
            public void OnComplete(Task task) => _cb(task);
        }
    }
}
