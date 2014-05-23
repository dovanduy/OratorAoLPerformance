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
    public class ADBLogViewModel : ViewModelBase
    {
        public string DisplayName { get; set; }

        private ObservableCollection<string> _logTextCollection;
        public ObservableCollection<string> LogTextCollection
        {
            get { return _logTextCollection; }
            private set
            {
                _logTextCollection = value;
                OnPropertyChanged("LogTextCollection");
            }
        }

        private string _selectedTextItem;
        public string SelectedTextItem
        { get { return _selectedTextItem; } set { _selectedTextItem = value; OnPropertyChanged("SelectedTextItem"); } }

        public ADBLogViewModel()
        {
            DisplayName = "ADBLogger";
            LogTextCollection = new ObservableCollection<string>();

            LogTextCollection.CollectionChanged += LogTextCollection_CollectionChanged;
            View = new ADBLogDataView();
            View.DataContext = this;
        }

        void LogTextCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                SelectedTextItem = (string)e.NewItems[0];
            }
        }

        public void ReceivedLogData(object sendor, LogDataReceivedEventArgs args)
        {
            if (args == null || args.Log == null)
            { return; }
            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate()
            {
                LogTextCollection.Add(args.Log.Data);
            });
        }
    }
}
