using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SCARA_ROBOT_SOFTWARE_WPF.Models;
using SCARA_ROBOT_SOFTWARE_WPF;
using System.Threading;
using System.Threading.Tasks;

namespace SCARA_ROBOT_SOFTWARE_WPF.ViewModels
{
    public class SetupProgramViewModel : INotifyPropertyChanged
    {
        private CancellationTokenSource? _holdCts;

        public ObservableCollection<RobotPoint> Points { get; } = new()
        {
            new RobotPoint(), 
            new RobotPoint(), 
            new RobotPoint(), 
            new RobotPoint()  
        };

        private int _selectedCornerIndex;
        public int SelectedCornerIndex
        {
            get => _selectedCornerIndex;
            set
            {
                if (_selectedCornerIndex != value)
                {
                    if (_selectedCornerIndex >= 0 && _selectedCornerIndex < Points.Count)
                        Points[_selectedCornerIndex].PropertyChanged -= Point_PropertyChanged;

                    _selectedCornerIndex = value;
                    OnPropertyChanged();
                    Points[_selectedCornerIndex].PropertyChanged += Point_PropertyChanged;
                    OnPropertyChanged(nameof(SelectedPoint));
                    UpdateAndSendKinematics();
                }
            }
        }

        public RobotPoint SelectedPoint => Points[SelectedCornerIndex];

        private void Point_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateAndSendKinematics();
        }
        public ICommand PxLeftCommand { get; }
        public ICommand PxRightCommand { get; }
        public ICommand PyUpCommand { get; }
        public ICommand PyDownCommand { get; }
        public ICommand PzUpCommand { get; }
        public ICommand PzDownCommand { get; }
        public ICommand YawLeftCommand { get; }
        public ICommand YawRightCommand { get; }
        public ICommand SelectCornerCommand { get; }
        public ICommand HomeCommand { get; }
        public ICommand SetPointCommand { get; }

        public SetupProgramViewModel()
        {
            PxLeftCommand = new RelayCommand(_ => SelectedPoint.Px -= GetStepSize());
            PxRightCommand = new RelayCommand(_ => SelectedPoint.Px += GetStepSize());
            PyUpCommand = new RelayCommand(_ => SelectedPoint.Py += GetStepSize());
            PyDownCommand = new RelayCommand(_ => SelectedPoint.Py -= GetStepSize());
            PzUpCommand = new RelayCommand(_ => SelectedPoint.Pz += GetStepSize());
            PzDownCommand = new RelayCommand(_ => SelectedPoint.Pz -= GetStepSize());
            YawLeftCommand = new RelayCommand(_ => SelectedPoint.Yaw -= GetStepSize());
            YawRightCommand = new RelayCommand(_ => SelectedPoint.Yaw += GetStepSize());
            HomeCommand = new RelayCommand(_ => GoHome());
            SetPointCommand = new RelayCommand(_ => SendSetPoint());

            SelectCornerCommand = new RelayCommand(param =>
            {
                if (int.TryParse(param?.ToString(), out int idx))
                    SelectedCornerIndex = idx;
            });

            _selectedCornerIndex = 0;
            Points[0].PropertyChanged += Point_PropertyChanged;
        }
        private void GoHome()
        {
            UartManager.Instance.SendCommand("Home\r\n");
        }
        private void SendSetPoint()
        {
            string[] labels = { "A", "B", "C", "D" };
            string label = "A"; 
            if (SelectedCornerIndex >= 0 && SelectedCornerIndex < labels.Length)
                label = labels[SelectedCornerIndex];

            string cmd = $"Set{label}\r\n";
            UartManager.Instance.SendCommand(cmd);
        }
        private double GetStepSize() => 1; 

        public void StartHold(string direction)
        {
            StopHold();
            _holdCts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                while (!_holdCts.Token.IsCancellationRequested)
                {
                    switch (direction)
                    {
                        case "PxLeft": SelectedPoint.Px -= GetStepSize(); break;
                        case "PxRight": SelectedPoint.Px += GetStepSize(); break;
                        case "PyUp": SelectedPoint.Py += GetStepSize(); break;
                        case "PyDown": SelectedPoint.Py -= GetStepSize(); break;
                        case "PzUp": SelectedPoint.Pz += GetStepSize(); break;
                        case "PzDown": SelectedPoint.Pz -= GetStepSize(); break;
                        case "YawLeft": SelectedPoint.Yaw -= GetStepSize(); break;
                        case "YawRight": SelectedPoint.Yaw += GetStepSize(); break;
                    }
                    await Task.Delay(200);
                }
            }, _holdCts.Token);
        }
        public void StopHold() => _holdCts?.Cancel();

        private void UpdateAndSendKinematics()
        {
            var pt = SelectedPoint;
            var kinematics = new KinematicsCaculator();
            kinematics.Inverse(pt.Px, pt.Py, pt.Pz, pt.Yaw);

            string cmd = $"Q1:{kinematics.Q1:F2}|D2:{kinematics.D2:F2}|Q3:{kinematics.Q3:F2}|D3:{kinematics.D3:F2}\r\n";
            UartManager.Instance.SendCommand(cmd);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
