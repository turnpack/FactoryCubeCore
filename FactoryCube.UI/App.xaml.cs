// App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using FactoryCube.Interfaces;
using FactoryCube.Vision.Camera;
using FactoryCube.UI.Views;
using FactoryCube.Vision.Vision;
using FactoryCube.Comm;
using FactoryCube.Services;
using System.Windows.Media.Media3D;
using System.ComponentModel.Design;

namespace FactoryCube.UI
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }
        public static IServiceProvider ServiceProvider => AppHost.Services;

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Core services
                    services.AddSingleton<ICameraService, HalconCameraService>();
                    services.AddSingleton<IVisionProcessorService, DummyVisionProcessor>(); // Replace with real processor
                    services.AddSingleton<Func<string, ICameraService>>(sp => deviceId => new HalconCameraService(deviceId));
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
