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

namespace Orator.AoL.MainWindow
{
	/// <summary>
	/// Interaction logic for NewMainWindow.xaml
	/// </summary>
	public partial class NewMainWindow : Window
	{
		public NewMainWindow()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NewMainWindowViewModel vm = new NewMainWindowViewModel();
            this.DataContext = vm;
        }
	}
}