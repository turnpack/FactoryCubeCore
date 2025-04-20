namespace FactoryCube.Interfaces
{
    using HalconDotNet;
    using System;

    public interface ICamera
    {
        event EventHandler<HObject> OnImageGrabbed;
        event EventHandler<string> OnError;
        bool IsRunning { get; }
        void Start();
        void Stop();
        void SetAcquisitionMode(string mode);
    }
}