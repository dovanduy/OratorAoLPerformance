using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace Profiler.Analyzer.Performance
{
    public static class Helpers
    {
        public static StreamResourceInfo GetApplicationResourceStream(string resource)
        {
            return Application.GetRemoteStream(new Uri(resource, UriKind.Relative));
        }
    }
}
