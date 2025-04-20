using Xunit;
using HalconDotNet;
using FactoryCube.Halcon;
using System;
using FactoryCube.Halcon.Vision;

namespace FactoryCube.Tests
{
    public class BitmapConversionTests
    {
        [Fact]
        public void ConvertHalconImageToBitmapImage_ReturnsValidImage()
        {
            var image = new HObject();
            HOperatorSet.GenImageConst(out image, "byte", 100, 100);
            var bmp = HalconImageConverter.ConvertToBitmapImage(image);
            Assert.NotNull(bmp);
        }
    }
}