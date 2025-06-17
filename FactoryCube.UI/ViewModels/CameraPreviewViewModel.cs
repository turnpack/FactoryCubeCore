using System.Collections.ObjectModel;
using FactoryCube.Interfaces;
using System.Windows.Media.Imaging;
using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using FactoryCube.UI.ViewModels;
using System.Collections.Generic;
using HalconDotNet;
using FactoryCube.Core.Models;
using FactoryCube.Vision.Camera;
using System.ComponentModel;
using System.Threading.Tasks;
using FactoryCube.UI.Graphics;
using FactoryCube.UI.Enums;

public class CameraPreviewViewModel : ViewModelBase
{
    private readonly ILogger<CameraPreviewViewModel> _logger;
    private ICameraService? _camera;
    private OverlayManager? _overlayManager;
    private readonly List<(double X, double Y)> _roiCenters = new();
    private HObject? _lastImage;
    private readonly Func<string, ICameraService> _cameraFactory;
    public HWindow? HalconWindow { get; set; }
    private double? _zoomRow1, _zoomCol1, _zoomRow2, _zoomCol2;
    private int _lastImageWidth = 0;
    private int _lastImageHeight = 0;

    private bool _isDrawingRectangle = false;
    private (double X, double Y)? _rectStart = null;


    public IRelayCommand RefreshCamerasCommand { get; }
    public IRelayCommand ResetZoomCommand { get; }

    private RoiShape _currentRoiShape = RoiShape.Rectangle;
    public RoiShape CurrentRoiShape
    {
        get => _currentRoiShape;
        set => SetProperty(ref _currentRoiShape, value);
    }


    public void SetRoiShape(RoiShape shape)
    {
        CurrentRoiShape = shape;
        Log($"ROI shape set to: {shape}");
    }

    public void AddCenteredRectangle(double x, double y) =>
    _overlayManager?.AddCenteredRectangle(x, y);

    public void AddCenteredEllipse(double x, double y) =>
        _overlayManager?.AddCenteredEllipse(x, y);

