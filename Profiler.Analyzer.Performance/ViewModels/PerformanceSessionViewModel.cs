using OratorCommonUtilities;
using Profiler.Analyzer.Performance.Models;
using Profiler.Analyzer.Performance.Views;
using Profiler.LogManager.Models;
using Profiler.LogManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace Profiler.Analyzer.Performance.ViewModels
{
    public class PerformanceSessionViewModel : ViewModelBase
    {

        public event LogDataReceivedHandler ADBLogDataReceivedEvent;

        public event LogDataReceivedHandler LogDataReceivedEvent;

        public ObservableCollection<ViewModelBase> ViewModels { get; set; }

        public SessionBase Model { get; private set; }

        private ViewModelBase _selectedVM;
        public ViewModelBase SelectedVM
        {
            get { return _selectedVM; }
            set
            {
                _selectedVM = value;
                OnPropertyChanged("SelectedVM");
            }
        }

       

        public PerformanceSessionViewModel()
        {
            ViewModels = new ObservableCollection<ViewModelBase>();

            FramerateMonitorViewModel fpsVM = new FramerateMonitorViewModel();
            ViewModels.Add(fpsVM);

            OpenTimeViewModel opentimeVM = new OpenTimeViewModel();
            ViewModels.Add(opentimeVM);

            ReactionTimeViewModel reactionTimeVM = new ReactionTimeViewModel();
            ViewModels.Add(reactionTimeVM);

            ADBLogViewModel adbLoggerVM = new ADBLogViewModel();
            ADBLogDataReceivedEvent += adbLoggerVM.ReceivedLogData;
            ViewModels.Add(adbLoggerVM);

            //LogDataViewModel logDataVM = new LogDataViewModel();
            //LogDataReceivedEvent += logDataVM.ReceivedLogData;
            //ViewModels.Add(logDataVM);

            SelectedVM = fpsVM;

            //View = new PerformanceSessionView();
            Model = new PerformanceSession();
            DisplayName = Model.SessionName;
            //View.DataContext = this;

        }

        public void OnLogDataReceived(object sendor, LogDataReceivedEventArgs e)
        {
            if (LogDataReceivedEvent != null)
            {
                LogDataReceivedEvent(sendor, e);
            }
        }

        public void OnADBLogDataReceived(object sendor, LogDataReceivedEventArgs e)
        {
            // Cache

            // Dispatcher


            if (ADBLogDataReceivedEvent != null)
            {
                ADBLogDataReceivedEvent(sendor, e);
            }


        }



    }
}
