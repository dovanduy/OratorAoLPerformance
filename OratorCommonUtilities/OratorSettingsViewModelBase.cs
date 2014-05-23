using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace OratorCommonUtilities
{
    public abstract class OratorSettingsViewModelBase : ViewModelBase
    {
        public abstract void Save();
        public abstract void Cancel();
        public bool IsPlugin { get; set; }
    }
}
