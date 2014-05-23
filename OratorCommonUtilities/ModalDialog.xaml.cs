using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;

namespace OratorCommonUtilities
{
	/// <summary>
	/// Interaction logic for ModalDialog.xaml
	/// </summary>
	public partial class ModalDialog : UserControl
	{
		public ModalDialog()
		{
			InitializeComponent();
			Visibility = Visibility.Hidden;
		}
		private bool _hideRequest = false;
        private ModelDialogResult _dialogResult = ModelDialogResult.Cancel;

        #region Owner
        public UIElement Owner
        {
            get { return (UIElement)GetValue(OwnerProperty); }
            set {SetValue(OwnerProperty,value);}
        }
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(UIElement), typeof(ModalDialog), new UIPropertyMetadata(null));
        #endregion

		#region Message

		public string Message
		{
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Message.
		// This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register(
				"Message", typeof(string), typeof(ModalDialog), new UIPropertyMetadata(string.Empty));

		#endregion

		public ModelDialogResult ShowHandlerDialog(string message)
		{
			Message = message;
			Visibility = Visibility.Visible;
            if (Owner != null)
            { Owner.IsEnabled = false; }
			_hideRequest = false;
            while (!_hideRequest)
            {
                // HACK: Stop the thread if the application is about to close
                if (this.Dispatcher.HasShutdownStarted ||
                    this.Dispatcher.HasShutdownFinished)
                {
                    break;
                }

                // HACK: Simulate "DoEvents"
                this.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new ThreadStart(delegate { }));
                Thread.Sleep(20);
            }
            return _dialogResult;
		}
		
		private void HideHandlerDialog()
		{
			_hideRequest = true;
			Visibility = Visibility.Hidden;
            if (Owner != null)
            { Owner.IsEnabled = true; }
		}

        private void ExportButton_Click(object sender, RoutedEventArgs e)
		{
            _dialogResult = ModelDialogResult.Export;
			HideHandlerDialog();
		}

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            _dialogResult = ModelDialogResult.Copy;
            HideHandlerDialog();
        }

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
            _dialogResult = ModelDialogResult.Cancel;
            HideHandlerDialog();
		}
	}

    public enum ModelDialogResult
    {
        Cancel,
        Copy,
        Export
    }
}
