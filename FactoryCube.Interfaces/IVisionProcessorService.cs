using HalconDotNet;

namespace FactoryCube.Interfaces
{
    public interface IVisionProcessorService
    {
        HObject Process(HObject image);
    }
}
