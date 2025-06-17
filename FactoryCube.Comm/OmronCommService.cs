using FactoryCube.Interfaces;

namespace FactoryCube.Comm
{
    public class OmronCommService : ICommService
    {
        public string LastStatus => throw new NotImplementedException();

        public Task<bool> SendCommandAsync(string command)
        {
            throw new NotImplementedException();
        }
    }
}
