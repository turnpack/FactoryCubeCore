using System.Threading.Tasks;
using FactoryCube.Core.Models.Production;

namespace FactoryCube.Interfaces
{
    public interface IProductionControlService
    {
        Task StartProductionAsync();
        Task PauseProductionAsync();
        Task ResumeProductionAsync();
        Task StopProductionAsync();
        Task StepAsync();
        ProductionStatus GetCurrentStatus();
        ProductionMetrics GetLiveMetrics();

    }
}
