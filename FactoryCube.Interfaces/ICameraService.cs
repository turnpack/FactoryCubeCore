using HalconDotNet;
using System.Threading.Tasks;
using System;


namespace FactoryCube.Interfaces
{


    public interface ICameraService
    {
        event EventHandler<HObject> OnImageGrabbed;
        event EventHandler<string> OnError;
        bool IsRunning { get; }

        Task InitializeAsync();                // was void Start()
        Task StartAcquisitionAsync();          // was void Start()
        Task StopAcquisitionAsync();           // was void Stop()
        Task CloseAsync();                     // was void Close()
        Task SetAcquisitionModeAsync(string mode);  // was void SetAcquisitionMode()
        Task SetParameterAsync(string name, object value);
        Task SetFrameRateAndExposureAsync(double frameRateFps, double exposureTimeUs);



        //void Start();
        //void Stop();
        //void Close();
        //void SetAcquisitionMode(string mode);
    }
}