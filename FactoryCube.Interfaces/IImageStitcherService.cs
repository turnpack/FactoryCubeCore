using HalconDotNet;
using System.Collections.Generic;
using System.Threading.Tasks;
using FactoryCube.Core.Models.Vision;

namespace FactoryCube.Interfaces
{


    public interface IImageStitcherService
    {
        Task<HImage> StitchAsync(IEnumerable<HImage> inputImages, StitchingParameters parameters);

    }
}