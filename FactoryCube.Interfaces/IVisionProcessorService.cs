using HalconDotNet;
using System.Threading.Tasks;
using FactoryCube.Core.Models.Vision;

namespace FactoryCube.Interfaces
{
    public class DetectionResult
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool Pass { get; set; }
        // …any other fields…
    }

    public interface IVisionProcessor
    {
        /// <summary>
        /// Runs your die-detection algorithm on the captured image.
        /// </summary>
        Task<DetectionResult[]> ProcessImageAsync(byte[] imageData);
        Task<VisionResult> RunSearchAsync(HImage image, VisionSearchConfig config);
        Task<VisionResult> RunInkDotDetectionAsync(HImage image, InkDotConfig config);
        Task<VisionResult> RunUpwardAlignmentAsync(HImage image, AlignmentConfig config);
        Task<DefectInspectionResult> InspectDieSurfaceAsync(HImage image);

    }
}
