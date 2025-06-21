using System.Threading.Tasks;
using FactoryCube.Interfaces.Motion;
using Microsoft.Extensions.Logging;

namespace FactoryCube.Services.Motion
{
    public class ZAxisMotionService : IZAxisService
    {
        private readonly ILogger<ZAxisMotionService> _logger;

        public ZAxisMotionService(ILogger<ZAxisMotionService> logger)
        {
            _logger = logger;
        }

        public async Task ValidateZHeightAsync()
        {
            _logger.LogInformation("[Z] Validating Z height...");
            // Simulate Z validation delay
            await Task.Delay(100);
            _logger.LogInformation("[Z] Z validation passed.");
        }
    }
}
