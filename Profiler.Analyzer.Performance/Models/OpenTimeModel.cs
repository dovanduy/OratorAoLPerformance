using OratorCommonUtilities;
using Profiler.LogManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.Analyzer.Performance.Models
{
    public class OpenTimeModel : ModelBase
    {
        public int Instance { get; set; }
        public string Alias { get; set; }
        public string CMP { get; set; }
        public string StartStamp { get; set; }
        public string DisplayedStamp { get; set; }
        public double Duration { get; set; }
        public double ActualTimeSpan { get; set; }

        public void Init(LogData logData)
        { 
            
        }
    }
}
