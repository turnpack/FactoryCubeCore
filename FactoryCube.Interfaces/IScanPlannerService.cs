using System;
using System.Collections.Generic;
using System.Windows;
using HalconDotNet;
using FactoryCube.Interfaces.Models;

namespace FactoryCube.Interfaces
{


    public interface IScanPlannerService
    {
        List<(double x, double y)> GenerateScanPath(RectD roi, double fovX, double fovY, double overlap);
        (double actualOverlapX, double actualOverlapY) CalculateActualOverlap(RectD roi, double fovX, double fovY, double requestedOverlap);
    }
}