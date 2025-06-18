using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Vision
{
    public class ScanParameters
    {
        public string CameraId { get; set; }
        public RectangleF Region { get; set; }
        public int StepX { get; set; }
        public int StepY { get; set; }
    }
}
