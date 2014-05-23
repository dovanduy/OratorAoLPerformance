using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.LogManager.Models
{
    public enum LogPriority
    {
        Unknown,
        Default,
        Verbose,
        Debug,
        Info,
        Warning,
        Error,
        Fatal,
        Silent
    }
}
