namespace FactoryCube.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public interface IScanPlannerService
    {
        List<(double x, double y)> GenerateScanPath(Rect roi, double fovX, double fovY, double overlap);
        (double actualOverlapX, double actualOverlapY) CalculateActualOverlap(Rect roi, double fovX, double fovY, double requestedOverlap);
    }
}