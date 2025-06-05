using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace SCARA_ROBOT_SOFTWARE_WPF.ViewModels
{
    public class ConnectViewModel : INotifyPropertyChanged
    {
        private readonly UartManager _uartManager = UartManager.Instance;
        public ObservableCollection<string> Ports { get; }
        public ObservableCollection<string> Baudrates { get; }

        private string? _selectedPort;
        public string? SelectedPort
        {
            get => _selectedPort;
            set { _selectedPort = value; OnPropertyChanged(); }
        }

        private string? _selectedBaudrate;
        public string? SelectedBaudrate
        {
            get => _selectedBaudrate;
            set { _selectedBaudrate = value; OnPropertyChanged(); }
        }


        public ICommand RefreshCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }


        private readonly MainViewModel _mainVM;
        public ConnectViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            Baudrates = new ObservableCollection<string> { "9600", "19200", "38400", "57600", "115200", "256000" };
            SelectedBaudrate = "115200";
            Ports = new ObservableCollection<string>(SerialPort.GetPortNames());
            RefreshCommand = new RelayCommand(_ => RefreshPorts());
            ConnectCommand = new RelayCommand(_ => Connect(), _ => CanConnect());
            DisconnectCommand = new RelayCommand(_ => Disconnect(), _ => _uartManager.IsOpen);
        }

        private void RefreshPorts()
        {
            Ports.Clear();
            foreach (var port in SerialPort.GetPortNames())
                Ports.Add(port);
            SelectedPort = null;
            SelectedBaudrate = null;
        }
        public event Action<string>? NotifyMessage;

        private void Connect()
        {
            if (!string.IsNullOrEmpty(SelectedPort) && !string.IsNullOrEmpty(SelectedBaudrate))
            {
                _uartManager.Open(SelectedPort, int.Parse(SelectedBaudrate));
                _mainVM.StatusColor = Brushes.Green;
                _mainVM.IsUartConnected = true;
                _mainVM.ConnectionStatus = $"  Status: Connected";
                NotifyMessage?.Invoke($"Connected to {SelectedPort} at {SelectedBaudrate} baud.");
            }
        }
        private void Disconnect()
        {
            _uartManager.Close();
            _mainVM.StatusColor = Brushes.Red;
            _mainVM.IsUartConnected = false;
            _mainVM.ConnectionStatus = "  Status: Disconnected";
            NotifyMessage?.Invoke("Disconnected from device.");
        }


        private bool CanConnect()
        {
            return !string.IsNullOrEmpty(SelectedPort) && !string.IsNullOrEmpty(SelectedBaudrate);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

    }
}
