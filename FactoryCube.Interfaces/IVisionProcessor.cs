using HalconDotNet;

namespace FactoryCube.Interfaces
{
    public interface IVisionProcessor
    {
        HObject Process(HObject image);
    }
}
