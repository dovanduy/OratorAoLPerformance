using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.LogManager.Models
{
    public interface ILogger
    {
        event LogDataReceivedHandler LogDataReceivedEvent;
        void Start(bool isClearBuffer);
        void Start(LogBuffer sourceBuffer, bool isClearBuffer);
        void Stop(bool isClearBuffer);
    }
}
