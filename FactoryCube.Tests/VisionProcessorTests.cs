using Xunit;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using FactoryCube.Interfaces;
using FactoryCube.Vision.Processors;
using Microsoft.Extensions.Logging.Abstractions;
using FactoryCube.UI.Helpers;

namespace FactoryCube.Tests.Vision
{
    public class VisionProcessorTests
    {
        [Fact]
        public async Task HalconVisionProcessor_ReturnsResults_FromBitmap()
        {
            // Arrange
            IVisionProcessor processor = new HalconVisionProcessor(NullLogger<HalconVisionProcessor>.Instance);

            // Replace with a valid test image in your test data directory
            string path = Path.Combine("TestData", "intu_surg_beam.bmp");
            Assert.True(File.Exists(path), $"Missing test image: {path}");

            var bmp = new BitmapImage();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = fs;
                bmp.EndInit();
                bmp.Freeze();
            }

            // Act
            var imageBytes = ImageHelpers.BitmapImageToByteArray(bmp);
            var results = await processor.ProcessImageAsync(imageBytes);

            // Assert
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.All(results, r =>
            {
                Assert.InRange(r.X, 0, bmp.PixelWidth);
                Assert.InRange(r.Y, 0, bmp.PixelHeight);
            });
        }
    }
}
