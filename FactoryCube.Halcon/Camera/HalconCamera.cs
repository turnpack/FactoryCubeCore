using HalconDotNet;
using System;
using System.Threading;
using FactoryCube.Interfaces;
using Microsoft.VisualBasic.Logging;
using Microsoft.Extensions.Logging;

namespace FactoryCube.Halcon.Camera
{
    public class HalconCamera : ICamera
    {
        private readonly ILogger<HalconCamera> _logger;

        private Thread _grabThread;
        private volatile bool _running;
        private HFramegrabber _grabber;
        private readonly string _deviceId;
        public string Name { get; set; } = "Default Camera";

        public event EventHandler<HObject> OnImageGrabbed;
        public event EventHandler<string> OnError;
        private CancellationTokenSource? _cancellation;

        public bool IsRunning => _running;

        private readonly string _deviceName;

        public HalconCamera(string deviceId)
        {
            _deviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));
        }

        public void Start()
        {
            try
            {
                _grabber = new HFramegrabber(
                    "USB3Vision", 1, 1, 0, 0, 0, 0,
                    "default", 8, "default", -1, "default", "default",
                    _deviceId, 0, -1);

                // Explicitly set trigger mode to continuous before grabbing
                _grabber.SetFramegrabberParam("TriggerMode", "Off"); // or "Continuous"
                _grabber.SetFramegrabberParam("AcquisitionMode", "Continuous");

                _cancellation = new CancellationTokenSource();
                _grabThread = new Thread(() => GrabLoop(_cancellation.Token));
                _grabThread.IsBackground = true;
                _grabThread.Start();

                _running = true;
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
                _running = false;

                if (_cancellation != null)
                {
                    _cancellation.Cancel();
                    _grabThread?.Join(1000);
                    _cancellation.Dispose();
                    _cancellation = null;
                }

                _grabThread = null;

                if (_grabber != null)
                {
                    try
                    {
                        _grabber.CloseFramegrabber();
                    }
                    catch (HOperatorException hex)
                    {
                        OnError?.Invoke(this, $"Error closing grabber: {hex.Message}");
                    }

                    _grabber.Dispose();
                    _grabber = null;
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Exception during Stop(): {ex.Message}");
            }
        }


        private void GrabLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_grabber == null)
                        return;

                    using HObject image = _grabber.GrabImageAsync(2000);
                    if (image != null && image.IsInitialized())
                    {
                        OnImageGrabbed?.Invoke(this, image);
                    }
                }
                catch (HOperatorException hex)
                {
                    if (hex.Message.Contains("#5322")) // timeout
                    {
                        OnError?.Invoke(this, $"HALCON timeout: {hex.Message}");
                        Thread.Sleep(200);
                    }
                    else
                    {
                        OnError?.Invoke(this, $"HALCON error: {hex.Message}");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(this, $"Unexpected grab loop error: {ex.Message}");
                    break;
                }
            }
        }





        public void SetAcquisitionMode(string mode)
        {
            // Optional: use this for switching to 'continuous', 'software trigger', etc.
            // Example: _grabber.SetFramegrabberParam("AcquisitionMode", mode);
        }

        public void SetParameter(string name, object value)
        {
            if (_grabber == null)
                throw new InvalidOperationException("Camera not initialized");

            try
            {
                var tupleValue = new HTuple(value);
                HOperatorSet.SetFramegrabberParam(_grabber, name, tupleValue);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set parameter '{name}' with value '{value}': {ex.Message}", ex);
            }
        }


        public void SetFramerateAndExposure(double frameRateFps, double exposureTimeUs)
        {
            if (_grabber == null)
                throw new InvalidOperationException("Camera not initialized");

            try
            {
                // Get the maximum exposure time based on the requested frame rate
                double frameIntervalUs = 1_000_000.0 / frameRateFps;

                // Clamp exposure to avoid exceeding the frame interval
                double safeExposure = Math.Min(exposureTimeUs, frameIntervalUs);

                // Optional: log what we're doing
                Console.WriteLine($"Setting framerate to {frameRateFps} fps (frame interval: {frameIntervalUs} µs)");
                Console.WriteLine($"Requested exposure: {exposureTimeUs} µs ? Using: {safeExposure} µs");

                // Set frame rate first
                HOperatorSet.SetFramegrabberParam(_grabber, "AcquisitionFrameRate", new HTuple(frameRateFps));

                // Then set exposure
                HOperatorSet.SetFramegrabberParam(_grabber, "ExposureTime", new HTuple(safeExposure));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Failed to set framerate/exposure: {ex.Message}");
            }
        }


        public void Close()
        {
            try
            {
                Stop(); // stops thread, cancels token, disposes grabber
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Camera close exception: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Close();
        }



    }
}
