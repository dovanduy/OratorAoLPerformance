using OratorCommonUtilities;
using Profiler.Analyzer.Performance.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.Analyzer.Performance.ViewModels
{
    public class SessionExplorerViewModel : ViewModelBase
    {
        public event SessionChangedHandler SessionChangedEvent;

        private ObservableCollection<ViewModelBase> _sessions;
        public ObservableCollection<ViewModelBase> Sessions
        {
            get { return _sessions; }
            private set
            {
                _sessions = value;
                OnPropertyChanged("Sessions");
            }
        }

        public SessionExplorerViewModel()
        {
            DisplayName = "Session Explorer";
            Sessions = new ObservableCollection<ViewModelBase>();
            Sessions.CollectionChanged += Sessions_CollectionChanged;
        }

        void Sessions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                if (SessionChangedEvent != null)
                {
                    SessionChangedEventArgs args = new SessionChangedEventArgs();
                    args.Session = e.NewItems[0];
                    args.Action = e.Action;
                    SessionChangedEvent(this, args);
                }
            }

            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                if (SessionChangedEvent != null)
                {
                    SessionChangedEventArgs args = new SessionChangedEventArgs();
                    args.Session = e.OldItems[0];
                    args.Action = e.Action;
                    SessionChangedEvent(this, args);
                }
            }
        }


        public void Add(ViewModelBase session)
        {
            Sessions.Add(session);
        }
    }
}
