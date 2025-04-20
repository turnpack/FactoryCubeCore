
namespace FactoryCube.Core.Utilities
{
    public static class MathHelper
    {
        public static double Clamp(double value, double min, double max)
        {
            return value < min ? min : (value > max ? max : value);
        }
    }
}
