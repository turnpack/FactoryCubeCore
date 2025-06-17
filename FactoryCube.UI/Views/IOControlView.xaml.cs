using System.ComponentModel;

namespace FactoryCube.UI.ViewModels
{
    public class IOControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Example property for a digital input
        private bool _vacuumSensorActive;
        public bool VacuumSensorActive
        {
            get => _vacuumSensorActive;
            set
            {
                if (_vacuumSensorActive != value)
                {
                    _vacuumSensorActive = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VacuumSensorActive)));
                }
            }
        }

        // Constructor
        public IOControlViewModel()
        {
            // Simulate default state
            VacuumSensorActive = false;
        }
    }
}
