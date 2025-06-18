using System.Threading;
using System.Threading.Tasks;
using FactoryCube.Core.Models.Calibration;

namespace FactoryCube.Interfaces
{
    public interface ICalibrationService
    {
        Task<CalibrationResult> RunAsync(CancellationToken token);
        string Name { get; }
        CalibrationType Type { get; }
    }
}
