using FactoryCube.Core.Models.Recipes;
using FactoryCube.Core.Models.Wizard;
using FactoryCube.Interfaces.Wizard;

namespace FactoryCube.Services.Wizard
{
    public class RecipeTeachingWizardService : IRecipeWizardService
    {
        private readonly List<IRecipeTeachingStep> _allSteps;

        public RecipeTeachingWizardService(IEnumerable<IRecipeTeachingStep> steps)
        {
            if (steps == null) throw new ArgumentNullException(nameof(steps));
            _allSteps = steps.ToList();
        }

        public async Task RunWizardAsync(Recipe recipe, TeachingContext context)
        {
            if (recipe == null) throw new ArgumentNullException(nameof(recipe));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var stepsToRun = _allSteps
                .Where(s => recipe.StepSequence.Contains(s.StepId))
                .OrderBy(s => recipe.StepSequence.IndexOf(s.StepId))
                .ToList();

            foreach (var step in stepsToRun)
            {
                try
                {
                    Console.WriteLine($"[Wizard] Executing: {step.StepName} (ID: {step.StepId})");
                    await step.ExecuteAsync(context);
                }
                catch (Exception ex)
                {
                    throw new RecipeTeachingException(
                        $"Error during step '{step.StepName}' (ID: {step.StepId})", ex);
                }
            }
        }

        /// <summary>
        /// Legacy fallback: executes all registered steps in declared order.
        /// </summary>
        public async Task RunWizardAsync(TeachingContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var orderedSteps = _allSteps.OrderBy(s => s.StepOrder).ToList();

            foreach (var step in orderedSteps)
            {
                try
                {
                    await step.ExecuteAsync(context);
                }
                catch (Exception ex)
                {
                    throw new RecipeTeachingException(
                        $"Error during step '{step.StepName}'", ex);
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
