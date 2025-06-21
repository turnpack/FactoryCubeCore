using FactoryCube.Core.Models.Wizard;
using FactoryCube.Interfaces.Wizard;
using FactoryCube.Interfaces.Motion;
using FactoryCube.Interfaces.Vision;

namespace FactoryCube.Services.Wizard.Steps
{
    public class EjectionSystemTeachingStep : IRecipeTeachingStep
    {
        private readonly IVisionService _visionService;
        private readonly IZAxisService _zAxisService;
        public string StepId => "eject";
        public int StepOrder => 2;
        public string StepName => "Ejection System Teaching";

        public EjectionSystemTeachingStep(IVisionService visionService, IZAxisService zAxisService)
        {
            _visionService = visionService ?? throw new ArgumentNullException(nameof(visionService));
            _zAxisService = zAxisService ?? throw new ArgumentNullException(nameof(zAxisService));
        }

        public string Title => "Teach Ejection System";

        public bool CanProceed(TeachingContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ExecuteAsync(TeachingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // 1. Use vision to locate the ejection point
            var ejectionPosition = await _visionService.LocateEjectionPointAsync();

            // 2. Optionally validate Z axis height
            await _zAxisService.ValidateZHeightAsync();

            // 3. Store in context
            context.Set("EjectionSystemPosition", ejectionPosition);
        }
    }
}
