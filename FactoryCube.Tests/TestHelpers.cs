using Moq;
using FactoryCube.Interfaces;

namespace FactoryCube.Tests
{
    public static class TestHelpers
    {
        public static Mock<ICamera> CreateMockCamera()
        {
            var mock = new Mock<ICamera>();
            mock.Setup(c => c.IsRunning).Returns(false);
            return mock;
        }
    }
}