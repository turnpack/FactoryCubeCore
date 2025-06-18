using FactoryCube.Interfaces;
using FactoryCube.Core.Models.Recipes;

namespace FactoryCube.Services
{
    public class RecipeManagerService : IRecipeManagerService
    {
        public Task<IEnumerable<Recipe>> GetAvailableRecipesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Recipe> LoadRecipeAsync(string recipeId)
        {
            throw new NotImplementedException();
        }

        public Task SaveRecipeAsync(Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public bool ValidateRecipe(Recipe recipe, out List<string> errors)
        {
            throw new NotImplementedException();
        }
    }
}
