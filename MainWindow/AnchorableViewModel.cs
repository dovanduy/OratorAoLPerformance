using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OratorCommonUtilities;

namespace Orator.AoL.MainWindow
{
   public class AnchorableViewModel : ViewModelBase
    {
       public AnchorableViewModel()
       {
           DisplayName = string.Format("Anchorable Title {0}", DateTime.Now);
       }

    }
}
