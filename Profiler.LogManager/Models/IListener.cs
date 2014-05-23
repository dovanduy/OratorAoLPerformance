using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.LogManager.Models
{
    public interface IListener
    {
        event LogDataReceivedHandler LogDataReceivedEvent;
        void Start(bool isClearBuffer);
        void Stop(bool isClearBuffer);
    }
}
