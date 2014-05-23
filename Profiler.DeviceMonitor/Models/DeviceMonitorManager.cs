
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Managed.Adb;
using System.Net.Sockets;
using System.IO;

namespace Profiler.DeviceMonitor.Models
{
    public class DeviceMonitorManager
    {
        public bool IsRunning;
        private AndroidDebugBridge _bridge;
        private Socket _monitorSocket;



        public DeviceMonitorManager()
        {
            AndroidDebugBridge.AdbOsLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AndroidDebugBridge.ADB);
            _bridge = AndroidDebugBridge.Instance;
        }

        public void Start()
        {
            _bridge.Start();
        }

        public void Stop()
        {
            _bridge.Stop();
        }
    }
}
