using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FactoryCube.Core.Models.Recipes;
using FactoryCube.Core.Models.Wizard;
using FactoryCube.Interfaces.Wizard;
using FactoryCube.Services.Wizard;


namespace FactoryCube.UI.ViewModels
{
    public class WizardViewModel
    {
        public ObservableCollection<string> ExecutedSteps { get; } = new();
        public string CurrentStep { get; private set; }

        public string ErrorMessage { get; private set; }

        public async Task RunAsync(IRecipeWizardService wizardService, Recipe recipe)
        {
            var context = new TeachingContext();

            try
            {
                foreach (var stepId in recipe.StepSequence)
                {
                    CurrentStep = stepId;
                    ExecutedSteps.Add($"Started: {stepId}");
                }

                await wizardService.RunWizardAsync(recipe, context);

                foreach (var stepId in recipe.StepSequence)
                {
                    ExecutedSteps.Add($"Completed: {stepId}");
                }

                CurrentStep = null;
            }
            catch (RecipeTeachingException rex)
            {
                ErrorMessage = $"Step failed: {rex.Message}";
            }
        }
    }
}
