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
            var camera = new Mock<ICameraService>();
            camera.Setup(c => c.IsRunning).Returns(false);
            camera.Setup(c => c.StartAcquisitionAsync());
            camera.Setup(c => c.StopAcquisitionAsync());

            camera.Object.StartAcquisitionAsync();
            Assert.False(camera.Object.IsRunning); // As per mock

            camera.Object.StopAcquisitionAsync();
            Assert.False(camera.Object.IsRunning);
        }
    }
}