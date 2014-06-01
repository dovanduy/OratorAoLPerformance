using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Net.Sockets;
using System.IO;
using System.Windows.Threading;

using Profiler.Analyzer.Performance;
using Profiler.Analyzer.Performance.Models;
using OratorCommonUtilities.Commands;
using Managed.Adb;
using Profiler.LogManager.Models;
using OratorCommonUtilities;
using OratorPluginBase;
using Profiler.DeviceMonitor;
using System.ComponentModel;
using Orator.AoL.MainWindow.Models;
using Profiler.Analyzer.Performance.ViewModels;
using Profiler.DeviceMonitor.Models;

namespace Orator.AoL.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        private LogDataReceivedHandler _logDataReceived;
        private LogDataReceivedHandler _adbLogDataReceived;

        #region Devices

        private DeviceCollection _devices;
        public DeviceCollection Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        private DeviceConncectState _connectedState;
        public DeviceConncectState ConnectedState
        {
            get { return _connectedState; }
            set
            {
                _connectedState = value;
                OnPropertyChanged("ConnectedState");
            }
        }
        #endregion

        #region Commands
        private RequestCommand _socketActionCommand; 
        /// <summary>
        /// start socket action to collect logs
        /// </summary>
        public ICommand SocketActionCommand
        {
            get
            {
                return _socketActionCommand ?? (_socketActionCommand = new RequestCommand(param => this.CanStartLogger(), param => this.StartSocketActionLogger()));
            }
        }


        private RequestCommand _startADBProcessCommand;
        /// <summary>
        /// start adb.exe process through logcat command to collect logs
        /// </summary>
        public ICommand StartADBProcessCommand
        {
            get
            {
                return _startADBProcessCommand ?? (_startADBProcessCommand = new RequestCommand(param => this.CanStartLogger(), param => this.StartADBProcess()));
            }
        }

        private RequestCommand _stopLoggerCommand;
        /// <summary>
        /// stop socket action or adb.exe process 
        /// </summary>
        public ICommand StopLoggerCommand
        {
            get { return _stopLoggerCommand ?? (_stopLoggerCommand = new RequestCommand(param => this.StopLogger())); }
        } 
        #endregion


        public ViewModelBase MainDockPanelVM { get; set; }

        public Action StopMonitorDevice { get; private set; }

        public MainWindowViewModel()
		{
            ConnectedState = DeviceConncectState.NoDevice;
            MainDockPanelVM = new PerformanceViewModel();
            _logDataReceived += ((PerformanceViewModel)MainDockPanelVM).OnLogDataReceived;
            _adbLogDataReceived += ((PerformanceViewModel)MainDockPanelVM).OnADBLogDataReceived;
            Devices = new DeviceCollection();
		}

        public void Init()
        {
            string adbLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DeviceMonitorHelper.ADB);

            AndroidDebugBridge.CreateBridge(adbLocation, false);
            AndroidDebugBridge.Instance.DeviceMonitor.UpdateDeviceListEvent += DeviceMonitor_UpdateDeviceListEvent;
            StopMonitorDevice = () =>
            {
                AndroidDebugBridge.Instance.Stop();
            };
        }

        void DeviceMonitor_UpdateDeviceListEvent(object sendor, UpdateDeviceListEventArgs e)
        {

            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                try
                {
                    Devices.Clear();
                    if (e != null && e.DeviceList != null)
                    {
                        foreach (var device in e.DeviceList)
                        {
                            Devices.Add(device);
                        }
                    }
                    if (Devices.Count > 0)
                    {
                        ConnectedState = DeviceConncectState.CanConncect;
                    }
                    else
                    {
                        ConnectedState = DeviceConncectState.NoDevice;
                    }
                }
                catch (Exception ex)
                {

                }
            });
        }

        private void UpdateDeviceList(IList<Device> newDeviceList)
        {

        }

        public bool CanStartLogger()
        {
            return Devices.Default == null ? false : true;
        }

        private Action _stopLoggerAction;

        public void StartSocketActionLogger()
        {
            if (Devices.Default != null)
            {
                ConnectedState = DeviceConncectState.CanDisconnect;
                Logger logger = new Logger(Devices.Default);
                logger.LogDataReceivedEvent += logger_LogDataReceivedEvent;
                _stopLoggerAction = () => {
                    logger.Stop(false);
                };
                logger.Start(false);
            }
        }

        void logger_LogDataReceivedEvent(object sendor, LogDataReceivedEventArgs args)
        {
            if (_logDataReceived != null)
            {
                _logDataReceived(this, args);
            }
        }

        public void StartADBProcess()
        {
            try
            {
                ConnectedState = DeviceConncectState.CanDisconnect;
                string adbLocation = AppDomain.CurrentDomain.BaseDirectory;
                ADBLogger adbLogger = new ADBLogger(adbLocation);
                adbLogger.LogDataReceived += logger_ADBLogDataReceivedEvent;
                _stopLoggerAction = () =>
                {
                    adbLogger.Stop();
                };
                adbLogger.Start();
            }
            catch (Exception ex)
            {
             
            }
        }

        public void StopLogger()
        {
            try
            {
                ConnectedState = DeviceConncectState.CanConncect;
                if (_stopLoggerAction != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(_stopLoggerAction);
                    _stopLoggerAction = null;
                }
            }
            catch (Exception ex)
            {

            }
        }

        void logger_ADBLogDataReceivedEvent(object sendor, LogDataReceivedEventArgs args)
        {

            if (_adbLogDataReceived != null)
            {
                _adbLogDataReceived(this, args);
            }
        }
    }
}
