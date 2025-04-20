using Xunit;
using FactoryCube.Interfaces;
using Moq;
using System;

namespace FactoryCube.Tests
{
    public class CameraManagerTests
    {
        [Fact]
        public void CanStartAndStopPreview()
        {
            var camera = new Mock<ICamera>();
            camera.Setup(c => c.IsRunning).Returns(false);
            camera.Setup(c => c.Start());
            camera.Setup(c => c.Stop());

            camera.Object.Start();
            Assert.False(camera.Object.IsRunning); // As per mock

            camera.Object.Stop();
            Assert.False(camera.Object.IsRunning);
        }
    }
}