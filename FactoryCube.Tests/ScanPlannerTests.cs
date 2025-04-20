using Xunit;
using FactoryCube.Core.Planning;
using FactoryCube.Core.Geometry;

namespace FactoryCube.Tests
{
    public class ScanPlannerTests
    {
        [Fact]
        public void GenerateScanPath_CenterOfRoiIsCorrect()
        {
            var planner = new ScanPlanner();
            var region = new ScanRegion(0, 0, 10, 10);
            var fov = new FOVConfig(10, 10, 0);

            var path = planner.Generate(region, fov);

            Assert.Single(path);
            Assert.Equal(5, path[0].X);
            Assert.Equal(5, path[0].Y);
        }
    }
}
