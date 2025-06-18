using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Vision
{
    public class ScanResult
    {
        public bool Success { get; set; }
        public List<PointF> Locations { get; set; }
        public string DiagnosticMessage { get; set; }
    }
}
