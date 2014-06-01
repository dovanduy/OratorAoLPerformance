using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Profiler.Analyzer.Performance.ViewModels;
using Profiler.Analyzer.Performance.Views;

namespace Orator.AoL.MainWindow
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style AnchorableStyle
        {
            get;
            set;
        }

        public Style DocumentStyle
        {
            get;
            set;
        }

        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (item is SessionExplorerViewModel)
                return AnchorableStyle;

            if (item is PerformanceAnalyzerViewModel)
                return DocumentStyle;

            return base.SelectStyle(item, container);
        }
    }
}
