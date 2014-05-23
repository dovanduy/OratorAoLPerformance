using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OratorCommonUtilities
{
    /// <summary>
    /// Class that can be used for updating the GUI during long tasks. Can be used to update the progression bar, status bar or both at the same time.
    /// </summary>
    public class StatusUpdater : NotifyPropertyChangedBase , IStatusUpdater, IDisposable
    {
        #region StatusUpdater
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
	#endregion

        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public virtual void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }
        #endregion // IDisposable Members
    }
}
