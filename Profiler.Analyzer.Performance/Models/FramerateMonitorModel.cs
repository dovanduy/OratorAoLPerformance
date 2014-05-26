using OratorCommonUtilities;
using Profiler.LogManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visiblox.Charts;

namespace Profiler.Analyzer.Performance.Models
{
    public class FramerateData : ModelBase
    {
        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set
            {
                _timeStamp = value;
                OnPropertyChanged("TimeStamp");
            }
        }

        private double _delta;
        public double Delta
        {
            get { return _delta; }
            set
            {
                _delta = value;
                OnPropertyChanged("Delta");
            }
        }

        public void Span()
        {
            this.Span(null);
        }

        public void Span(FramerateData previous)
        {
            if (previous == null)
            {
                Delta = 0d;
            }
            else
            {
                double span = TimeStamp.Ticks - previous.TimeStamp.Ticks;
                Delta = span / TimeSpan.TicksPerMillisecond;
                Delta = Math.Round(Delta, 2);
            }
        }
    }
}
