using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.LogManager.Models
{
    public class LogData : ILogData
    {
        private DateTime m_epoc = new DateTime(0x7b2, 1, 1, 0, 0, 0);

        public override string ToString()
        {
            return this.ToString(OutputFormat.Brief, LogTimestampMode.Remote);
        }

        public string ToString(OutputFormat format, LogTimestampMode timestampMode)
        {
            DateTime time = (timestampMode == LogTimestampMode.Remote) ? this.RemoteTimestamp : this.LocalTimestamp;
            switch (format)
            {
                case OutputFormat.Brief:
                    return string.Format("{0}/{1}({2}): {3}", new object[] { this.Priority.ToString()[0], this.Tag.PadRight(8, ' '), this.ProcessId.ToString().PadLeft(5, ' '), this.Data });

                case OutputFormat.Process:
                    return string.Format("{0}({1}) {2}", this.Priority.ToString()[0], this.ProcessId.ToString().PadLeft(5, ' '), this.Data);

                case OutputFormat.Tag:
                    return string.Format("{0}/{1}: {2}", this.Priority.ToString()[0], this.Tag, this.Data);

                case OutputFormat.Raw:
                    return this.Data;

                case OutputFormat.Time:
                    return string.Format("{0} {1}/{2}( {3}): {4}", new object[] { time.ToString("MM-dd HH:mm:ss.fff"), this.Priority.ToString()[0], this.Tag.PadRight(8, ' '), this.ProcessId, this.Data });

                case OutputFormat.ThreadTime:
                    return string.Format("{0} {1} {2} {3} {4}: {5}", new object[] { time.ToString("MM-dd HH:mm:ss.fff"), this.ProcessId.ToString().PadLeft(5, ' '), this.ThreadId.ToString().PadLeft(5, ' '), this.Priority.ToString()[0], this.Tag, this.Data });

                case OutputFormat.Long:
                    return string.Format("[ {0} {1}: {2} {3}/{4} ]\r\n{5}\r\n", new object[] { time.ToString("MM-dd HH:mm:ss.fff"), this.ProcessId, this.ThreadId, this.Priority.ToString()[0], this.Tag, this.Data });

                case OutputFormat.ADBMimic:
                    return string.Format("{0} {1}/{2}( {3}) ({4}): {5}", new object[] { time.ToString("MM-dd HH:mm:ss.fff"), this.Priority.ToString()[0], this.Tag, this.ProcessId, this.ThreadId, this.Data });
            }
            return string.Empty;
        }

        public string Data { get; set; }

        public EventContainer EventData { get; internal set; }

        public bool HasGCEventData
        {
            get
            {
                return ((this.EventData != null) && (this.EventData is GcEventContainer));
            }
        }

        public DateTime LocalTimestamp { get; internal set; }

        public int NanoSeconds { get; internal set; }

        public LogPriority Priority { get; internal set; }

        public int ProcessId { get; internal set; }

        public DateTime RemoteTimestamp
        {
            get
            {
                return this.m_epoc.AddSeconds((double)this.Seconds).AddTicks((long)(this.NanoSeconds / 100)).ToLocalTime();
            }
        }

        public int Seconds { get; internal set; }

        public LogBuffer SourceBuffer { get; internal set; }

        public string Tag { get; internal set; }

        public int ThreadId { get; internal set; }
    }
}
