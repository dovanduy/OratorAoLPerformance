using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OratorCommonUtilities
{
    //Common event handlers and event arguments
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
    public class StatusChangedEventArgs : EventArgs
    {
        public string UpdateText { get; set; }
        public int UpdatePercentage { get; set; }
        public bool Marquee { get; set; }
    }

    public delegate void SystemLogUpdateEventHandler(object sender, SystemLogUpdateEventArgs e);
    public class SystemLogUpdateEventArgs : EventArgs
    {
        public string LogUpdateText { get; set; }
    }

    public delegate void StatusBarUpdateEventHandler(object sender, StatusBarUpdateEventArgs e);
    public class StatusBarUpdateEventArgs : EventArgs
    {
        public string StatusBarText { get; set; }
    }

    public delegate void NotificationEventHandler(object sender, NotificationEventArgs e);
    public class NotificationEventArgs : EventArgs
    {
        public NotificationViewModelBase NotificationViewModel { get; set; }

        public NotificationEventArgs(NotificationViewModelBase notificationViewModel)
        {
            this.NotificationViewModel = notificationViewModel;
        }
        
        public NotificationEventArgs() { }
        
    }
}
