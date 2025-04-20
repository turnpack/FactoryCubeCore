namespace FactoryCube.Interfaces
{
    using HalconDotNet;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    public interface IImageStitcher
    {
        Task<BitmapImage> StitchAsync(List<HObject> images);
    }
}