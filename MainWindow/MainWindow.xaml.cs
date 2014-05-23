using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Profiler.Analyzer.Performance;
using Profiler.DeviceMonitor;
using System.Threading.Tasks;

namespace Orator.AoL.MainWindow
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
    public partial class MainWindow : Window
	{
        private MainWindowViewModel _mainVM;

        public MainWindow()
		{
			this.InitializeComponent();
			// Insert code required on object creation below this point.
            _mainVM = new MainWindowViewModel();
            this.DataContext = _mainVM;
		}

		private void LayoutRoot_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                MainDockPanel.Children.Add(_mainVM.MainDockPanelVM.View);
                _mainVM.Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
		}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (_mainVM.ConnectedState == Models.DeviceConncectState.CanDisconnect)
                {
                    MessageBoxResult mbr = MessageBox.Show("logger is running , would you want to quit directly?", "propt", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);
                    if (mbr == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        
                        _mainVM.StopLogger();
                    }
                }

                if (_mainVM != null && _mainVM.StopMonitorDevice != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(_mainVM.StopMonitorDevice);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
         
        }
	}
}