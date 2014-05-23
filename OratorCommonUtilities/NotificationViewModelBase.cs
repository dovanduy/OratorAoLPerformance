using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace OratorCommonUtilities
{
    public class NotificationViewModelBase : ViewModelBase
    {
        public string NotificationText { get; set; }
        public event EventHandler RequestDeleteEvent;

        public NotificationViewModelBase()
        {
        }

        private RelayCommand _requestDeleteCommand;
        public ICommand RequestDeleteCommand
        {
            get {  return _requestDeleteCommand ?? (_requestDeleteCommand = new RelayCommand(param => this.RequestDeleteCommandExecuted())); }
        }

        private void RequestDeleteCommandExecuted()
        {
            if (RequestDeleteEvent != null) this.RequestDeleteEvent(this, new EventArgs());
        }
    }
}
