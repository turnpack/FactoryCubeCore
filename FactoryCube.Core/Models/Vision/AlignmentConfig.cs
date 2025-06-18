using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Vision
{
    public class AlignmentConfig
    {
        public RectangleF ROI { get; set; }
        public float ExpectedAngle { get; set; }
    }
}
