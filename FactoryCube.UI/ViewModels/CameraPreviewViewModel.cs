using System.Collections.ObjectModel;
using FactoryCube.Interfaces;
using System.Windows.Media.Imaging;
using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using FactoryCube.UI.ViewModels;
using System.Collections.Generic;
using FactoryCube.Halcon.Vision;
using FactoryCube.UI;
using HalconDotNet;
using FactoryCube.Core.Models;
using FactoryCube.Halcon.Camera;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;

public class CameraPreviewViewModel : ViewModelBase
{
    private readonly ILogger<CameraPreviewViewModel> _logger;
    private ICamera? _camera;
    private readonly Func<string, ICamera> _cameraFactory;
    public HWindow? HalconWindow { get; set; }

    public IRelayCommand RefreshCamerasCommand { get; }


    public ObservableCollection<CameraInfo> AvailableCameras { get; } = new();
    private CameraInfo? _selectedCamera;
    public CameraInfo? SelectedCamera
    {
        get => _selectedCamera;
        set
        {
            if (_selectedCamera != null)
            {
                _selectedCamera.PropertyChanged -= OnSelectedCameraChanged;
            }

            if (SetProperty(ref _selectedCamera, value))
            {
                Log($"Camera selected: {value?.Name}");

                if (_selectedCamera != null)
                {
                    _selectedCamera.PropertyChanged += OnSelectedCameraChanged;
                }

                if (IsPreviewRunning)
                {
                    StopPreview();
                    StartPreview();
                }
            }
        }
    }

