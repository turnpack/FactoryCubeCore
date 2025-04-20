// App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using FactoryCube.Interfaces;
using FactoryCube.Halcon.Camera;
using FactoryCube.UI.ViewModels;
using FactoryCube.Halcon.Vision;

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

        protected override async void OnExit(ExitEventArgs e)
        {
            using (AppHost)
            {
                await AppHost.StopAsync(TimeSpan.FromSeconds(5));
            }
            base.OnExit(e);
        }
    }
}
