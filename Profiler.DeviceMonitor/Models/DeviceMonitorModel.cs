using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Managed.Adb;
using System.Net.Sockets;
using System.Net;
using Managed.Adb.Logs;
using System.IO;

namespace Profiler.DeviceMonitor
{
    public class DeviceMonitorModel
    {
        private AndroidDebugBridge _bridge;

        private IList<Device> _devices;
        public IList<Device> Devices
        {
            get
            {
                return _bridge.Devices;
            }
        }

        public DeviceMonitorModel()
        {
            string adbFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AndroidDebugBridge.ADB);
            //_bridge = AndroidDebugBridge.CreateBridge(adbFileName, false);
        }

        public List<Device> GetDevices()
        {
            return AdbHelper.Instance.GetDevices(AndroidDebugBridge.SocketAddress);
        }

        public Socket Open(IDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
            return AdbHelper.Instance.Open(AndroidDebugBridge.HostAddress, device, AndroidDebugBridge.ADB_PORT);
        }

        public bool Disconnect(IDevice device)
        {
            return true;
        }

        public void RunEventLogService(IPEndPoint address, IDevice device, LogReceiver rcvr)
        {
            Device d = (Device)device;
            AdbHelper.Instance.RunEventLogService(address, d, rcvr);
        }
    }
}
