using OratorCommonUtilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.Analyzer.Performance.Models
{
    public delegate void SessionChangedHandler(object sendor , SessionChangedEventArgs e);
    public class SessionChangedEventArgs : EventArgs
    {
        public object Session { get; set; }

        // Summary:
        //     Gets the action that caused the event.
        //
        // Returns:
        //     A System.Collections.Specialized.NotifyCollectionChangedAction value that
        //     describes the action that caused the event.
        public NotifyCollectionChangedAction Action { get; set; }
    }
    
}
