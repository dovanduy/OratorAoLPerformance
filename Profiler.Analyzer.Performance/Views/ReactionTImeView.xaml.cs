using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Visiblox.Charts;
using System.Linq;

namespace Profiler.Analyzer.Performance.Views
{
	/// <summary>
	/// Interaction logic for OpentimeView.xaml
	/// </summary>
	public partial class ReactionTimeView : UserControl
	{
        public ReactionTimeView()
        {
            InitializeComponent();

            var series = MainChart.Series.First() as IChartMultipleSeries;

            foreach (BarSeries barSeries in series.Series)
            {
                barSeries.HighlightedStyle = barSeries.SelectedStyle;
                barSeries.MouseEnter += new MouseEventHandler(barSeries_MouseEnter);
                barSeries.MouseLeave += new MouseEventHandler(barSeries_MouseLeave);
            }
        }


        /// <summary>
        /// Mouse has entered one of the bar datapoints - set cursor to hand
        /// </summary>
        void barSeries_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Mouse has left one of the bar datapoints - set cursor to arrow
        /// </summary>
        void barSeries_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void Stacked_Checked(object sender, RoutedEventArgs e)
        {
            if (MainChart == null)
                return;

            var series = MainChart.Series.First() as StackedBarSeries;
            series.StackingMode = StackingMode.Normal;

            MainChart.XAxis.Title = "Dwellings built (thousands)";
        }

        private void OneHundredStacked_Checked(object sender, RoutedEventArgs e)
        {
            if (MainChart == null)
                return;

            var series = MainChart.Series.First() as StackedBarSeries;
            series.StackingMode = StackingMode.HundredPercent;

            MainChart.XAxis.Title = "Dwellings built (%)";
        }
    }

    // Data Model

    public class HousingStockNumberList : List<HousingStockNumber> { }

    public class HousingStockNumber
    {
        public String Period { get; set; }

        public double ThousandsOfDwellings { get; set; }
    }
}