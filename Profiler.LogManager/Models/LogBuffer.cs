using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.LogManager.Models
{
    public enum LogBuffer
    {
        Events = 1,
        Main = 2,
        Radio =  4,
        System = 8,
        Kernel = 16,
        All = (LogBuffer.Events | LogBuffer.Main | LogBuffer.Radio | LogBuffer.System | LogBuffer.Kernel)
    }
}
