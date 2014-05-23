using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OratorCommonUtilities
{
    public class OneTimeNotificationViewModel : NotificationViewModelBase
    {
        public OneTimeNotificationViewModel(string notificationText)
        {
            this.NotificationText = notificationText;
        }
    }
}
