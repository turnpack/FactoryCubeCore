using HalconDotNet;

namespace FactoryCube.Halcon.Utilities
{
    public static class HalconHelpers
    {
        public static bool IsInitialized(HObject obj)
        {
            return obj != null && obj.IsInitialized();
        }
    }
}
