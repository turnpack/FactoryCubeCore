using HalconDotNet;
using System;
using System.Threading;
using System.Threading.Tasks;
using FactoryCube.Interfaces;
using Microsoft.Extensions.Logging;

namespace FactoryCube.Vision.Camera
{
    public class HalconCameraService : ICameraService, IDisposable
    {
        private readonly ILogger<HalconCameraService> _logger;
        private HFramegrabber _grabber;
        private Thread _grabThread;
        private CancellationTokenSource _cancellation;
        private volatile bool _running;

        public string Name { get; set; } = "Default Camera";
        public bool IsRunning => _running;

        public event EventHandler<HObject> OnImageGrabbed;
        public event EventHandler<string> OnError;

        private readonly string _deviceId;

        public HalconCameraService(string deviceId, ILogger<HalconCameraService> logger)
        {
            _deviceId = deviceId ?? throw new ArgumentNullException(nameof(deviceId));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InitializeAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    _grabber = new HFramegrabber(
                        "USB3Vision", 1, 1, 0, 0, 0, 0,
                        "default", 8, "default", -1, "default", "default",
                        _deviceId, 0, -1);

                    // continuous trigger
                    _grabber.SetFramegrabberParam("TriggerMode", "Off");
                    _grabber.SetFramegrabberParam("AcquisitionMode", "Continuous");
                });
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"InitializeAsync failed: {ex.Message}");
            }
        }

        public async Task StartAcquisitionAsync()
        {
            if (_grabber == null)
                throw new InvalidOperationException("Camera not initialized");

            await Task.Run(() =>
            {
                _cancellation = new CancellationTokenSource();
                _grabThread = new Thread(() => GrabLoop(_cancellation.Token))
                {
                    IsBackground = true
                };
                _grabThread.Start();
                _running = true;
            });
        }



        public async Task StopAcquisitionAsync()
        {
            await Task.Run(() =>
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
            });
        }

        public async Task CloseAsync()
        {
            await StopAcquisitionAsync();

            if (_grabber != null)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        _grabber.CloseFramegrabber();
                    }
                    catch (HOperatorException hex)
                    {
                        OnError?.Invoke(this, $"Error closing grabber: {hex.Message}");
                    }
                    finally
                    {
                        _grabber.Dispose();
                        _grabber = null;
                    }
                });
            }
        }

        public async Task SetAcquisitionModeAsync(string mode)
        {
            if (_grabber == null)
                throw new InvalidOperationException("Camera not initialized");

            await Task.Run(() =>
            {
                _grabber.SetFramegrabberParam("AcquisitionMode", mode);
            });
        }

        private void GrabLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using var image = _grabber.GrabImageAsync(2000);
                    if (image != null && image.IsInitialized())
                        OnImageGrabbed?.Invoke(this, image);
                }
                catch (HOperatorException hex)
                {
                    if (hex.Message.Contains("#5322"))
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
                    OnError?.Invoke(this, $"Unexpected grab error: {ex.Message}");
                    break;
                }
            }
        }

        public void Dispose()
        {
            // block to fully close before disposing
            CloseAsync().GetAwaiter().GetResult();
        }



        public async Task SetParameterAsync(string name, object value)
        {
            if (_grabber == null) throw new InvalidOperationException("Camera not initialized");
            await Task.Run(() =>
            {
                var tupleValue = new HTuple(value);
                HOperatorSet.SetFramegrabberParam(_grabber, name, tupleValue);
            });
        }

        public async Task SetFrameRateAndExposureAsync(double frameRateFps, double exposureTimeUs)
        {
            if (_grabber == null) throw new InvalidOperationException("Camera not initialized");
            await Task.Run(() =>
            {
                double frameIntervalUs = 1_000_000.0 / frameRateFps;
                double safeExposure = Math.Min(exposureTimeUs, frameIntervalUs);
                HOperatorSet.SetFramegrabberParam(_grabber, "AcquisitionFrameRate", new HTuple(frameRateFps));
                HOperatorSet.SetFramegrabberParam(_grabber, "ExposureTime", new HTuple(safeExposure));
            });
        }
    }
}
