using FactoryCube.Interfaces;
using HalconDotNet;
using Microsoft.Extensions.Logging;

namespace FactoryCube.Vision.Vision
{
    public class DummyVisionProcessor : IVisionProcessorService
    {
        private readonly ILogger<DummyVisionProcessor> _logger;

        public DummyVisionProcessor(ILogger<DummyVisionProcessor> logger)
        {
            _logger = logger;
        }

        public HObject Process(HObject image)
        {
            _logger.LogInformation("Dummy processing image.");
            return image;
        }
    }
}
