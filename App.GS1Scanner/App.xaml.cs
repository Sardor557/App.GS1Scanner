using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;

namespace App.GS1Scanner
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
    Debug.WriteLine(e.ExceptionObject);

            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
            {
                Debug.WriteLine(e.Exception);
                e.Handled = true;    // чтобы приложение не закрывалось
            };

            MainPage = serviceProvider.GetRequiredService<MainPage>();
        }
    }
}
