namespace FactoryCube.Interfaces
{
    using HalconDotNet;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IScanExecutorService
    {
        event EventHandler<string> OnStatusUpdate;
        event EventHandler<(int current, int total)> OnProgressUpdate;
        event EventHandler<Exception> OnError;
        event EventHandler<HObject> OnImageCaptured;
        Task<List<HObject>> ExecuteScanAsync(List<(double x, double y)> positions, CancellationToken token);
    }
}