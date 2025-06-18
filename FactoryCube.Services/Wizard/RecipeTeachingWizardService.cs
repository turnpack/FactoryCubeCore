using FactoryCube.Core.Models.Wizard;
using FactoryCube.Interfaces.Wizard;

namespace FactoryCube.Services.Wizard
{
    public class RecipeTeachingWizardService : IRecipeWizardService
    {
        private readonly List<IRecipeTeachingStep> _orderedSteps;

        public RecipeTeachingWizardService(IEnumerable<IRecipeTeachingStep> steps)
        {
            if (steps == null) throw new ArgumentNullException(nameof(steps));

            // Sort steps by defined StepOrder
            _orderedSteps = steps.OrderBy(s => s.StepOrder).ToList();
        }

        public async Task RunWizardAsync(TeachingContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            foreach (var step in _orderedSteps)
            {
                try
                {
                    // Optional: add debug/log here
                    Console.WriteLine($"[Wizard] Executing: {step.StepName} (Order {step.StepOrder})");
                    await step.ExecuteAsync(context);
                }
                catch (Exception ex)
                {
                    throw new RecipeTeachingException(
                        $"Error during step '{step.StepName}' (Order {step.StepOrder})", ex);
                }
            }
        }
    }

    public class RecipeTeachingException : Exception
    {
        public RecipeTeachingException(string message, Exception inner)
            : base(message, inner) { }
    }
}
