
using System;

namespace FactoryCube.Core.Models
{
    public class RecipeMetadata
    {
        public string Name { get; set; } = "";
        public string Operator { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
