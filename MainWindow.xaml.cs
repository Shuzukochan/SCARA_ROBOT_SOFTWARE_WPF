using SCARA_ROBOT_SOFTWARE_WPF.Views;
using SCARA_ROBOT_SOFTWARE_WPF.ViewModels;
using System.Windows;
using System.Diagnostics;

namespace SCARA_ROBOT_SOFTWARE_WPF;

public partial class MainWindow : Window
{
    private readonly MainViewModel mainViewModel;
    private ConnectView connectView;
    private SetupProgramView? setupProgramView;


    public MainWindow()
    {
        InitializeComponent();
        mainViewModel = new MainViewModel();
        DataContext = mainViewModel;
        var connectViewModel = new ConnectViewModel(mainViewModel);
        connectView = new ConnectView(connectViewModel);
        MainContent.Content = connectView;
    }

    private void Connect_Click(object sender, RoutedEventArgs e)
    {
        if (connectView == null)
        {
            var connectViewModel = new ConnectViewModel(mainViewModel);
            connectView = new ConnectView(connectViewModel);
        }
        MainContent.Content = connectView;
    }
    private void SetupProgram_Click(object sender, RoutedEventArgs e)
    {
        if (!mainViewModel.IsUartConnected)
        {
            MessageBox.Show("UART is not connected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (setupProgramView == null)
        {
            var setupProgramVM = new SetupProgramViewModel();
            setupProgramView = new SetupProgramView(setupProgramVM);
        }
        MainContent.Content = setupProgramView;
    }

}
