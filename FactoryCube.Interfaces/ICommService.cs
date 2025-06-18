using System;
using System.Threading.Tasks;
using FactoryCube.Core.Models.Comm;

namespace FactoryCube.Interfaces
{
    public interface ICommService
    {
        Task ConnectAsync();
        Task<bool> IsConnectedAsync();
        Task<T> ReadTagAsync<T>(string tagName);
        Task WriteTagAsync<T>(string tagName, T value);
        Task<bool> SendCommandAsync(string command, object parameters);

        event EventHandler<PlcEventArgs> PlcEventReceived;
    }
}
