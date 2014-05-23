using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Managed.Adb;
using OratorCommonUtilities;
using Profiler.LogManager.ViewModels;
using Profiler.LogManager.Models;
using OratorCommonUtilities.Commands;
using System.Windows;

namespace TestAdb
{
    public class TestAdbDeviceViewModel : ViewModelBase 
    {
        public event LogDataReceivedHandler LogDataReceived;

        public event ClearLogDataHandler ClearLogDataEvent;

        private ObservableCollection<ViewModelBase> _viewModels;
        public ObservableCollection<ViewModelBase> ViewModels
        {
            get { return _viewModels; }
            set
            {
                _viewModels = value;
                OnPropertyChanged("ViewModels");
            }
        }

        private ObservableCollection<Device> _devices;
        public ObservableCollection<Device> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        private TestAdbDeviceModel _model;

        private Device _selectedDevice;
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }
        public ObservableCollection<LogBuffer> LogBufferCollection
        { get; set; }

        private LogBuffer _selectedLogBuffer;
        public LogBuffer SelectedLogBuffer
        {
            get { return _selectedLogBuffer; }
            set
            {
                _selectedLogBuffer = value;
                OnPropertyChanged("SelectedLogBuffer");
            }
        }

        private Logger _logger;

        private RequestCommand _getDevicesCommand;
        public RequestCommand GetDevicesCommand
        {
            get { return _getDevicesCommand ?? (_getDevicesCommand = new RequestCommand(param => this.GetDevices())); }
        }

        private RequestCommand _startLoggerCommand;
        public RequestCommand StartLoggerCommand
        {
            get { return _startLoggerCommand ?? (_startLoggerCommand = new RequestCommand(param => this.StartOutputEventLog()));}
        }

        private RequestCommand _stopLoggerCommand;
        public RequestCommand StopLoggerCommand
        {
            get { return _stopLoggerCommand ?? (_stopLoggerCommand = new RequestCommand(param => this.StopOutputLog())); }
        }

        private RequestCommand _clearLogCommand;
        public RequestCommand ClearLogCommand
        {
            get { return _clearLogCommand ?? (_clearLogCommand = new RequestCommand(param => this.ClearLog())); }
        }

        public TestAdbDeviceViewModel()
        {
            _model = new TestAdbDeviceModel();
            ViewModels = new ObservableCollection<ViewModelBase>();
            LogBufferCollection = new ObservableCollection<LogBuffer>();
            LogBufferCollection.Add(LogBuffer.All);
            LogBufferCollection.Add(LogBuffer.Events);
            LogBufferCollection.Add(LogBuffer.Kernel);
            LogBufferCollection.Add(LogBuffer.Main);
            LogBufferCollection.Add(LogBuffer.Radio);
            LogBufferCollection.Add(LogBuffer.System);
            SelectedLogBuffer = LogBuffer.All;
        }

        public void GetDevices()
        {
            if (Devices == null)
            {
                Devices = new ObservableCollection<Device>();
            }
            else
            {
                Devices.Clear();
            }
            List<Device> devices = _model.GetDevices();
            foreach (var device in devices)
            {
                Devices.Add(device);
            }
            if (Devices.Count > 0)
            {
                SelectedDevice = Devices[0];
            }
        }

        private void StartOutputEventLog()
        {
            if (SelectedDevice != null)
            {
                _logger = new Logger(SelectedDevice);
                _logger.LogDataReceivedEvent += _logger_LogDataReceivedEvent;
                _logger.Start(SelectedLogBuffer, false);
                _logger.Start(false, true);
            }
        }

        void _logger_LogDataReceivedEvent(object sendor, LogDataReceivedEventArgs args)
        {
            if (LogDataReceived != null)
            {
                LogDataReceived(sendor, args);
            }
        }

        private void StopOutputLog()
        {
            if (_logger != null)
            {
                _logger.Stop(false);
            }
        }

        public void ClearLog()
        {
            if (ClearLogDataEvent != null)
            {
                ClearLogDataEvent();
            }
        }
    }
}
