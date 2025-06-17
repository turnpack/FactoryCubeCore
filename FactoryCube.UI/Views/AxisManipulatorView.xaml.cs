using System.ComponentModel;

namespace FactoryCube.UI.ViewModels
{
    public class AxisManipulatorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _axisXPosition;
        public double AxisXPosition
        {
            get => _axisXPosition;
            set
            {
                if (_axisXPosition != value)
                {
                    _axisXPosition = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AxisXPosition)));
                }
            }
        }

        private double _jogVelocity;
        public double JogVelocity
        {
            get => _jogVelocity;
            set
            {
                if (_jogVelocity != value)
                {
                    _jogVelocity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JogVelocity)));
                }
            }
        }

        // Constructor
        public AxisManipulatorViewModel()
        {
            AxisXPosition = 0.0;
            JogVelocity = 10.0;
        }
    }
}
