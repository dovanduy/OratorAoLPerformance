using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Managed.Adb;
using System.Net.Sockets;

namespace Profiler.DeviceMonitor.Models
{
    public delegate void UpdateDevicesHandler(object sendor, UpdateDevicesEventArgs e);
    public class UpdateDevicesEventArgs : EventArgs
    {
        public IList<IDevice> Devices { get; set; }
    }
}
