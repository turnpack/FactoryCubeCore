using FactoryCube.Interfaces;
using FactoryCube.Core.Models.Calibration;

namespace FactoryCube.Services
{
    public class CalibrationManager : ICalibrationService
    {
        string ICalibrationService.Name => throw new NotImplementedException();

        CalibrationType ICalibrationService.Type => throw new NotImplementedException();

        Task<CalibrationResult> ICalibrationService.RunAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
