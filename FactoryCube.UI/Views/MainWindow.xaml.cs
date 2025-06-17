// MainWindow.xaml.cs
using FactoryCube.UI.Enums;
using HalconDotNet;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;

namespace FactoryCube.UI.Views
{
    public partial class MainWindow : Window
    {
        private Point? _lastRightClick;
        private bool _isShiftHeld = false;

        public MainWindow()
        {
            InitializeComponent();

            // ViewModel creation and camera detection should happen BEFORE DataContext assignment
            var viewModel = App.ServiceProvider.GetRequiredService<CameraPreviewViewModel>();
            viewModel.DetectAvailableCameras();
            this.DataContext = viewModel;
        }

        private void HalconView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CameraPreviewViewModel vm)
            {
                vm.HalconWindow = HalconView.HalconWindow;

                if (!vm.IsPreviewRunning && vm.SelectedCamera != null)
                {
                    vm.StartPreviewCommand.Execute(null);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void HalconView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(DataContext is CameraPreviewViewModel vm)) return;
            GetImageCoordinates(e.GetPosition(HalconView), out var x, out var y);

            switch (vm.CurrentRoiShape)
            {
                case RoiShape.Rectangle:
                    vm.BeginRectangle(x, y);
                    break;
                case RoiShape.Ellipse:
                    vm.BeginEllipse(x, y);
                    break;
                case RoiShape.Polygon:
                    vm.AddPolygonPoint(x, y);
                    break;
            }

            e.Handled = true;
        }


        private void HalconView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(DataContext is CameraPreviewViewModel vm)) return;
            GetImageCoordinates(e.GetPosition(HalconView), out var x, out var y);

            switch (vm.CurrentRoiShape)
            {
                case RoiShape.Rectangle:
                    vm.CompleteRectangle(x, y, _isShiftHeld);
                    break;
                case RoiShape.Ellipse:
                    vm.CompleteEllipse(x, y, _isShiftHeld);
                    break;
                case RoiShape.Polygon:
                    // finishing polygons is on right?click
                    break;
            }

            e.Handled = true;
        }




        private void HalconView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is CameraPreviewViewModel vm && vm.HalconWindow is { } window)
            {
                // Get current window part
                window.GetPart(out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);

                // Convert to double for arithmetic
                double r1 = row1.D, r2 = row2.D, c1 = col1.D, c2 = col2.D;
                double height = r2 - r1;
                double width = c2 - c1;

                // Get zoom direction
                double scaleFactor = e.Delta > 0 ? 0.9 : 1.1;

                // Get mouse position
                Point mousePos = e.GetPosition(HalconView);
                double mouseX = mousePos.X / HalconView.ActualWidth;
                double mouseY = mousePos.Y / HalconView.ActualHeight;

                // Compute new center based on mouse
                double centerRow = r1 + height * mouseY;
                double centerCol = c1 + width * mouseX;

                // Apply zoom
                double newHeight = height * scaleFactor;
                double newWidth = width * scaleFactor;

                double newRow1 = centerRow - newHeight * mouseY;
                double newRow2 = newRow1 + newHeight;
                double newCol1 = centerCol - newWidth * mouseX;
                double newCol2 = newCol1 + newWidth;

                window.SetPart(newRow1, newCol1, newRow2, newCol2);
                vm.SetZoomPart(newRow1, newCol1, newRow2, newCol2);

                vm.RedrawOverlays(); // Optional: refresh overlays to match zoom
            }
        }

        private void HalconView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(DataContext is CameraPreviewViewModel vm)) return;

            if (vm.CurrentRoiShape == RoiShape.Polygon)
            {
                vm.FinishPolygon();
                vm.RedrawOverlays();
                e.Handled = true;
                return;
            }
            // otherwise, begin panning…
            _lastRightClick = e.GetPosition(HalconView);
            HalconView.CaptureMouse();
        }

        private void HalconView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _lastRightClick = null;
            HalconView.ReleaseMouseCapture();
        }

        private void HalconView_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(DataContext is CameraPreviewViewModel vm)) return;

            // 1) Left?button drag ? live preview for Rect / Ellipse / Polygon
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                GetImageCoordinates(e.GetPosition(HalconView), out var x, out var y);

                switch (vm.CurrentRoiShape)
                {
                    case RoiShape.Rectangle:
                        vm.UpdateRectanglePreview(x, y, _isShiftHeld);
                        e.Handled = true;
                        return;

                    case RoiShape.Ellipse:
                        vm.UpdatePreviewEllipse(x, y, _isShiftHeld);
                        e.Handled = true;
                        return;

                    case RoiShape.Polygon:
                        vm.UpdatePolygonPreview(x, y);
                        e.Handled = true;
                        return;
                }
            }

            // 2) Right-button drag ? pan
            if (_lastRightClick.HasValue && e.RightButton == MouseButtonState.Pressed)
            {
                var window = vm.HalconWindow!;
                Point current = e.GetPosition(HalconView);
                Vector delta = current - _lastRightClick.Value;

                window.GetPart(out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                double scaleX = (c2.D - c1.D + 1) / HalconView.ActualWidth;
                double scaleY = (r2.D - r1.D + 1) / HalconView.ActualHeight;

                double dCol = -delta.X * scaleX;
                double dRow = -delta.Y * scaleY;

                window.SetPart(r1.D + dRow, c1.D + dCol, r2.D + dRow, c2.D + dCol);
                vm.SetZoomPart(r1.D + dRow, c1.D + dCol, r2.D + dRow, c2.D + dCol);

                _lastRightClick = current;
                e.Handled = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                _isShiftHeld = true;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                _isShiftHeld = false;
        }

        private void GetImageCoordinates(Point mouse, out double imageX, out double imageY)
        {
            if (DataContext is CameraPreviewViewModel vm && vm.HalconWindow is { } window)
            {
                window.GetPart(out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);

                double scaleX = (c2.D - c1.D + 1) / HalconView.ActualWidth;
                double scaleY = (r2.D - r1.D + 1) / HalconView.ActualHeight;

                imageX = c1.D + mouse.X * scaleX;
                imageY = r1.D + mouse.Y * scaleY;
            }
            else
            {
                imageX = 0;
                imageY = 0;
            }
        }
    }
}
