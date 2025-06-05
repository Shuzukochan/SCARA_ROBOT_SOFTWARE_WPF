using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SCARA_ROBOT_SOFTWARE_WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isUartConnected;
        public bool IsUartConnected
        {
            get => _isUartConnected;
            set { _isUartConnected = value; OnPropertyChanged(); }
        }
        private string _connectionStatus = "  Status: Disconnected";
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set { _connectionStatus = value; OnPropertyChanged(); }
        }

        private Brush _statusColor = Brushes.Red;
        public Brush StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

}
