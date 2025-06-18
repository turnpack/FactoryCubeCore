using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Logging
{
    public class ProcessData
    {
        public string RecipeId { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Values { get; set; }
    }
}
