using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using OratorCommonUtilities;

namespace OratorPluginBase
{
    public class DropDownMenuOption : ViewModelBase
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

        public DropDownMenuOption(string name)
        {
            DisplayName = name;
        }
    }
}
