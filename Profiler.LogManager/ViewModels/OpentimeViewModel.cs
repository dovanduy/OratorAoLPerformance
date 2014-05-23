using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OratorCommonUtilities;
using Profiler.LogManager.Views;

namespace Profiler.LogManager.ViewModels
{
    public class OpentimeViewModel : ViewModelBase
    {
        public OpentimeViewModel()
        {
            DisplayName = "Opentime";
            View = new OpentimeView();
            View.DataContext = this;
        }
    }
}
