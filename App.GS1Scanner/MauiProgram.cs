using App.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Settings.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.GS1Scanner.Utils;
using App.GS1Scanner.Platforms.Android;

namespace App.GS1Scanner
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var environment = "Production";
#if DEBUG
            environment = "Development";
#endif

            var configuration = ConfigurationExtension.LoadConfiguration();
            builder.Configuration.AddConfiguration(configuration);
            builder.Services.AddSingleton(configuration);

            var configurationAssemblies = new[]
            {
                typeof(ConsoleLoggerConfigurationExtensions).Assembly,
                typeof(FileLoggerConfigurationExtensions).Assembly,
                typeof(LoggerConfigurationTelegramExtensions).Assembly
            };

            var options = new ConfigurationReaderOptions(configurationAssemblies);

            Log.Logger = new LoggerConfiguration()
                   .Enrich.FromLogContext()
                   .Enrich.WithMachineName()
                   .Enrich.WithExceptionDetails()
                   .Enrich.WithProperty("Environment", environment)
                   .ReadFrom.Configuration(builder.Configuration, options)
                   .CreateLogger();

            builder.Services.AddMyServices();
            builder.Services.AddMemoryCache();

            builder.Services.AddTransient<MainPage>();

            builder
                .UseMauiApp<App>(provider => new App(provider))
                .ConfigureMauiHandlers(handlers =>
                {
                    # if ANDROID
                    handlers.AddHandler<MlKitScanner, MlKitScannerHandler>();
                    #endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            return builder.Build();
        }
    }
}

