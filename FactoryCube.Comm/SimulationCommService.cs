using System;
using FactoryCube.Core.Models.Comm;
using FactoryCube.Interfaces;

namespace FactoryCube.Comm
{
    public class SimulationCommService : ICommService
    {
        public string LastStatus;

        public event EventHandler<PlcEventArgs> PlcEventReceived;

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public double GetAxisPosition(string axis)
        {
            // Simulated axis position
            return axis switch
            {
                "X" => 100.0,
                "Y" => 200.0,
                "Z" => 300.0,
                _ => 0.0
            };
        }

        public bool GetInputState(string inputTag) => inputTag == "VacuumSensor";

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
