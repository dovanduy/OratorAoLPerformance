using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Managed.Adb;
using System.Net.Sockets;

namespace Profiler.DeviceMonitor.Models
{
    public class DeviceConnection
    {
        public IDevice ConnectedDevice { get; set; }
        public Socket ConnectedSocket { get; set; }
    }
}
