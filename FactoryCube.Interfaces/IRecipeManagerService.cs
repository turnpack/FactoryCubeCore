using System.Collections.Generic;
using System.Threading.Tasks;
using FactoryCube.Core.Models.Recipes;

namespace FactoryCube.Interfaces
{
    public interface IRecipeManagerService
    {
        Task<Recipe> LoadRecipeAsync(string recipeId);
        Task SaveRecipeAsync(Recipe recipe);
        Task<IEnumerable<Recipe>> GetAvailableRecipesAsync();
        bool ValidateRecipe(Recipe recipe, out List<string> errors);

    }
}
