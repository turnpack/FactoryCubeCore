namespace FactoryCube.Interfaces
{
    using HalconDotNet;
    using System;

    public interface ICameraService
    {
        event EventHandler<HObject> OnImageGrabbed;
        event EventHandler<string> OnError;
        bool IsRunning { get; }
        void Start();
        void Stop();

        void Close();
        void SetAcquisitionMode(string mode);
    }
}