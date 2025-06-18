using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Vision
{
    public class DefectInspectionResult
    {
        public bool HasDefect { get; set; }
        public List<string> DetectedDefects { get; set; }
        public HImage AnnotatedImage { get; set; }
    }
}
