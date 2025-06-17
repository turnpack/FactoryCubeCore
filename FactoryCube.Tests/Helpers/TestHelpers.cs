using Moq;
using FactoryCube.Interfaces;

namespace FactoryCube.Tests.Helpers
{
    public static class TestHelpers
    {
        public static Mock<ICameraService> CreateMockCamera()
        {
            var mock = new Mock<ICameraService>();
            mock.Setup(c => c.IsRunning).Returns(false);
            return mock;
        }
    }
}