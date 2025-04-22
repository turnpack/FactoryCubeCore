using CommunityToolkit.Mvvm.ComponentModel;

namespace FactoryCube.Core.Models
{
    public partial class CameraInfo : ObservableObject
    {
        public string Id { get; set; }
        public string Name { get; set; }

        [ObservableProperty]
        private double exposureTime;

        [ObservableProperty]
        private double frameRate;

        [ObservableProperty]
        private double gain;

        [ObservableProperty]
        private string triggerMode;

        [ObservableProperty]
        private string vendor;

        [ObservableProperty]
        private string model;

        [ObservableProperty]
        private string serialNumber;
    }
}
