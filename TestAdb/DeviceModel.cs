using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Managed.Adb;
using OratorCommonUtilities;

namespace TestAdb
{
    public class TestAdbDeviceModel
    {

        public List<Device> GetDevices()
        {
            return AdbHelper.Instance.GetDevices(AndroidDebugBridge.SocketAddress);
        }
    }

    public delegate void ClearLogDataHandler();
}
