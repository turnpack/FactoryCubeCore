using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using FactoryCube.Interfaces;
using FactoryCube.Vision.Vision;
using HalconDotNet;
using FactoryCube.Vision.Helpers;
using Microsoft.Extensions.Logging;

namespace FactoryCube.Vision.Processors
{
    public class HalconVisionProcessor : IVisionProcessor
    {
        private readonly ILogger<HalconVisionProcessor> _logger;

        public HalconVisionProcessor(ILogger<HalconVisionProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<DetectionResult[]> ProcessImageAsync(byte[] imageData)
        {
            return await Task.Run(() =>
            {
                // 1) Convert BitmapSource → HImage
                HImage hImg = HalconImageConverter.ToHImage(imageData);

                // 2) Your existing HALCON model call, e.g.:
                HObject regions;
                HOperatorSet.Threshold(hImg, out regions, 128, 255);

                // 3) Extract blobs and map to DetectionResult[]
                HTuple row, col;
                HOperatorSet.AreaCenter(regions, out _, out row, out col);

                var results = new List<DetectionResult>();
                for (int i = 0; i < row.Length; i++)
                {
                    results.Add(new DetectionResult
                    {
                        X = (double)col[i],
                        Y = (double)row[i],
                        Pass = true  // or apply your logic
                    });
                }
                return results.ToArray();
            });
        }
    }
}
