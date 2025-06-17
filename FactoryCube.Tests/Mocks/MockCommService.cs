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

        public Task<bool> SendCommandAsync(string command)
        {
            LastStatus = $"Mocked Command: {command}";
            return Task.FromResult(true);
        }
    }
}

