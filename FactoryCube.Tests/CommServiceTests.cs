using System.Threading.Tasks;
using Xunit;
using FactoryCube.Interfaces;
using FactoryCube.Tests.Helpers;
using FactoryCube.UI;
using Microsoft.Extensions.DependencyInjection;
using FactoryCube.Comm;


namespace FactoryCube.Tests
{
    public class CommServiceTests
    {
        [Fact]
        public async Task Test_CommService_Mock_Response()
        {
            // Arrange
            var provider = TestServiceBootstrapper.Build();
            var comm = provider.GetRequiredService<ICommService>();

            // Act
            await comm.SendCommandAsync("PING", null);

            // Assert
            Assert.Contains("Mocked", ((SimulationCommService)comm).LastStatus);
        }

    }
}
