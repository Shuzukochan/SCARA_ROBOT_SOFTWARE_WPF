using SCARA_ROBOT_SOFTWARE_WPF.Views;
using System.IO.Ports;
using System.Text;

public class UartManager
{
    private static readonly UartManager _instance = new UartManager();
    public static UartManager Instance => _instance;

    private SerialPort? _serialPort;
    public bool IsOpen => _serialPort != null && _serialPort.IsOpen;

    public event Action<string>? DataReceived;

    public void Open(string portName, int baudrate)
    {
        if (_serialPort != null && _serialPort.IsOpen) return;

        _serialPort = new SerialPort
        {
            PortName = portName,
            BaudRate = baudrate,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
            Handshake = Handshake.None,
            Encoding = Encoding.ASCII
        };

        _serialPort.DataReceived += (s, e) =>
        {
            string data = _serialPort.ReadExisting();
            DataReceived?.Invoke(data); 
        };

        _serialPort.Open();
    }

    public void Close()
    {
        if (_serialPort != null && _serialPort.IsOpen)
            _serialPort.Close();
    }

    public void SendCommand(string command)
    {
        if (_serialPort != null && _serialPort.IsOpen && !string.IsNullOrEmpty(command))
        {
            _serialPort.Write(command);
        }
    }
}
