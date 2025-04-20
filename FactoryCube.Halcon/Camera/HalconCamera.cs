using HalconDotNet;
using System;
using System.Threading;
using FactoryCube.Interfaces;

namespace FactoryCube.Halcon.Camera
{
    public class HalconCamera : ICamera
    {
        private Thread _grabThread;
        private volatile bool _running;
        private HFramegrabber _grabber;

        public event EventHandler<HObject> OnImageGrabbed;
        public event EventHandler<string> OnError;
        private CancellationTokenSource? _cancellation;

        public bool IsRunning => _running;

        public void Start()
        {
            try
            {
                _grabber = new HFramegrabber(
                    "USB3Vision",        // Interface name
                    1,                   // Horizontal resolution
                    1,                   // Vertical resolution
                    0,                   // ImageWidth
                    0,                   // ImageHeight
                    0,                   // StartRow
                    0,                   // StartColumn
                    "default",           // Field
                    8,                   // Bits per channel
                    "default",           // ColorSpace
                    -1,                  // Generic
                    "default",           // ExternalTrigger
                    "default",           // CameraType
                    "default",           // Device - try default device
                    0,                   // Port
                    -1);

                _running = true;
                _cancellation = new CancellationTokenSource();
                _grabThread = new Thread(() => GrabLoop(_cancellation.Token));
                _grabThread.Start();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Failed to start camera: {ex.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                _cancellation?.Cancel();
                _grabThread?.Join(1000);
                _grabber?.Dispose();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Error while stopping camera: {ex.Message}");
            }
        }


        private void GrabLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    HObject image = _grabber.GrabImage();

                    if (image.IsInitialized())
                    {
                        OnImageGrabbed?.Invoke(this, image);
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, $"Camera error: {ex.Message}");
                }
            }
        }


        public void SetAcquisitionMode(string mode)
        {
            // Optional: use this for switching to 'continuous', 'software trigger', etc.
            // Example: _grabber.SetFramegrabberParam("AcquisitionMode", mode);
        }
    }
}
