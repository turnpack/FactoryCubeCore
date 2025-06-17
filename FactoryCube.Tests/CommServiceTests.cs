using System.Threading.Tasks;
using Xunit;
using FactoryCube.Interfaces;
using FactoryCube.Tests.Helpers;
using FactoryCube.UI;
using Microsoft.Extensions.DependencyInjection;


namespace FactoryCube.Tests
{
    public class CommServiceTests
    {
        [Fact]
        public async Task Test_CommService_Mock_Response()
        {
            // Arrange
            App.ServiceProvider = TestServiceBootstrapper.Build();
            var comm = App.ServiceProvider.GetRequiredService<ICommService>();

            // Act
            var result = await comm.SendCommandAsync("PING");

            // Assert
            Assert.True(result);
            Assert.Contains("Mocked", comm.LastStatus);
        }
    }
}
