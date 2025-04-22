// MainWindow.xaml.cs
using FactoryCube.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FactoryCube.UI
{
    public partial class MainWindow : Window
    {
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
    }
}
