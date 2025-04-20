using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
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

public class CameraPreviewViewModel : ViewModelBase
{
    private readonly ILogger<CameraPreviewViewModel> _logger;
    private readonly ICamera _camera;
    public IEnumerable<ICamera> CameraList => AvailableCameras;


    public ObservableCollection<ICamera> AvailableCameras { get; } = new();
    private ICamera? _selectedCamera;
    public ICamera? SelectedCamera
    {
        get => _selectedCamera;
        set
        {
            if (SetProperty(ref _selectedCamera, value))
            {
                StatusMessage = $"Selected camera: {_selectedCamera?.ToString() ?? "None"}";
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

    public IRelayCommand StartPreviewCommand { get; }
    public IRelayCommand StopPreviewCommand { get; }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }


    public CameraPreviewViewModel(ICamera camera, ILogger<CameraPreviewViewModel> logger)
    {
        _camera = camera;

        _camera.OnImageGrabbed += OnImageGrabbed;
        _logger = logger;

        StartPreviewCommand = new RelayCommand(StartPreview, () => !IsPreviewRunning);
        StopPreviewCommand = new RelayCommand(StopPreview, () => IsPreviewRunning);

        AvailableCameras.Add(_camera); // For now, single camera. Add real scan later.
        SelectedCamera = _camera;

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

    private void StartPreview()
    {
        try
        {
            _camera.SetAcquisitionMode("continuous");
            _camera.Start();
            IsPreviewRunning = true;
            Log("Preview started.");
        }
        catch (Exception ex)
        {
            Log($"Error starting preview: {ex.Message}");
        }
    }

    private void StopPreview()
    {
        try
        {
            _camera.Stop();
            IsPreviewRunning = false;
            Log("Preview stopped.");            
        }
        catch (Exception ex)
        {
            Log($"Error stopping preview: {ex.Message}");
        }
    }

    private void OnImageGrabbed(object? sender, HObject image)
    {
        try
        {
            if (image == null || !image.IsInitialized())
                return;

            var bitmap = HalconImageConverter.ConvertToBitmapImage(image);

            App.Current.Dispatcher.Invoke(() =>
            {
                LiveImage = bitmap;
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error converting image: {ex.Message}";
        }
    }


    private void Log(string message)
    {
        Debug.WriteLine(message);
        LogOutput += $"{DateTime.Now:HH:mm:ss} - {message}\n";
        OnPropertyChanged(nameof(LogOutput));
    }
}
