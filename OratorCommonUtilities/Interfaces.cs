using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OratorCommonUtilities
{
    //For all elements that wish to update the UI
    public interface IStatusUpdater
    {
        event SystemLogUpdateEventHandler SystemLogUpdateEvent;
        event NotificationEventHandler NotificationEvent;
    }
}