    private void OnSelectedCameraChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!IsPreviewRunning || _camera is not HalconCamera halcon || SelectedCamera == null)
            return;

        try
        {
            switch (e.PropertyName)
            {
                case nameof(CameraInfo.ExposureTime):
                case nameof(CameraInfo.Gain):
                case nameof(CameraInfo.FrameRate):
                case nameof(CameraInfo.TriggerMode):
                    Log($"Parameter changed: {e.PropertyName}, reapplying settings...");
                    halcon.SetFramerateAndExposure(SelectedCamera.FrameRate, SelectedCamera.ExposureTime);
                    halcon.SetParameter("Gain", SelectedCamera.Gain);
                    halcon.SetParameter("TriggerMode", SelectedCamera.TriggerMode);
                    break;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to apply setting: {e.PropertyName}: {ex.Message}";
            _logger.LogError(ex, "Parameter update error");
        }
    }



    private BitmapImage? _liveImage;
    public BitmapImage? LiveImage
    {
        get => _liveImage;
        set => SetProperty(ref _liveImage, value);
    }

    private string _logOutput = string.Empty;
    public string LogOutput
    {
        get => _logOutput;
        set => SetProperty(ref _logOutput, value);
    }

    public IRelayCommand StartPreviewCommand { get; }
    public IRelayCommand StopPreviewCommand { get; }
    public IRelayCommand ApplyCameraSettingsCommand { get; }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }


    public CameraPreviewViewModel(Func<string, ICamera> cameraFactory, ILogger<CameraPreviewViewModel> logger)
    {
        _cameraFactory = cameraFactory;
        _logger = logger;

        StartPreviewCommand = new RelayCommand(StartPreview, () => !IsPreviewRunning);
        StopPreviewCommand = new RelayCommand(StopPreview, () => IsPreviewRunning);
        RefreshCamerasCommand = new RelayCommand(DetectAvailableCameras);
        ApplyCameraSettingsCommand = new RelayCommand(ApplyCameraSettings);



        Log("CameraPreviewViewModel initialized.");
    }


    private bool _isPreviewRunning;
    public bool IsPreviewRunning
    {
        get => _isPreviewRunning;
        private set
        {
            if (SetProperty(ref _isPreviewRunning, value))
            {
                StartPreviewCommand.NotifyCanExecuteChanged();
                StopPreviewCommand.NotifyCanExecuteChanged();
            }
        }
    }





    public void DetectAvailableCameras()
    {
        AvailableCameras.Clear();

        try
        {
            // Query all devices for USB3Vision
            HOperatorSet.InfoFramegrabber("USB3Vision", "device", out HTuple info, out HTuple devices);

            for (int i = 0; i < devices.Length; i++)
            {
                string deviceId = devices[i].S;
                string vendor = "N/A", model = "N/A", serial = "N/A";
                string exposure = "N/A", gain = "N/A", framerate = "N/A", triggerMode = "N/A";

                try
                {
                    // Open a temporary connection to query metadata
                    using var grabber = new HFramegrabber(
                        "USB3Vision",
                        1, 1, 0, 0, 0, 0,
                        "default", 8, "default",
                        -1, "default", "default",
                        deviceId, 0, -1);

                    HOperatorSet.GetFramegrabberParam(grabber, "DeviceVendorName", out HTuple vendorTuple);
                    HOperatorSet.GetFramegrabberParam(grabber, "DeviceModelName", out HTuple modelTuple);
                    HOperatorSet.GetFramegrabberParam(grabber, "DeviceSerialNumber", out HTuple serialTuple);
                    HOperatorSet.GetFramegrabberParam(grabber, "ExposureTime", out HTuple exposureTuple);
                    HOperatorSet.GetFramegrabberParam(grabber, "Gain", out HTuple gainTuple);
                    HOperatorSet.GetFramegrabberParam(grabber, "AcquisitionFrameRate", out HTuple framerateTuple);
                    HOperatorSet.GetFramegrabberParam(grabber, "TriggerMode", out HTuple triggerModeTuple);

                    
                    vendor = vendorTuple.ToString();
                    model = modelTuple.ToString();
                    serial = serialTuple.ToString();
                    exposure = exposureTuple.ToString();
                    gain = gainTuple.ToString();
                    framerate = framerateTuple.ToString();
                    triggerMode = triggerModeTuple.ToString();

                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Could not query metadata for device {deviceId}");
                }

                var cameraInfo = new CameraInfo
                {
                    Id = deviceId,
                    Name = $"Camera {i + 1}: {deviceId}",
                    Vendor = vendor,
                    Model = model,
                    SerialNumber = serial,
                    ExposureTime = Convert.ToDouble(exposure),
                    Gain = Convert.ToDouble(gain),
                    FrameRate = Convert.ToDouble(framerate),
                    TriggerMode = triggerMode,

                };

                AvailableCameras.Add(cameraInfo);
                _logger.LogInformation($"Detected camera: {cameraInfo.Name} ({vendor}, {model}, {serial})");
            }

            if (AvailableCameras.Count > 0)
            {
                SelectedCamera = AvailableCameras[0];
                _logger.LogInformation($"Default selected camera: {SelectedCamera.Name}");
            }
            else
            {
                _logger.LogWarning("No cameras detected.");
            }
        }
        catch (PlatformNotSupportedException ex)
        {
            _logger.LogError(ex, "This functionality is only supported on Windows 7.0 and later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting available cameras");
        }
    }


    private string GetInfoValue(HTuple infoValues, string key, int index)
    {
        try
        {
            HOperatorSet.GetFramegrabberParam("USB3Vision", key, out HTuple result);
            return result.Length > index ? result[index].S : "N/A";
        }
        catch
        {
            return "N/A";
        }
    }

    private void ApplyCameraSettings()
    {
        if (SelectedCamera == null || _camera == null)
            return;

        try
        {
            if (_camera is HalconCamera halcon)
            {
                // Use combined method for framerate and exposure
                halcon.SetFramerateAndExposure(SelectedCamera.FrameRate, SelectedCamera.ExposureTime);

                // Set remaining individual parameters
                halcon.SetParameter("Gain", SelectedCamera.Gain);
                halcon.SetParameter("TriggerMode", SelectedCamera.TriggerMode);

                StatusMessage = "Camera settings updated.";
                Log($"Applied settings: Framerate={SelectedCamera.FrameRate}, Exposure={SelectedCamera.ExposureTime}, Gain={SelectedCamera.Gain}, Trigger={SelectedCamera.TriggerMode}");
            }
        }
        catch (Exception ex)
        {
            Log($"Error updating settings: {ex.Message}");
            StatusMessage = $"Error updating settings: {ex.Message}";
        }
    }




    private void StartPreview()
    {
        if (IsPreviewRunning || SelectedCamera is null)
            return;

        Log($"Starting preview for camera: {SelectedCamera.Name}");

        try
        {
            StopPreview(); // Ensure clean state

            _camera = new HalconCamera(SelectedCamera.Id);
            _camera.OnImageGrabbed += OnImageGrabbed;
            _camera.OnError += (_, message) =>
            {
                StatusMessage = $"Camera error: {message}";
                Log(message);
            };

            _camera.Start();
            IsPreviewRunning = true;
            StatusMessage = $"Preview started for {SelectedCamera.Name}";
            Log(StatusMessage);
            NotifyCommandStates();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to start preview: {ex.Message}";
            Log(StatusMessage);
        }
    }

    public void StopPreview()
    {
        if (!IsPreviewRunning)
            return;

        try
        {
            Log("Stopping preview...");
            _camera?.Close();
            _camera = null;
            IsPreviewRunning = false;
            StatusMessage = "Preview stopped.";
            LiveImage = null;

            NotifyCommandStates();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to stop preview: {ex.Message}";
            Log(StatusMessage);
        }
    }




    private void NotifyCommandStates()
    {
        (StartPreviewCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (StopPreviewCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }


    private void OnImageGrabbed(object? sender, HObject image)
    {
        try
        {
            if (image == null || !image.IsInitialized())
            {
                Log("Received uninitialized image.");
                return;
            }

            if (HalconWindow?.Handle == IntPtr.Zero)
            {
                Log("HalconWindow not ready, skipping image display.");
                return;
            }

            HTuple row = 0, column = 0;
            HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
            
            HalconWindow.SetPart(row, column, height - 1, width - 1);
            HalconWindow.ClearWindow();
            HalconWindow.DispObj(image);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error displaying image: {ex.Message}";
            Log(StatusMessage);
        }
    }




    private void Log(string message)
    {
        _logger.LogInformation(message); // Add this
        Debug.WriteLine(message);
        LogOutput += $"{DateTime.Now:HH:mm:ss} - {message}\n";
        OnPropertyChanged(nameof(LogOutput));
    }
}
