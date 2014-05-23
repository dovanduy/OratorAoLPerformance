using OratorCommonUtilities;
using Profiler.LogManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Profiler.Analyzer.Performance.Models;
using Profiler.Analyzer.Performance.Views;
using Visiblox.Charts;

namespace Profiler.Analyzer.Performance.ViewModels
{
    public class OpenTimeViewModel : ViewModelBase
    {
        private ObservableCollection<OpenTimeModel> _openTimeCollection;
        public ObservableCollection<OpenTimeModel> OpenTimeCollection
        {
            get { return _openTimeCollection; }
            set
            {
                _openTimeCollection = value;
                OnPropertyChanged("OpenTimeCollection");
            }
        }

        private double _xAxisMaximum;

        public double XAxisMaximum
        {
            get
            {
                return _xAxisMaximum;
            }
            private set
            {
                if (value > _xAxisMaximum)
                {
                    _xAxisMaximum = value;
                    XAxisDoubleRange = new DoubleRange(0, value);
                }
            }
        }

        private DoubleRange _xAxisDoubleRange;

        public DoubleRange XAxisDoubleRange
        {
            get
            {
                return _xAxisDoubleRange;
            }
            set
            {
                _xAxisDoubleRange = value;
                OnPropertyChanged("XAxisDoubleRange"); 
            }
        }



        public OpenTimeViewModel()
        {
            OpenTimeCollection = new ObservableCollection<OpenTimeModel>();
            OpenTimeCollection.CollectionChanged += OpenTimeCollection_CollectionChanged;
            DisplayName = "Opentime";
            this.View = new OpenTimeView();
            this.View.DataContext = this;
            this.InitDemo();
        }

        void OpenTimeCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            XAxisMaximum = OpenTimeCollection.Max(x => x.Duration);
        }

        private void InitDemo()
        {
            OpenTimeModel model = new OpenTimeModel();
            model.CMP = "com.android.deskclock/.DeskClock";
            model.Alias = "DeskClock";
            model.StartStamp = "2014-05-18 23:18:50.350";
            model.DisplayedStamp = "2014-05-18 23:18:52.690";
            model.Duration = 2069d;
            model.ActualTimeSpan = 2340d;
            OpenTimeCollection.Add(model);

            model = new OpenTimeModel();
            model.CMP = "com.android.calendar/.AllInOneActivity";
            model.Alias = "Calendar";
            model.StartStamp = "2014-05-18 23:18:50.700";
            model.DisplayedStamp = "2014-05-18 23:18:52.690";
            model.Duration = 1605d;
            model.ActualTimeSpan = 1990d;
            OpenTimeCollection.Add(model);

            model = new OpenTimeModel();
            model.CMP = "com.android.calculator2/.Calculator";
            model.Alias = "calculator2";
            model.StartStamp = "2014-05-18 23:18:57.270";
            model.DisplayedStamp = "2014-05-18 23:18:58.320";
            model.Duration = 813d;
            model.ActualTimeSpan = 1050d;
            OpenTimeCollection.Add(model);

            model = new OpenTimeModel();
            model.CMP = "com.android.settings/.Settings";
            model.Alias = "Settings";
            model.StartStamp = "2014-05-18 23:19:01.830";
            model.DisplayedStamp = "2014-05-18 23:19:02.940";
            model.Duration = 965d;
            model.ActualTimeSpan = 1110d;
            OpenTimeCollection.Add(model);
        }

        public void Add(LogData logData)
        {
            OpenTimeModel model = new OpenTimeModel();
            model.Init(logData);
            OpenTimeCollection.Add(model);
        }

    }
}
