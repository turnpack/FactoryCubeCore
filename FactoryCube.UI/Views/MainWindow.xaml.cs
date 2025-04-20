using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using FactoryCube.UI.ViewModels;

namespace FactoryCube.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = App.ServiceProvider.GetRequiredService<CameraPreviewViewModel>();
            this.DataContext = viewModel;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
