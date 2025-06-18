using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Language;
using FactoryCube.Interfaces.Vision;
using FactoryCube.Interfaces.Motion;
using FactoryCube.Core.Models.Wizard;
using FactoryCube.Services.Wizard.Steps;

namespace FactoryCube.Tests.ModularRecipeWizard
{
    public class PickupNozzleTeachingStepTests
    {
        private readonly Mock<IVisionService> _visionServiceMock;
        private readonly Mock<IZAxisService> _zAxisServiceMock;
        private readonly TeachingContext _context;
        private readonly PickupNozzleTeachingStep _step;

        public PickupNozzleTeachingStepTests()
        {
            _visionServiceMock = new Mock<IVisionService>();
            _zAxisServiceMock = new Mock<IZAxisService>();
            _context = new TeachingContext();

            _step = new PickupNozzleTeachingStep(
                _visionServiceMock.Object,
                _zAxisServiceMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_ShouldStorePickupPositionInContext_WhenVisionSucceeds()
        {
            // Arrange
            var expectedPosition = new Position2D { X = 123.45, Y = 67.89 };
            _visionServiceMock
                .Setup(v => v.LocatePickupNozzleAsync())
                .Returns(Task.FromResult(expectedPosition));


            _zAxisServiceMock
                .Setup(z => z.ValidateZHeightAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _step.ExecuteAsync(_context);

            // Assert
            Assert.True(_context.Has("PickupNozzlePosition"));
            var actual = _context.Get<Position2D>("PickupNozzlePosition");

            Assert.Equal(expectedPosition.X, actual.X);
            Assert.Equal(expectedPosition.Y, actual.Y);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenVisionFails()
        {
            // Arrange
            _visionServiceMock
                .Setup(v => v.LocatePickupNozzleAsync())
                .ThrowsAsync(new Exception("Vision failure"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _step.ExecuteAsync(_context));
            Assert.Contains("Vision failure", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrow_WhenZValidationFails()
        {
            // Arrange
            _visionServiceMock
                .Setup(v => v.LocatePickupNozzleAsync())
                .Returns(Task.FromResult(new Position2D { X = 10, Y = 10 }));


            _zAxisServiceMock
                .Setup(z => z.ValidateZHeightAsync())
                .ThrowsAsync(new InvalidOperationException("Z axis blocked"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _step.ExecuteAsync(_context));
            Assert.Equal("Z axis blocked", ex.Message);
        }
    }


}
