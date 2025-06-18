using FactoryCube.Interfaces;
using FactoryCube.Core.Models.Logging;

namespace FactoryCube.Services
{
    public class DataLoggingService : IDataLoggingService
    {
        public Task LogEventAsync(string category, string message, LogSeverity severity)
        {
            throw new NotImplementedException();
        }

        public Task LogProcessDataAsync(ProcessData data)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LogEntry>> QueryLogsAsync(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }
    }
}
