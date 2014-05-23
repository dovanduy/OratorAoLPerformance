using System;
using System.Windows.Input;

namespace OratorCommonUtilities
{
    public class CircularProgressBarViewModel : NotificationViewModelBase
    {
        private RelayCommand _sizeChangeCommand;
        public ICommand SizeChangeCommand
        {
            get { return _sizeChangeCommand ?? (_sizeChangeCommand = new RelayCommand(param => this.ChangeSize((object)param))); }
        }

        #region fields

        private double _angle;
        private double _centreX;
        private double _centreY;
        private double _radius;
        private double _innerRadius;
        private double _diameter;
        private double _percent;
        private double _holeSizeFactor = 0.75;

        private int _progressPercentage;
        private int _maximumValue = 100;
        private int _minimumValue = 0;        

        #endregion

        #region properties

        public double Percent
        {
            get { return _percent; }
            set { _percent = value; OnPropertyChanged("Percent"); }
        }

        public double Diameter
        {
            get { return _diameter; }
            set { _diameter = value; OnPropertyChanged("Diameter"); }
        }

        public double Radius
        {
            get { return _radius; }
            set { _radius = value; OnPropertyChanged("Radius"); }
        }

        public double InnerRadius
        {
            get { return _innerRadius; }
            set { _innerRadius = value; OnPropertyChanged("InnerRadius"); }
        }

        public double CentreX
        {
            get { return _centreX; }
            set { _centreX = value; OnPropertyChanged("CentreX"); }
        }

        public double CentreY
        {
            get { return _centreY; }
            set { _centreY = value; OnPropertyChanged("CentreY"); }
        }

        public double Angle
        {
            get { return _angle; }
            set { _angle = value; OnPropertyChanged("Angle"); }
        }

        public double HoleSizeFactor
        {
            get { return _holeSizeFactor; }
            set { _holeSizeFactor = value; ComputeViewModelProperties(); }
        }

        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set { _progressPercentage = value; ComputeViewModelProperties(); }
        }

        private int _duration;
        public int Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }

        private string _resultText;
        public string ResultText
        {
            get { return _resultText; }
            set
            {
                _resultText = value;
                OnPropertyChanged("ResultText");
            }
        }

        private bool _completed;
        public bool Completed
        {
            get { return _completed; }
            set
            {
                _completed = value;
                ProgressPercentage = 100;
                ResultText = "Completed";
                OnPropertyChanged("Completed");
            }
        }

        private bool _cancelled;
        public bool Cancelled
        {
            get { return _cancelled; }
            set
            {
                _cancelled = value;
                ResultText = "Cancelled";
                OnPropertyChanged("Cancelled");
            }
        }

        private bool _cancelRequested;
        public bool CancelRequested
        {
            get { return _cancelRequested; }
            set
            {
                _cancelRequested = value;
                OnPropertyChanged("CancelRequested");
            }
        }

        #endregion

        /// <summary>
        /// Re-computes the various properties that the elements in the template bind to.
        /// </summary>
        protected virtual void ComputeViewModelProperties()
        {
            Angle = (double)(ProgressPercentage - _minimumValue) * 360 / (double)(_maximumValue - _minimumValue);
            Radius = Math.Min(CentreX, CentreY);
            Diameter = Radius * 2;
            InnerRadius = Radius * HoleSizeFactor;
            Percent = Angle / 360;
        }   
     
        public void ChangeSize(object size)
        {
            var sizeArgs = size as Tuple<double, double>;
            if (sizeArgs != null)
            {
                CentreX = sizeArgs.Item1 / 2;
                CentreY = sizeArgs.Item2 / 2;
            }
        }

        public void RequestCancellation()
        {
            this.CancelRequested = true;
        }

        public CircularProgressBarViewModel(string notificationText)
        {
            this.NotificationText = notificationText;
        }
    }
}
