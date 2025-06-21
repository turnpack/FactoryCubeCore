using System.Threading.Tasks;
using FactoryCube.Interfaces.Vision;
using FactoryCube.Core.Models.Vision;
using HalconDotNet;
using Microsoft.Extensions.Logging;
using FactoryCube.Interfaces;

namespace FactoryCube.Services.Vision
{
    public class HalconVisionService : IVisionService
    {
        private readonly IVisionProcessor _visionProcessor;
        private readonly ILogger<HalconVisionService> _logger;

        public HalconVisionService(IVisionProcessor visionProcessor, ILogger<HalconVisionService> logger)
        {
            _visionProcessor = visionProcessor;
            _logger = logger;
        }

        public async Task<Position2D> LocatePickupNozzleAsync()
        {
            _logger.LogInformation("[Vision] Locating pickup nozzle...");

            // Simulate with dummy image for CLI; replace with actual frame grab logic
            var image = await Task.Run(() => new HImage("fabrik"));

            // Call HALCON search via IVisionProcessor
            var result = await _visionProcessor.RunSearchAsync(image, new VisionSearchConfig());

            if (result.Success)
            {
                _logger.LogInformation($"[Vision] Found pickup at X={result.Location.X}, Y={result.Location.Y}");
                return new Position2D { X = result.Location.X, Y = result.Location.Y };
            }

            throw new VisionException("Pickup nozzle not found.");
        }

        public async Task<Position2D> LocateEjectionPointAsync()
        {
            _logger.LogInformation("[Vision] Locating ejection point...");

            // Simulate with dummy image for CLI
            var image = await Task.Run(() => new HImage("fabrik"));

            var result = await _visionProcessor.RunSearchAsync(image, new VisionSearchConfig());

            if (result.Success)
            {
                _logger.LogInformation($"[Vision] Found eject at X={result.Location.X}, Y={result.Location.X}");
                return new Position2D { X = result.Location.X, Y = result.Location.X };
            }

            throw new VisionException("Ejection point not found.");
        }
    }

    public class VisionException : System.Exception
    {
        public VisionException(string message) : base(message) { }
    }
}
