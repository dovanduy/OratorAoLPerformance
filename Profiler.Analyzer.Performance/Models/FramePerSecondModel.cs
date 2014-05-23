using OratorCommonUtilities;
using Profiler.LogManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.Analyzer.Performance.Models
{
    public class FramePerSecondModel : ModelBase
    {
        public int Instance { get; set; }
        public string CMP { get; set; }
        public DateTime StartStamp { get; set; }
        public DateTime DisplayedStamp { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan ActualTimeSpan { get; set; }

        public void Init(LogData logData)
        { 
            
        }
    }
}
