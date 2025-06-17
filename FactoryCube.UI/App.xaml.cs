// App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using FactoryCube.Interfaces;
using FactoryCube.Vision.Camera;
using FactoryCube.UI.Views;
using FactoryCube.Comm;
using Microsoft.Extensions.Logging;
using FactoryCube.Vision.Processors;
using NLog;


namespace FactoryCube.UI
{
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static IHost AppHost { get; private set; }
        public static IServiceProvider ServiceProvider => AppHost.Services;

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Core services
                    services.AddSingleton<ICameraService, HalconCameraService>();
                    //services.AddSingleton<IVisionProcessorService, DummyVisionProcessor>();
                    services.AddSingleton<IVisionProcessor, HalconVisionProcessor>();
                    services.AddSingleton<Func<string, ICameraService>>(sp =>
                    {
                        return deviceId =>
                        {
                            var logger = sp.GetRequiredService<ILogger<HalconCameraService>>();
                            return new HalconCameraService(deviceId, logger);
                        };
                    });
                    services.AddSingleton<ICommService, OmronCommService>();



                    // ViewModels
                    services.AddSingleton<CameraPreviewViewModel>();


                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();

            var mainWindow = new MainWindow();
            mainWindow.DataContext = AppHost.Services.GetRequiredService<CameraPreviewViewModel>();
            Logger.Info("FactoryCube application started.");
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (ServiceProvider.GetService(typeof(CameraPreviewViewModel)) is CameraPreviewViewModel vm)
            {
                vm?.StopPreviewCommand.Execute(null); // Stop the camera safely
            }

            base.OnExit(e);
        }


    }
}
