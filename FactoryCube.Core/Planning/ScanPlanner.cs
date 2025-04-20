using System;
using FactoryCube.Core.Geometry;

namespace FactoryCube.Core.Planning
{
    public class ScanPlanner
    {
        /// <summary>
        /// Generates a list of scan points within the specified region using a snake-style pattern.
        /// </summary>
        /// <param name="region">The rectangular region to scan.</param>
        /// <param name="fov">Field of view configuration (dimensions and overlap).</param>
        /// <returns>A ScanPath object containing ordered scan points.</returns>
        public ScanPath Generate(ScanRegion region, FOVConfig fov)
        {
            if (fov.FovWidth <= 0 || fov.FovHeight <= 0)
                throw new ArgumentException("FOV dimensions must be positive");

            if (fov.Overlap < 0 || fov.Overlap >= 1)
                throw new ArgumentException("Overlap must be between 0 (inclusive) and 1 (exclusive)");

            var path = new ScanPath();

            double stepX = fov.FovWidth * (1 - fov.Overlap);
            double stepY = fov.FovHeight * (1 - fov.Overlap);

            int stepsX = Math.Max(1, (int)Math.Ceiling(region.Width / stepX));
            int stepsY = Math.Max(1, (int)Math.Ceiling(region.Height / stepY));

            for (int y = 0; y < stepsY; y++)
            {
                for (int x = 0; x < stepsX; x++)
                {
                    int posX = (y % 2 == 0) ? x : (stepsX - 1 - x);
                    double px = region.X + posX * stepX + fov.FovWidth / 2;
                    double py = region.Y + y * stepY + fov.FovHeight / 2;

                    path.Add(new ScanPoint(px, py));
                }
            }

            return path;
        }
    }
}
