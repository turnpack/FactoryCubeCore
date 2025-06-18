using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Vision
{
    public class VisionResult
    {
        public bool Success { get; set; }
        public PointF Location { get; set; }
        public float Angle { get; set; }
        public float Score { get; set; }
    }
}
