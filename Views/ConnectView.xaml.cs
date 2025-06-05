using SCARA_ROBOT_SOFTWARE_WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SCARA_ROBOT_SOFTWARE_WPF.Views
{
    public partial class ConnectView : UserControl
    {
        public ConnectView(ConnectViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.NotifyMessage += msg => MessageBox.Show(msg, "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
