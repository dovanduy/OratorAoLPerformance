using OratorCommonUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.Analyzer.Performance.Models
{
    public class PerformanceSession : SessionBase
    {
        public bool IsCheckedReactionTime { get; set; }
        public bool IsCheckedFPS { get; set; }
        public bool IsCheckedMCU { get; set; }

        public PerformanceSession()
        {
            DateTime dt = DateTime.Now;
            SessionName = dt.ToString("yyyy-MM-dd HH:mm:ss");
            CreateTimeStamp = dt;
        }

    }
}
