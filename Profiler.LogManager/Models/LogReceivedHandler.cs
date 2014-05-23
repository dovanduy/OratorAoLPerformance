using Managed.Adb.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.LogManager.Models
{
    public delegate void LogEntryReceivedHandler(object sendor, LogEntryReceivedEventArgs args);
    public class LogEntryReceivedEventArgs : EventArgs
    {
        public LogEntry Log { get; set; }
    
    }
    public delegate void LogDataReceivedHandler(object sendor, LogDataReceivedEventArgs args);

    public class LogDataReceivedEventArgs : EventArgs
    {
        public ILogData Log { get; set; }

        public List<ILogData> LogArray { get; set; }
    }
}
