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
    /// <summary>
    /// 1.Start ADB process to monitor devices connected with this
    /// 2.Could create connection for get logs
    /// 3.Could match other functions
    ///   3.1 add session
    ///   3.2 remove session
    ///   3.3 end session
    ///   3.4 view some screems, for example document, anchorable
    /// </summary>
    public class NewMainWindowViewModel : ViewModelBase
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

        public NewMainWindowViewModel()
		{
            ConnectedState = DeviceConncectState.NoDevice;
            MainDockPanelVM = new PerformanceViewModel();
            _logDataReceived += ((PerformanceViewModel)MainDockPanelVM).OnLogDataReceived;
            _adbLogDataReceived += ((PerformanceViewModel)MainDockPanelVM).OnADBLogDataReceived;
            Devices = new DeviceCollection();


            SessionViewModelCollection = new ObservableCollection<PerformanceSessionViewModel>();
            SessionViewModelCollection.CollectionChanged += SessionViewModelCollection_CollectionChanged;

            this.Init();
		}

        private void Init()
        {
            Anchorables = new ObservableCollection<ViewModelBase>();
            Documents = new ObservableCollection<ViewModelBase>();

            SessionExplorerVM = new SessionExplorerViewModel();
            SessionExplorerVM.SessionChangedEvent += SessionExplorerVM_SessionChangedEvent;
            Anchorables.Add(SessionExplorerVM);

            this.InitADB();
        }

        void SessionExplorerVM_SessionChangedEvent(object sendor, SessionChangedEventArgs e)
        {
            switch (e.Action)
            { 
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
            }
        }


        public void InitADB()
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


        public event LogDataReceivedHandler LogDataReceivedEvent;
        public event LogDataReceivedHandler ADBLogDataReceivedEvent;

        private ObservableCollection<PerformanceSessionViewModel> _sessionViewModelCollection;
        public ObservableCollection<PerformanceSessionViewModel> SessionViewModelCollection
        {
            get { return _sessionViewModelCollection; }
            set
            {
                _sessionViewModelCollection = value;
                OnPropertyChanged("SessionViewModelCollection");
            }
        }

        private PerformanceSessionViewModel _selectedSessionViewModel;
        public PerformanceSessionViewModel SelectedSessionViewModel
        {
            get
            {
                return _selectedSessionViewModel;
            }
            set
            {
                _selectedSessionViewModel = value;
                OnPropertyChanged("SelectedSessionViewModel");
            }
        }

        #region Sessions

        private ObservableCollection<ViewModelBase> _documents;

        public ObservableCollection<ViewModelBase> Documents
        {
            get { return _documents; }
            private set
            {
                _documents = value;
                OnPropertyChanged("Documents");
                ActiveDocument = GetLast(_documents);
            }
        }

        private ViewModelBase _activeDocument;

        public ViewModelBase ActiveDocument
        {
            get { return _activeDocument; }
            private set
            {
                _activeDocument = value;
                OnPropertyChanged("ActiveDocument");
            }
        }

        private SessionExplorerViewModel _sessionExplorerVM;

        public SessionExplorerViewModel SessionExplorerVM
        {
            get { return _sessionExplorerVM; }
            private set
            {
                _sessionExplorerVM = value;
            }
        }

        ObservableCollection<ViewModelBase> _anchorables;

        public ObservableCollection<ViewModelBase> Anchorables
        {
            get { return _anchorables; }
            set
            {
                _anchorables = value;
                OnPropertyChanged("Anchorables");
            }
        }

        private ViewModelBase _activeAnchorable;

        public ViewModelBase ActiveAnchorable
        {
            get { return _activeAnchorable; }
            private set
            {
                _activeAnchorable = value;
                OnPropertyChanged("ActiveAnchorable");
            }
        }

        #endregion


        #region Commands
        private RequestCommand _addSessionCommand;
        public ICommand AddSessionCommand
        {
            get { return _addSessionCommand ?? (_addSessionCommand = new RequestCommand(param => this.AddSession())); }
        }

        private RequestCommand _removeSelectedSessionCommand;
        public ICommand RemoveSelectedSessionCommand
        {
            get { return _removeSelectedSessionCommand ?? (_removeSelectedSessionCommand = new RequestCommand(param => this.RemoveSession())); }
        }

        private RequestCommand _removeAllSessionCommand;
        public ICommand RemoveAllSessionCommand
        {
            get { return _removeAllSessionCommand ?? (_removeAllSessionCommand = new RequestCommand(param => this.RemoveAll())); }
        } 
        #endregion

        void SessionViewModelCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                SelectedSessionViewModel = (PerformanceSessionViewModel)e.NewItems[0];
            }

            if (e.OldItems != null && e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (SessionViewModelCollection.Count == 0)
                {
                    SelectedSessionViewModel = null;
                }
                else
                {
                    if (e.OldStartingIndex == 0)
                    {
                        SelectedSessionViewModel = SessionViewModelCollection[0];
                    }
                    else if (e.OldStartingIndex == SessionViewModelCollection.Count)
                    {
                        SelectedSessionViewModel = SessionViewModelCollection[SessionViewModelCollection.Count - 1];
                    }
                    else if (e.OldStartingIndex < SessionViewModelCollection.Count)
                    {
                        SelectedSessionViewModel = SessionViewModelCollection[e.OldStartingIndex];
                    }
                }
            }
        }

        public void AddSession()
        {
            PerformanceSessionViewModel newSessionVM = new PerformanceSessionViewModel();
            LogDataReceivedEvent += newSessionVM.OnLogDataReceived;
            ADBLogDataReceivedEvent += newSessionVM.OnADBLogDataReceived;
            SessionViewModelCollection.Add(newSessionVM);

            PerformanceAnalyzerViewModel performanceAnalyzerVM = new PerformanceAnalyzerViewModel();
            SessionExplorerVM.Add(performanceAnalyzerVM);
            Documents.Add(performanceAnalyzerVM);
        }

        public bool RemoveSession()
        {
            bool isClear = false;
            if (SelectedSessionViewModel != null)
            {
                if (LogDataReceivedEvent != null)
                {
                    LogDataReceivedEvent -= SelectedSessionViewModel.OnLogDataReceived;
                }
                if (ADBLogDataReceivedEvent != null)
                {
                    ADBLogDataReceivedEvent -= SelectedSessionViewModel.OnADBLogDataReceived;
                }

                SessionViewModelCollection.Remove(SelectedSessionViewModel);
                isClear = true;
            }
            return isClear;
        }

        public void FinishSession()
        {
            if (SelectedSessionViewModel != null)
            {
                if (LogDataReceivedEvent != null)
                {
                    LogDataReceivedEvent -= SelectedSessionViewModel.OnLogDataReceived;
                }
                if (ADBLogDataReceivedEvent != null)
                {
                    ADBLogDataReceivedEvent -= SelectedSessionViewModel.OnADBLogDataReceived;
                }

                SessionViewModelCollection.Remove(SelectedSessionViewModel);
            }
        }

        public void RemoveAll()
        {
            SessionViewModelCollection.Clear();
        }

        public void OnLogDataReceived(object sendor, LogDataReceivedEventArgs e)
        {
            if (LogDataReceivedEvent != null)
            {
                LogDataReceivedEvent(sendor, e);
            }
        }

        public void OnADBLogDataReceived(object sendor, LogDataReceivedEventArgs e)
        {
            if (ADBLogDataReceivedEvent != null)
            {
                ADBLogDataReceivedEvent(sendor, e);
            }
        }

        public void OnLogEntryReceived(object sendor, LogEntryReceivedEventArgs e)
        { 
        
        }


        private ViewModelBase GetLast(ObservableCollection<ViewModelBase> viewmodels)
        {
            if (viewmodels == null || viewmodels.Count == 0)
            {
                return null;
            }

            return viewmodels[viewmodels.Count - 1];
        }
    }
}
