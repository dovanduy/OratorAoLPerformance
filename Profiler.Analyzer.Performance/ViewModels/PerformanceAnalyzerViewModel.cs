using OratorCommonUtilities;
using OratorCommonUtilities.Commands;
using Profiler.Analyzer.Performance.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visiblox.Charts;

namespace Profiler.Analyzer.Performance.ViewModels
{
    public class PerformanceAnalyzerViewModel : ViewModelBase
    {
        private RequestCommand _simulateCommand;
        public RequestCommand SimulateCommand { get { return _simulateCommand ?? (_simulateCommand = new RequestCommand(param => this.SimulateFPS())); } }

        private ObservableCollection<FramerateData> _framerateCollection;
        public ObservableCollection<FramerateData> FramerateCollection
        {
            get { return _framerateCollection; }
            set
            {
                _framerateCollection = value;
                OnPropertyChanged("FramerateCollection");
            }
        }

        public DateTime XAxisMinimum
        {
            get { return XAxisRange.Minimum; }
            private set
            {
                if (XAxisRange == null)
                {
                    XAxisRange = new DateTimeRange();
                }
                if (XAxisRange.Minimum.Year != DateTime.Now.Year)
                {
                    XAxisRange.Minimum = value;
                }
                else if (value < XAxisRange.Minimum)
                {
                    XAxisRange.Minimum = value;
                }
            }
        }

        public DateTime XAxisMaximum
        {
            get { return XAxisRange.Maximum; }
            private set
            {
                if (XAxisRange == null)
                {
                    XAxisRange = new DateTimeRange();
                }
                if (value > XAxisRange.Maximum)
                {
                    XAxisRange.Maximum = value;
                }
            }
        }

        private DateTimeRange _xAxisRange;
        public DateTimeRange XAxisRange
        {
            get { return _xAxisRange; }
            set
            {
                _xAxisRange = value;
                OnPropertyChanged("XAxisRange");
            }
        }


        public double YAxisMinimum
        {
            get
            {
                return YAxisRange.Minimum;
            }
            private set
            {
                if (YAxisRange == null)
                {
                    YAxisRange = new DoubleRange();
                }
                if (value < YAxisRange.Minimum)
                {
                    YAxisRange = new DoubleRange(value, YAxisRange.Maximum);
                }
            }
        }

        public double YAxisMaximum
        {
            get
            {
                return YAxisRange.Maximum;
            }
            private set
            {
                if (YAxisRange == null)
                {
                    YAxisRange = new DoubleRange();
                }
                if (value > YAxisRange.Maximum)
                {
                    YAxisRange = new DoubleRange(YAxisRange.Minimum, value);
                }
            }
        }

        private DoubleRange _yAxisRange;

        public DoubleRange YAxisRange
        {
            get
            {
                return _yAxisRange;
            }
            set
            {
                _yAxisRange = value;
                OnPropertyChanged("YAxisRange");
            }
        }

        public PerformanceAnalyzerViewModel()
        {
            FramerateCollection = new ObservableCollection<FramerateData>();
            FramerateCollection.CollectionChanged += FramerateCollection_CollectionChanged;

            DisplayName = "Performance Analyzer";
        }

        void FramerateCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void SimulateFPS()
        {
            DateTime begin = DateTime.Now;
            FramerateData beginFrame = new FramerateData();
            beginFrame.TimeStamp = begin;

            Random ran = new Random();
            DateTime next = begin.AddMilliseconds(ran.Next(40, 80));
            FramerateData previous = beginFrame;
            for (int i = 0; i < 500; i++)
            {
                FramerateData nextFrame = new FramerateData();
                nextFrame.TimeStamp = next;
                nextFrame.Span(previous);
                FramerateCollection.Add(nextFrame);
                previous = nextFrame;
                next = next.AddMilliseconds(ran.Next(40, 70));
            }

            XAxisMinimum = FramerateCollection.Min(x => x.TimeStamp);
            XAxisMaximum = FramerateCollection.Max(x => x.TimeStamp);

            YAxisMinimum = FramerateCollection.Min(y => y.Delta);
            YAxisMaximum = FramerateCollection.Max(y => y.Delta);
        }
    }
}
