using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using OratorCommonUtilities;
using System.Windows.Controls;
using System.Windows.Input;
using OratorCommonUtilities.Commands;
using Profiler.Analyzer.Performance.Models;
using Profiler.LogManager.Models;
using Managed.Adb.Logs;

namespace Profiler.Analyzer.Performance.ViewModels
{
	public class PerformanceViewModel : ViewModelBase
	{
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

        public PerformanceViewModel()
		{
            SessionViewModelCollection = new ObservableCollection<PerformanceSessionViewModel>();
            SessionViewModelCollection.CollectionChanged += SessionViewModelCollection_CollectionChanged;
            View = new PerformanceView();
            View.DataContext = this;

            this.AddSession();
		}

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
	}
}