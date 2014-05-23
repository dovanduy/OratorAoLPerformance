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
    public class FramePerSecondViewModel : ViewModelBase
    {
        public ObservableCollection<FramePerSecondModel> ModelCollection { get; private set; }

        public FramePerSecondViewModel()
        {
            DisplayName = "Frame Per Second";
            this.View = new FramePerSecondView();
            this.View.DataContext = this;
        }

        public void Add(LogData logData)
        {
            FramePerSecondModel model = new FramePerSecondModel();
            model.Init(logData);
            ModelCollection.Add(model);
        }

    }
}
