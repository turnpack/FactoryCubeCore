using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Recipes
{
    public class Recipe
    {
        public string RecipeId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        // Add this for modular wizard
        public List<string> StepSequence { get; set; } = new();
    }
}
