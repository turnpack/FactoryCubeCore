using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FactoryCube.Core.Models.Recipes;
using FactoryCube.Interfaces.Wizard;
using FactoryCube.Core.Models.Wizard;

namespace FactoryCube.UI
{
    public class WizardCliRunner
    {
        public static async Task Run(string recipePath, IRecipeWizardService wizardService)
        {
            var recipeJson = File.ReadAllText(recipePath);
            var recipe = JsonSerializer.Deserialize<Recipe>(recipeJson);

            var context = new TeachingContext();
            try
            {
                await wizardService.RunWizardAsync(recipe, context);
                Console.WriteLine("✅ Recipe executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Recipe execution failed: {ex.Message}");
            }
        }
    }
}
