using FactoryCube.Interfaces;
using FactoryCube.Core.Models.Calibration;

namespace FactoryCube.Services
{
    public class CalibrationManagerService : ICalibrationManagerService
    {
        // TODO: Implement calibration logic
        public bool IsCalibrationInProgress => throw new NotImplementedException();

        public CalibrationResult GetLastCalibrationResult(CalibrationType type)
        {
            throw new NotImplementedException();
        }

        public Task StartAutoCalibrationAsync()
        {
            throw new NotImplementedException();
        }

        public Task StartCameraCalibrationAsync(string cameraId)
        {
            throw new NotImplementedException();
        }

        public Task StartForceCalibrationAsync()
        {
            throw new NotImplementedException();
        }
    }
}
