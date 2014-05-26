﻿using System;
using System.Collections.Generic;
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

namespace Profiler.Analyzer.Performance.Views
{
	/// <summary>
	/// Interaction logic for OpentimeView.xaml
	/// </summary>
	public partial class FramerateMonitorView : UserControl
	{

         /// <summary>
        /// Specify which files we want to read from 
        /// </summary>
        String[] fileNames = new String[] { "GBPtoUSD.cvs", "GBPtoEUR.cvs", "EURtoUSD.cvs" };

        /// <summary>
        /// Specify the name of each dataSeries
        /// </summary>
        String[] legendNames = new String[] { "£ vs $", "£ vs €", "€ vs $" };

        public FramerateMonitorView()
        {
            InitializeComponent();

            ////Read each file, create DataSeries then add to the chart
            //for (int i = 0; i < fileNames.Length; i++)
            //{
            //    CreateLineSeries(fileNames[i], legendNames[i]);
            //}
        }

        private void CreateLineSeries(string filename, string legendName)
        {
            //Create the LineSeries, then add to the chart
            LineSeries lineSeries = new LineSeries();
            lineSeries.DataSeries = GenerateDataSeries(filename, legendName);
            lineSeries.LineStrokeThickness = 1.5;
            chart.Series.Add(lineSeries);
        }

        private IDataSeries GenerateDataSeries(string filename, string legendName)
        {
            //Create a data series with the appropriate name
            var series = new DataSeries<DateTime, float>(legendName);

            using (StreamReader streamReader = new StreamReader(Helpers.GetApplicationResourceStream("Opentime/Data/" + filename).Stream))
            {
                float rebaseValue = 0;

                while (streamReader.Peek() >= 0)
                {
                    string line = streamReader.ReadLine();
                    string[] parts = line.Split(',');

                    //the files are formatted like so: "DD/MM/YYYY,VALUE"
                    String[] dateParts = parts[0].Split('/');
                    DateTime time = new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[0]), int.Parse(dateParts[1]));
                    float rate = float.Parse(parts[1]);

                    //If it's the first data point, work out the rebase value
                    if (rebaseValue == 0)
                    {
                        rebaseValue = 100 / rate;
                    }

                    //rebase value to 0
                    rate = (rate * rebaseValue) - 100;

                    series.Add(new DataPoint<DateTime, float>(time, rate));
                }

            }

            return series;
        }
	}
}