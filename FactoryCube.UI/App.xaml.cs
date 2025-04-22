// App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using FactoryCube.Interfaces;
using FactoryCube.Halcon.Camera;
using FactoryCube.UI.ViewModels;
using FactoryCube.Halcon.Vision;
using System.Windows.Media.Media3D;

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
                    services.AddSingleton<ICamera, HalconCamera>();
                    services.AddSingleton<IVisionProcessor, DummyVisionProcessor>(); // Replace with real processor
                    services.AddSingleton<Func<string, ICamera>>(sp => deviceId => new HalconCamera(deviceId));


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
