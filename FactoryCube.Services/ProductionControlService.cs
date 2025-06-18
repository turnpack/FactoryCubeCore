using FactoryCube.Interfaces;
using FactoryCube.Core.Models.Production;


namespace FactoryCube.Services
{
    public class ProductionControlService : IProductionControlService
    {
        public ProductionStatus GetCurrentStatus()
        {
            throw new NotImplementedException();
        }

        public ProductionMetrics GetLiveMetrics()
        {
            throw new NotImplementedException();
        }

        public Task PauseProductionAsync()
        {
            throw new NotImplementedException();
        }

        public Task ResumeProductionAsync()
        {
            throw new NotImplementedException();
        }

        public Task StartProductionAsync()
        {
            throw new NotImplementedException();
        }

        public Task StepAsync()
        {
            throw new NotImplementedException();
        }

        public Task StopProductionAsync()
        {
            throw new NotImplementedException();
        }
    }
}
