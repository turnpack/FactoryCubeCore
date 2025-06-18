using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FactoryCube.Interfaces.Vision;
using FactoryCube.Interfaces.Motion;
using FactoryCube.Core.Models.Wizard;
using FactoryCube.Services.Wizard.Steps;

namespace FactoryCube.Tests
{
    public class EjectionSystemTeachingStepTests
    {
        private readonly Mock<IVisionService> _visionServiceMock;
        private readonly Mock<IZAxisService> _zAxisServiceMock;
        private readonly TeachingContext _context;
        private readonly EjectionSystemTeachingStep _step;

        public EjectionSystemTeachingStepTests()
        {
            _visionServiceMock = new Mock<IVisionService>();
            _zAxisServiceMock = new Mock<IZAxisService>();
            _context = new TeachingContext();

            _step = new EjectionSystemTeachingStep(
                _visionServiceMock.Object,
                _zAxisServiceMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_ShouldStoreEjectionPosition_WhenVisionAndZAreValid()
        {
            // Arrange
            var expectedPosition = new Position2D { X = 77.0, Y = 33.0 };
            _visionServiceMock
                .Setup(v => v.LocateEjectionPointAsync())
                .Returns(Task.FromResult(expectedPosition));

            _zAxisServiceMock
                .Setup(z => z.ValidateZHeightAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _step.ExecuteAsync(_context);

            // Assert
            Assert.True(_context.Has("EjectionSystemPosition"));
            var actual = _context.Get<Position2D>("EjectionSystemPosition");

            Assert.Equal(expectedPosition.X, actual.X);
            Assert.Equal(expectedPosition.Y, actual.Y);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenVisionFails()
        {
            // Arrange
            _visionServiceMock
                .Setup(v => v.LocateEjectionPointAsync())
                .ThrowsAsync(new Exception("Vision error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _step.ExecuteAsync(_context));
            Assert.Contains("Vision error", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenZAxisFails()
        {
            // Arrange
            _visionServiceMock
                .Setup(v => v.LocateEjectionPointAsync())
                .Returns(Task.FromResult(new Position2D { X = 1, Y = 1 }));

            _zAxisServiceMock
                .Setup(z => z.ValidateZHeightAsync())
                .ThrowsAsync(new InvalidOperationException("Z axis failed"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _step.ExecuteAsync(_context));
            Assert.Equal("Z axis failed", ex.Message);
        }
    }
}
