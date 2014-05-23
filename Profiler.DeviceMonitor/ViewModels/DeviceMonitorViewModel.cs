using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using OratorCommonUtilities;
using Managed.Adb;
using System.Net.Sockets;
using Managed.Adb.Logs;

namespace Profiler.DeviceMonitor
{
    public class DeviceMonitorViewModel : ViewModelBase
    {
        private DeviceMonitorModel _model;

        private ObservableCollection<IDevice> _devices;
        public ObservableCollection<IDevice> Devices {

            get {
                if (_model.Devices != null)
                {
                    foreach (var item in _model.Devices)
                    {
                        _devices.Add(item);
                    }
                }
                return _devices;
            }
        }

        public ObservableCollection<IDevice> DeviceList { get; set;}


        public DeviceMonitorViewModel()
        {
            _model = new DeviceMonitorModel();
            DeviceList = new ObservableCollection<IDevice>();
        }

        public void GetDevices()
        {
           List<Device> devices  = _model.GetDevices();
           foreach (Device d in devices)
           {
               DeviceList.Add(d);
           }
        }

        public Socket ConnectDevice(IDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
            return _model.Open(device);
        }

        public void RunEventLogService(IDevice device)
        {
            LogReceiver lr = new LogReceiver(null);
            _model.RunEventLogService(AndroidDebugBridge.SocketAddress, device, lr);
        }
    }
}
