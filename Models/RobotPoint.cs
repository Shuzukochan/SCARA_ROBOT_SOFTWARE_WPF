using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SCARA_ROBOT_SOFTWARE_WPF.Models
{
    public class RobotPoint : INotifyPropertyChanged
    {
        private double _px, _py, _pz, _yaw;
        public double Px { get => _px; set { _px = value; OnPropertyChanged(); } }
        public double Py { get => _py; set { _py = value; OnPropertyChanged(); } }
        public double Pz { get => _pz; set { _pz = value; OnPropertyChanged(); } }
        public double Yaw { get => _yaw; set { _yaw = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
