/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using Profiler.Analyzer.Performance.ViewModels;
using Profiler.Analyzer.Performance.Views;

namespace Orator.AoL.MainWindow
{
    public class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        {

        }


        public DataTemplate AnchorableViewTemplate
        {
            get;
            set;
        }

        public DataTemplate DocumentViewTemplate
        {
            get;
            set;
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is SessionExplorerViewModel)
                return AnchorableViewTemplate;

            if (item is PerformanceAnalyzerViewModel)
                return DocumentViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
