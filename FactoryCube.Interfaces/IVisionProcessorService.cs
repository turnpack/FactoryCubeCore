using HalconDotNet;
using System.Threading.Tasks;

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

    }
}
