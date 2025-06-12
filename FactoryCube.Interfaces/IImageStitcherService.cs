namespace FactoryCube.Interfaces
{
    using HalconDotNet;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    public interface IImageStitcherService
    {
        Task<BitmapImage> StitchAsync(List<HObject> images);
    }
}