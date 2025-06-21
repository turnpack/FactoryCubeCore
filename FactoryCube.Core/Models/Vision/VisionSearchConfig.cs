using System.Drawing;

namespace FactoryCube.Core.Models.Vision
{
    public class VisionSearchConfig
    {
        /// <summary>
        /// Optional name of the shape or template to search for.
        /// </summary>
        public string ModelId { get; set; } = "default";

        /// <summary>
        /// Search region of interest (ROI).
        /// </summary>
        public RectangleF RegionOfInterest { get; set; } = new RectangleF(0, 0, 0, 0);

        /// <summary>
        /// Minimum match score to accept a result (0.0–1.0).
        /// </summary>
        public float MinScore { get; set; } = 0.7f;

        /// <summary>
        /// Maximum number of matches to return.
        /// </summary>
        public int MaxResults { get; set; } = 1;

        /// <summary>
        /// Optional search angle tolerance in degrees.
        /// </summary>
        public float AngleTolerance { get; set; } = 10f;
    }
}
