using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

using OratorCommonUtilities;

namespace OratorPluginBase
{
    public class CommandViewModel : ViewModelBase
    {        
        public string Text { get; set; }
        public ICommand Command { get; set; }

        public CommandViewModel(string text, ICommand command)
        {
            Text = text;
            Command = command;
        }

        public CommandViewModel()
        {
        }
    }

    public class CheckableCommandViewModel : CommandViewModel
    {
        private bool _enabled;
        public bool Enabled
        {
            get{ return _enabled;}
            set
            {
                _enabled = value;
                OnPropertyChanged("Enabled");
            }
        }
    }
}
