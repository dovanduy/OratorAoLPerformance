using OratorCommonUtilities;
using Profiler.LogManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.LogManager.ViewModels
{
    public class LogDataViewModel : ViewModelBase
    {
        public string DisplayName { get; set; }

       private ObservableCollection<ILogData> _logCollection;
        public ObservableCollection<ILogData> LogCollection
        {
            get { return _logCollection; }
            private set
            {
                _logCollection = value;
                OnPropertyChanged("LogCollection");
            }
        }

        private ILogData _selectedItem;

        public ILogData SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public LogDataViewModel()
        {
            DisplayName = "Logger";
            LogCollection = new ObservableCollection<ILogData>();
            View = new LogDataView();
            View.DataContext = this;
        }

        private int _currentCount = 0;
        private int MAX_COUNT = 10;
        private ObservableCollection<ILogData> _logDataArray = new ObservableCollection<ILogData>();

        public void ReceivedLogData(object sendor, LogDataReceivedEventArgs args)
        {
            if (args == null || args.Log == null)
            { return; }


            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LogCollection.Add(args.Log);
                SelectedItem = LogCollection[LogCollection.Count - 1];
            });

            //_logDataArray.Add(args.Log);
            //_currentCount++;
            //if (_currentCount == MAX_COUNT)
            //{
            //    System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate()
            //    {
            //        foreach (var log in _logDataArray)
            //        {
            //            LogCollection.Add(log);
            //        }
            //        SelectedItem = LogCollection[LogCollection.Count - 1];
            //    });
            //    _logDataArray = new ObservableCollection<ILogData>();
            //    _currentCount = 0;
            //}
        }

        public void ClearLogData()
        {
            if (LogCollection != null)
            {
                LogCollection.Clear();
            }

            
        }
    }
}
