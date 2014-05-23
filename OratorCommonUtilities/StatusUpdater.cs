using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OratorCommonUtilities
{
    /// <summary>
    /// Class that can be used for updating the GUI during long tasks. Can be used to update the progression bar, status bar or both at the same time.
    /// </summary>
    public class StatusUpdater : IStatusUpdater
    {
        public event SystemLogUpdateEventHandler SystemLogUpdateEvent;
        protected virtual void OnSystemLogUpdate(string logUpdateText)
        {
            if (SystemLogUpdateEvent != null)
            {
                SystemLogUpdateEventArgs args = new SystemLogUpdateEventArgs();
                args.LogUpdateText = logUpdateText;
                this.SystemLogUpdateEvent(this, args);
            }
        }

        public event NotificationEventHandler NotificationEvent;
        protected virtual void OnNotificationEvent(NotificationViewModelBase notificationViewModel)
        {
            if (NotificationEvent != null)
            {
                NotificationEventArgs args = new NotificationEventArgs(notificationViewModel);
                this.NotificationEvent(this, args);
            }
        }
        protected virtual void OnNotificationEvent(string notificationText)
        {
            if (NotificationEvent != null)
            {                
                NotificationEventArgs args = new NotificationEventArgs(new OneTimeNotificationViewModel(notificationText));
                this.NotificationEvent(this, args);
            }
        }
    }
}
