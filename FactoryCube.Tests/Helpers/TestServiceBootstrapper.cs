using Microsoft.Extensions.DependencyInjection;
using FactoryCube.Interfaces;
using FactoryCube.Comm;
using System;

namespace FactoryCube.Tests.Helpers
{
    public static class TestServiceBootstrapper
    {
        public static IServiceProvider Build()
        {
            var services = new ServiceCollection();

            // ✅ Mock services
            services.AddSingleton<ICommService, SimulationCommService>();

            // ✅ Register any ViewModels or dependencies needed by tests
            services.AddSingleton<CameraPreviewViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
