using FactoryCube.Core.Models.Comm;
using FactoryCube.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FactoryCube.Tests.Mocks
{
    public class MockCommService : ICommService
    {
        public string LastStatus { get; private set; } = "Mock Ready";

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
            LastStatus = $"Mocked Command: {command}";
            return Task.FromResult(true);
        }




        public Task WriteTagAsync<T>(string tagName, T value)
        {
            throw new NotImplementedException();
        }

    }
}

