using System;
using FactoryCube.Interfaces;

namespace FactoryCube.Comm
{
    public class SimulationCommService : ICommService
    {
        public string LastStatus => throw new NotImplementedException();

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

        public void SendCommand(string commandTag, object value)
        {
            Console.WriteLine($"Simulated sending: {commandTag} = {value}");
        }

        public Task<bool> SendCommandAsync(string command)
        {
            throw new NotImplementedException();
        }
    }
}
