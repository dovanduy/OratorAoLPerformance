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
	public partial class OpenTimeView : UserControl
	{
        public OpenTimeView()
        {
            InitializeComponent();

            //CourseList allCourses = Resources["Courses"] as CourseList;
            //foreach (Course element in allCourses)
            //{
            //    element.Courses = allCourses;
            //}

            //add handlers to change cursor when mouse enters and leaves the bars
            //(CourseInformation.Series[0] as BarSeries).MouseEnter += new MouseEventHandler(AdditionalInformationExample_MouseEnter);
            //(CourseInformation.Series[0] as BarSeries).MouseLeave += new MouseEventHandler(AdditionalInformationExample_MouseLeave);
        }

        ///// <summary>
        ///// Mouse has left one of the bar datapoints - set cursor to arrow
        ///// </summary>
        //void AdditionalInformationExample_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    this.Cursor = Cursors.Arrow;
        //}

        ///// <summary>
        ///// Mouse has entered one of the bar datapoints - set cursor to hand
        ///// </summary>
        //void AdditionalInformationExample_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    this.Cursor = Cursors.Hand;
        //}

        ///// <summary>
        ///// The accept changes button has been clicked
        ///// </summary>
        //private void AcceptChanges_Click(object sender, RoutedEventArgs e)
        //{
        //    CourseInformation.Focus();  // Forces the text box to give up focus and hence applies the changes
        //}
	}

    // Data Model

    /// <summary>
    /// A list of courses
    /// </summary>
    public class CourseList : List<Course> { }

    /// <summary>
    /// A course object
    /// </summary>
    public class Course : INotifyPropertyChanged
    {
        /// <summary>
        /// The private backing variable for Score
        /// </summary>
        private double score;

        /// <summary>
        /// The private backing variable for CourseName
        /// </summary>
        private string courseName;

        public CourseList Courses
        {
            get;
            set;
        }


        /// <summary>
        /// The name of the course
        /// </summary>
        public string CourseName
        {
            get { return courseName; }

            set
            {
                if (Courses != null && Courses.Any(p => p.CourseName == value))
                {
                    throw new NotSupportedException(string.Format("Course with the name: {0} already exists", value));
                }

                courseName = value;
                OnPropertyChanged("CourseName");
            }
        }

        /// <summary>
        /// A string description of the timetable
        /// </summary>
        public string Timetable { get; set; }

        /// <summary>
        /// The lecturer's name
        /// </summary>
        public string Lecturer { get; set; }

        /// <summary>
        /// The average score for people on this course
        /// </summary>
        public double Score
        {
            get { return score; }
            set
            {
                //throw exception if score not valid percent value
                if (value < 0 || value > 100)
                {
                    //TextBox binding sets ValidatesOnExceptions in XAML to display error message
                    throw new NotSupportedException("Score must be between 0 and 100%");
                }

                else
                {
                    score = value; OnPropertyChanged("Score");
                }
            }
        }

        /// <summary>
        /// Private helper for notifying that a particular property has changed
        /// </summary>
        private void OnPropertyChanged(string inName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(inName));
        }

        /// <summary>
        /// Event to notify if the property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Public override of ToString for category axis
        /// </summary>
        public override string ToString()
        {
            return CourseName.ToString(); ;
        }
    }
}