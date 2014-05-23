using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestControls
{
    public class TestLogData
    {
        private DateTime m_epoc = new DateTime(0x7b2, 1, 1, 0, 0, 0);

        public string Data { get; set; }

        public DateTime LocalTimestamp { get; internal set; }

        public int NanoSeconds { get; internal set; }

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

    public enum LogBuffer
    {
        Events = 1,
        Main = 2,
        Radio = 4,
        System = 8,
        Kernel = 16,
        All = (LogBuffer.Events | LogBuffer.Main | LogBuffer.Radio | LogBuffer.System | LogBuffer.Kernel)
    }
}
