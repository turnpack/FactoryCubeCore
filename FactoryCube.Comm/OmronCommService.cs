using FactoryCube.Core.Models.Comm;
using FactoryCube.Interfaces;

namespace FactoryCube.Comm
{
    public class OmronCommService : ICommService
    {
        public string LastStatus => throw new NotImplementedException();

        public event EventHandler<PlcEventArgs> PlcEventReceived;

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsConnectedAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> ReadTagAsync<T>(string tagName)
        {
            throw new NotImplementedException();
        }


        public Task<bool> SendCommandAsync(string command, object parameters)
        {
            throw new NotImplementedException();
        }

        public Task WriteTagAsync<T>(string tagName, T value)
        {
            throw new NotImplementedException();
        }
    }
}
