using HalconDotNet;
using System;

namespace FactoryCube.Halcon.Vision
{
    public static class HalconImageUtils
    {
        public static bool IsInitialized(HObject image)
        {
            return image != null && image.IsInitialized();
        }

        public static void DisposeImage(HObject? image)
        {
            if (image != null && image.IsInitialized())
            {
                image.Dispose();
            }
        }
    }
}