    public void AddPolygonPoint(double x, double y) =>
        _overlayManager?.AddPolygonPoint(x, y); // You’ll build up points here



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
                    StopPreviewAsync();
                    StartPreviewAsync();
                }
            }
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

    public IAsyncRelayCommand StartPreviewCommand { get; }
    public IAsyncRelayCommand StopPreviewCommand { get; }
    public IAsyncRelayCommand ApplySettingsCommand { get; }

    public IRelayCommand ApplyCameraSettingsCommand { get; }
    public IRelayCommand ToggleCrosshairCommand { get; }
    public IRelayCommand ToggleGridCommand { get; }


    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }


    public CameraPreviewViewModel(Func<string, ICameraService> cameraFactory, ILogger<CameraPreviewViewModel> logger)
    {
        _cameraFactory = cameraFactory;
        _logger = logger;
        StartPreviewCommand = new AsyncRelayCommand(
            StartPreviewAsync,
            () => !IsPreviewRunning);

        StopPreviewCommand = new AsyncRelayCommand(
            StopPreviewAsync,
            () => IsPreviewRunning);

        ApplySettingsCommand = new AsyncRelayCommand(
            ApplyCameraSettingsAsync,
            () => SelectedCamera != null && _camera != null);

        //StartPreviewCommand = new RelayCommand(StartPreview, () => !IsPreviewRunning);

        //StopPreviewCommand = new RelayCommand(StopPreview, () => IsPreviewRunning);
        RefreshCamerasCommand = new RelayCommand(DetectAvailableCameras);
        ApplyCameraSettingsCommand = new RelayCommand(() => ApplyCameraSettingsAsync().ConfigureAwait(false));
        ResetZoomCommand = new RelayCommand(ResetZoom);
        ToggleCrosshairCommand = new RelayCommand(() =>
        {
            if (_overlayManager != null)
            {
                _overlayManager.ShowCrosshair = !_overlayManager.ShowCrosshair;
                RedrawOverlays();
            }
        });

        ToggleGridCommand = new RelayCommand(() =>
        {
            if (_overlayManager != null)
            {
                _overlayManager.ShowGrid = !_overlayManager.ShowGrid;
                RedrawOverlays();
            }
        });
        DeleteRoiCommand = new RelayCommand<RoiItem>(DeleteRoi);
        MoveUpRoiCommand = new RelayCommand<RoiItem>(MoveUpRoi);
        MoveDownRoiCommand = new RelayCommand<RoiItem>(MoveDownRoi);


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

    //private async Task ApplyCameraSettingsAsync()
    //{
    //    if (SelectedCamera == null || _camera == null)
    //        return;

    //    try
    //    {
    //        // combined setting
    //        await _camera.SetFrameRateAndExposureAsync(
    //            SelectedCamera.FrameRate,
    //            SelectedCamera.ExposureTime);

    //        // then the rest
    //        await _camera.SetParameterAsync("Gain", SelectedCamera.Gain);
    //        await _camera.SetParameterAsync("TriggerMode", SelectedCamera.TriggerMode!);

    //        StatusMessage = "Camera settings updated.";
    //    }
    //    catch (Exception ex)
    //    {
    //        StatusMessage = $"Error updating settings: {ex.Message}";
    //    }
    //}





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

    private async Task ApplyCameraSettingsAsync()
    {
        if (SelectedCamera == null || _camera == null)
            return;

        try
        {
            if (_camera is HalconCameraService halcon)
            {
                // Use combined method for framerate and exposure
                await halcon.SetFrameRateAndExposureAsync(SelectedCamera.FrameRate, SelectedCamera.ExposureTime);

                // Set remaining individual parameters
                await halcon.SetParameterAsync("Gain", SelectedCamera.Gain);
                await halcon.SetParameterAsync("TriggerMode", SelectedCamera.TriggerMode);

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


    private async Task StartPreviewAsync()
    {
        if (IsPreviewRunning || SelectedCamera is null)
            return;

        Log($"Starting preview for camera: {SelectedCamera.Name}");

        try
        {
            // Ensure any prior preview is fully stopped
            await StopPreviewAsync();

            // Create the camera via your factory (so DI works)
            _camera = _cameraFactory(SelectedCamera.Id);
            _camera.OnImageGrabbed += OnImageGrabbed;
            //_camera.OnImageGrabbed += new EventHandler<HObject>(OnImageGrabbed);

            _camera.OnError += (_, msg) =>
            {
                StatusMessage = $"Camera error: {msg}";
                Log(msg);
            };

            // Initialize & start acquisition asynchronously
            await _camera.InitializeAsync();
            await _camera.StartAcquisitionAsync();

            IsPreviewRunning = true;
            StatusMessage = $"Preview started for {SelectedCamera.Name}";
            Log(StatusMessage);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to start preview: {ex.Message}";
            Log(StatusMessage);
        }
        finally
        {
            NotifyPreviewCommandStates();
        }
    }



    //private void StartPreview()
    //{
    //    if (IsPreviewRunning || SelectedCamera is null)
    //        return;

    //    Log($"Starting preview for camera: {SelectedCamera.Name}");

    //    try
    //    {
    //        StopPreview(); // Ensure clean state

    //        _camera = new HalconCameraService(SelectedCamera.Id);
    //        _camera.OnImageGrabbed += OnImageGrabbed;
    //        _camera.OnError += (_, message) =>
    //        {
    //            StatusMessage = $"Camera error: {message}";
    //            Log(message);
    //        };

    //        _camera.Start();
    //        IsPreviewRunning = true;
    //        StatusMessage = $"Preview started for {SelectedCamera.Name}";
    //        Log(StatusMessage);
    //        NotifyCommandStates();
    //    }
    //    catch (Exception ex)
    //    {
    //        StatusMessage = $"Failed to start preview: {ex.Message}";
    //        Log(StatusMessage);
    //    }
    //}


    private async Task StopPreviewAsync()
    {
        if (!IsPreviewRunning)
            return;

        Log("Stopping preview...");

        try
        {
            if (_camera != null)
            {
                // Gracefully close acquisition
                await _camera.CloseAsync();
                _camera = null;
            }

            IsPreviewRunning = false;
            StatusMessage = "Preview stopped.";
            LiveImage = null;

            // dispose last HObject if any
            _lastImage?.Dispose();
            _lastImage = null;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to stop preview: {ex.Message}";
            Log(StatusMessage);
        }
        finally
        {
            NotifyPreviewCommandStates();
        }
    }


    //public void StopPreview()
    //{
    //    if (!IsPreviewRunning)
    //        return;

    //    try
    //    {
    //        Log("Stopping preview...");
    //        _camera?.Close();
    //        _camera = null;
    //        IsPreviewRunning = false;
    //        StatusMessage = "Preview stopped.";
    //        LiveImage = null;
    //        _lastImage?.Dispose();
    //        _lastImage = null;

    //        NotifyCommandStates();
    //    }
    //    catch (Exception ex)
    //    {
    //        StatusMessage = $"Failed to stop preview: {ex.Message}";
    //        Log(StatusMessage);
    //    }
    //}

    private void NotifyPreviewCommandStates()
    {
        StartPreviewCommand.NotifyCanExecuteChanged();
        StopPreviewCommand.NotifyCanExecuteChanged();
    }


    //private void NotifyCommandStates()
    //{
    //    (StartPreviewCommand as RelayCommand)?.NotifyCanExecuteChanged();
    //    (StopPreviewCommand as RelayCommand)?.NotifyCanExecuteChanged();
    //}


    private void OnImageGrabbed(object sender, HObject image)
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

            _lastImageWidth = width.I;
            _lastImageHeight = height.I;
            if (_zoomRow1.HasValue)
            {
                HalconWindow.SetPart(_zoomRow1.Value, _zoomCol1.Value, _zoomRow2.Value, _zoomCol2.Value);
            }
            else
            {
                HalconWindow.SetPart(row, column, height - 1, width - 1);
            }

            _lastImage?.Dispose();
            _lastImage = image.CopyObj(1, -1); // force copy entire object



            HalconWindow.ClearWindow();
            HalconWindow.DispObj(image);

            // Initialize overlay manager if needed
            if (_overlayManager == null)
                _overlayManager = new OverlayManager(HalconWindow, width.I, height.I);
            else
                _overlayManager.UpdateImageSize(width.I, height.I);

            _overlayManager.DrawOverlays(PlacedRois);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error displaying image: {ex.Message}";
            Log(StatusMessage);
        }
    }

    public void DrawSnapCenteredRoi(double x, double y)
    {
        _overlayManager?.AddCenteredRoi(x, y);
        _overlayManager?.DrawOverlays(PlacedRois);
    }

    public void ClearRois()
    {
        _overlayManager?.ClearAllRois(); // You'll need to implement this
        _overlayManager?.DrawOverlays(PlacedRois);
    }

    private (double X, double Y)? _previewPolygonPoint;

    // Called on MouseMove in Polygon mode
    public void UpdatePolygonPreview(double x, double y)
    {
        _previewPolygonPoint = (x, y);
        RedrawOverlays();
    }

    // Called on right?click to finish the poly
    public void FinishPolygon()
    {
        if (_previewPolygonPoint.HasValue)
            AddPolygonPoint(_previewPolygonPoint.Value.X, _previewPolygonPoint.Value.Y);
        _previewPolygonPoint = null;
        RedrawOverlays();
    }

    public void RedrawOverlays()
    {
        try
        {
            if (_lastImage != null && HalconWindow != null)
            {
                // 1) Clear out the old lines
                HalconWindow.ClearWindow();

                // 2) Re-paint the last image
                HalconWindow.DispObj(_lastImage);
            }

            // 3) Draw only the overlays you currently want
            _overlayManager?.DrawOverlays(PlacedRois);
        }
        catch (Exception ex)
        {
            Log($"Overlay redraw error: {ex.Message}");
        }
    }

    public void SetZoomPart(double row1, double col1, double row2, double col2)
    {
        _zoomRow1 = row1;
        _zoomCol1 = col1;
        _zoomRow2 = row2;
        _zoomCol2 = col2;
    }

    public void ResetZoom()
    {
        _zoomRow1 = _zoomCol1 = _zoomRow2 = _zoomCol2 = null;

        if (_lastImage != null && HalconWindow != null && _lastImageWidth > 0 && _lastImageHeight > 0)
        {
            try
            {
                HalconWindow.SetPart(0, 0, _lastImageHeight - 1, _lastImageWidth - 1);
                Log($"Resetting zoom to: (0, 0, {_lastImageHeight - 1}, {_lastImageWidth - 1})");

                HalconWindow.ClearWindow();
                HalconWindow.DispObj(_lastImage);
                _overlayManager?.UpdateImageSize(_lastImageWidth, _lastImageHeight);
                _overlayManager?.DrawOverlays(PlacedRois);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Reset Zoom failed: {ex.Message}";
                Log(StatusMessage);
            }
        }
    }

    public ObservableCollection<RoiItem> PlacedRois { get; } = new();

    private RoiItem _selectedRoi;
    public RoiItem SelectedRoi
    {
        get => _selectedRoi;
        set => SetProperty(ref _selectedRoi, value);
    }

    public IRelayCommand<RoiItem> DeleteRoiCommand { get; }
    public IRelayCommand<RoiItem> MoveUpRoiCommand { get; }
    public IRelayCommand<RoiItem> MoveDownRoiCommand { get; }


    private void DeleteRoi(RoiItem roi)
    {
        PlacedRois.Remove(roi);
        RedrawOverlays();
    }

    private void MoveUpRoi(RoiItem roi)
    {
        var i = PlacedRois.IndexOf(roi);
        if (i > 0) { PlacedRois.Move(i, i - 1); RedrawOverlays(); }
    }

    private void MoveDownRoi(RoiItem roi)
    {
        var i = PlacedRois.IndexOf(roi);
        if (i < PlacedRois.Count - 1) { PlacedRois.Move(i, i + 1); RedrawOverlays(); }
    }


    private (double X, double Y)? _rectangleStart = null;

    public void BeginRectangle(double x, double y)
    {
        _rectangleStart = (x, y);
    }

    public void UpdateRectanglePreview(double x, double y, bool snapToSquare)
    {
        if (_rectangleStart is (double x0, double y0))
            _overlayManager?.SetPreviewRectangle(x0, y0, x, y, snapToSquare);
    }

    public void CompleteRectangle(double x, double y, bool snapToSquare)
    {
        if (_rectangleStart is (double x0, double y0))
        {
            if (snapToSquare)
            {
                double side = Math.Min(Math.Abs(x - x0), Math.Abs(y - y0));
                x = x0 + Math.Sign(x - x0) * side;
                y = y0 + Math.Sign(y - y0) * side;
            }

            double centerX = (x0 + x) / 2;
            double centerY = (y0 + y) / 2;
            double width = Math.Abs(x - x0);
            double height = Math.Abs(y - y0);

            _overlayManager?.AddCenteredRectangle(centerX, centerY, width, height);
            _overlayManager?.ClearPreview();
            _rectangleStart = null;
        }
    }

    private bool _isDrawingEllipse = false;
    private (double X, double Y)? _ellipseStart;

    public void BeginEllipse(double x, double y)
    {
        _isDrawingEllipse = true;
        _ellipseStart = (x, y);
    }

    public void UpdatePreviewEllipse(double x, double y, bool snapCircle)
    {
        if (!_isDrawingEllipse || _ellipseStart == null) return;
        var (x1, y1) = _ellipseStart.Value;
        _overlayManager?.SetPreviewEllipse(x1, y1, x, y, snapCircle);
        RedrawOverlays();
    }

    public void CompleteEllipse(double x, double y, bool snapCircle)
    {
        if (!_isDrawingEllipse || _ellipseStart == null) return;
        var (x1, y1) = _ellipseStart.Value;
        _isDrawingEllipse = false;
        _ellipseStart = null;
        _overlayManager?.ClearPreviewEllipse();
        _overlayManager?.AddEllipseFromCorners(x1, y1, x, y, snapCircle);
        RedrawOverlays();
    }



    private void Log(string message)
    {
        _logger.LogInformation(message); // Add this
        Debug.WriteLine(message);
        LogOutput += $"{DateTime.Now:HH:mm:ss} - {message}\n";
        OnPropertyChanged(nameof(LogOutput));
    }

    private async void OnSelectedCameraChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!IsPreviewRunning || _camera == null || SelectedCamera == null)
            return;

        try
        {
            switch (e.PropertyName)
            {
                case nameof(CameraInfo.FrameRate):
                case nameof(CameraInfo.ExposureTime):
                    await _camera.SetFrameRateAndExposureAsync(
                        SelectedCamera.FrameRate,
                        SelectedCamera.ExposureTime);
                    break;

                case nameof(CameraInfo.Gain):
                    await _camera.SetParameterAsync("Gain", SelectedCamera.Gain);
                    break;

                case nameof(CameraInfo.TriggerMode):
                    await _camera.SetParameterAsync("TriggerMode", SelectedCamera.TriggerMode!);
                    break;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to apply {e.PropertyName}: {ex.Message}";
            _logger.LogError(ex, "Parameter update error");
        }
    }
}
