
using HalconDotNet;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
namespace FactoryCube.Interfaces
{


    public interface IImageStitcherService
    {
        Task<byte[]> StitchAsync(List<HObject> images);
    }
}