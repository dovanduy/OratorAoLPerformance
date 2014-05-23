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

namespace Profiler.Analyzer.Performance.ViewModels
{
    public class ReactionTimeViewModel : ViewModelBase
    {
        public ObservableCollection<ReactionTimeModel> ModelCollection { get; private set; }

        public ReactionTimeViewModel()
        {
            DisplayName = "Reaction Time";
            this.View = new ReactionTimeView();
            this.View.DataContext = this;
        }
    }
}
