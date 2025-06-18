using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FactoryCube.Core.Models.Logging;


namespace FactoryCube.Interfaces
{
    public interface IDataLoggingService
    {
        Task LogEventAsync(string category, string message, LogSeverity severity);
        Task LogProcessDataAsync(ProcessData data);
        Task<IEnumerable<LogEntry>> QueryLogsAsync(DateTime from, DateTime to);
    }
}
