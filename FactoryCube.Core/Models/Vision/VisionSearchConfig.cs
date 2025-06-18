using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Vision
{
    public class VisionSearchConfig
    {
        public string AlgorithmType { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
