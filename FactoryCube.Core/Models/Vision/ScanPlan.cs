using System;
using System.Collections.Generic;
using System.Drawing;
using FactoryCube.Core.Models.Recipes;

namespace FactoryCube.Core.Models.Vision
{
    public class ScanPlan
    {
        public List<PointF> Targets { get; set; }
        public StepAndRepeatConfig RepeatConfig { get; set; }
    }
}
