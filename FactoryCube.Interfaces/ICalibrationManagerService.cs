using System.Threading.Tasks;
using FactoryCube.Core.Models.Calibration;


namespace FactoryCube.Interfaces
{
    public interface ICalibrationManagerService
    {
        Task StartAutoCalibrationAsync();
        Task StartCameraCalibrationAsync(string cameraId);
        Task StartForceCalibrationAsync();

        bool IsCalibrationInProgress { get; }
        CalibrationResult GetLastCalibrationResult(CalibrationType type);
    }
}
