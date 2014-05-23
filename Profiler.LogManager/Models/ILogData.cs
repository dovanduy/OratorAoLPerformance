using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.LogManager.Models
{
    public interface ILogData
    {
        string ToString(OutputFormat format, LogTimestampMode timestampMode);

        string Data { get; set; }

        DateTime LocalTimestamp { get; }

        DateTime RemoteTimestamp { get; }

        LogBuffer SourceBuffer { get; }

        string Tag { get; }
    }
}
