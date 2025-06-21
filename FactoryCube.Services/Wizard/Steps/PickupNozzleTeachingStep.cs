using FactoryCube.Core.Models.Wizard;
using FactoryCube.Interfaces.Wizard;
using FactoryCube.Interfaces.Motion;
using FactoryCube.Interfaces.Vision;


namespace FactoryCube.Services.Wizard.Steps
{
    public class PickupNozzleTeachingStep : IRecipeTeachingStep
    {
        private readonly IVisionService _visionService;
        private readonly IZAxisService _zAxisService;

        public string StepId => "pickup";
        public int StepOrder => 1;
        public string StepName => "Pickup Nozzle Teaching";

        public PickupNozzleTeachingStep(IVisionService visionService, IZAxisService zAxisService)
        {
            _visionService = visionService ?? throw new ArgumentNullException(nameof(visionService));
            _zAxisService = zAxisService ?? throw new ArgumentNullException(nameof(zAxisService));
        }
        public string Title => "Teach Pickup Nozzle";

        public async Task ExecuteAsync(TeachingContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Use the vision system to find the nozzle position
            var position = await _visionService.LocatePickupNozzleAsync();

            // Optionally validate Z-axis height
            await _zAxisService.ValidateZHeightAsync();

            // Store result in context
            context.Set("PickupNozzlePosition", position);
        }

        public bool CanProceed(TeachingContext context)
        {
            return context.Data.ContainsKey("PickupZ");
            
        }
    }
}
