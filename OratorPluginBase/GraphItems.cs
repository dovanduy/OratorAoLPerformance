using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Controls;
using System.ComponentModel;
using ProtoBuf;

using OratorCommonUtilities;

namespace OratorPluginBase
{
    public delegate void RangeChangedEventHandler(object sender, RangeChangedEventArgs e);

    public class Range
    {
        public DateTime StartPosition { get; set; }
        public DateTime EndPosition { get; set; }

        public Range(DateTime start, DateTime end)
        {
            this.StartPosition = start;
            this.EndPosition = end;
        }

        public Range() { }
    }

    public class SelectionLabel
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string Unit { get; set; }
        public bool Primary { get; set; }

        public SelectionLabel(string name, object value)
        {
            Name = name;
            Value = value;
            Primary = true;
            Unit = "";
        }

        public SelectionLabel(string name, object value, string unit, bool primary)
        {
            Name = name;
            Value = value;
            Primary = primary;
            Unit = unit;
        }
    }

    [ProtoContract]
    public class MultiSeriesGraphItem : GraphItemBase
    {
        [ProtoMember(1)]
        public OratorSeriesCollection SeriesCollection { get; set; }

        public override bool Available
        {
            get
            {
                bool result = false; 
                foreach (OratorSeries series in SeriesCollection)
	            {
		            if (series.Points.Count > 0)
	                {
		                result = true;
	                }
	            }
                return result;
            }          
        }

        public MultiSeriesGraphItem()
        {
            SeriesCollection = new OratorSeriesCollection();
        }
    }

    [ProtoContract]
    public class SingleSeriesGraphItem : GraphItemBase
    {
        [ProtoMember(1)]
        public OratorSeries Series { get; set; }

        public override bool Available
        {
            get
            {
                if (this.Series.Points.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public SingleSeriesGraphItem()
        {
            Series = new OratorSeries();
        }
    }

    [ProtoContract, ProtoInclude(1000, typeof(MultiSeriesGraphItem)), ProtoInclude(1001, typeof(SingleSeriesGraphItem))]    
    public abstract class GraphItemBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event RangeChangedEventHandler SelectionChangedEvent;
        public event EventHandler SelectionChangingEventStarted;
        public event RangeChangedEventHandler ZoomChangedEvent;

        private static object _locker = new object();

        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public string Shortname { get; set; }
        [ProtoMember(3)]
        public string Description { get; set; }

        [ProtoMember(5)]
        public Axis XAxis { get; set; }
        [ProtoMember(6)]
        public Axis YAxis { get; set; }
        [ProtoMember(7)]
        public ObservableCollection<OratorStripline> Striplines { get; set; }
        [ProtoMember(8)]
        public bool AnnotationOnPeakValue { get; set; }
        [ProtoMember(9)]
        public string AnnotationUnitType { get; set; }
        [ProtoMember(10)]
        [DefaultValue(false)]
        public bool AnnotationOnTroughValue { get; set; }
        [ProtoMember(11)]
        [DefaultValue(false)]
        public bool AnnotationOnGraphEndValue { get; set; }

        public ObservableCollection<SelectionLabel> SelectionLabels { get; set; }
        public bool SourceFilesAvailable { get; set; }
        public List<CommandViewModel> ContextMenuCommands { get; set; }
        public List<CheckableCommandViewModel> DropDownMenuOptions { get; set; }
        public bool SourceFilesActivityOngoing { get; set; }
        public List<ViewModelBase> SidePanels { get; set; }

        private bool _active;
        public bool Active
        {
            get { return _active; }
            set
            {
                lock (_locker)
                {
                    _active = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Active"));
                    }      
                }
            }
        }
        
        public abstract bool Available
        {
            get;
        }

        protected Range ActiveSelection;
      
        public GraphItemBase()
        {
            YAxis = new Axis();            
            XAxis = new Axis();
            XAxis.Type = UnitType.Time;
            XAxis.Title = "Time";
            Striplines = new ObservableCollection<OratorStripline>();
            ContextMenuCommands = new List<CommandViewModel>();
            DropDownMenuOptions = new List<CheckableCommandViewModel>();
            SelectionLabels = new ObservableCollection<SelectionLabel>();
            SidePanels = new List<ViewModelBase>();            
        }

        public void SelectionCleared()
        {
            ActiveSelection = null;
            OnSelectionCleared();
        }

        public void SelectionChanged(Range range)
        {
            ActiveSelection = range;
            OnSelectionChanged(range);            
        }

        public void SelectionChangingStarted()
        {
            ActiveSelection = null;
            OnSelectionChangingStarting();
        }

        public void ZoomChanged(Range range)   
        {
            if (this.ZoomChangedEvent != null) ZoomChangedEvent(this, new RangeChangedEventArgs() { NewRange = range });
        }
    
        protected virtual void OnSelectionChanged(Range range)
        {
            if (SelectionChangedEvent != null)
            {
                RangeChangedEventArgs args = new RangeChangedEventArgs { NewRange = range };
                this.SelectionChangedEvent(this, args);
            }
        }        
        protected virtual void OnSelectionCleared()
        {
            if (SelectionChangedEvent != null)
            {
                this.SelectionChangedEvent(this, new RangeChangedEventArgs());
            }
        }
        protected virtual void OnSelectionChangingStarting()
        {
            if (SelectionChangingEventStarted != null)
	        {
                this.SelectionChangingEventStarted(this, new EventArgs());
	        }
        }

        public virtual void OnMouseOver(object details)
        {
        }
    }

    public class RangeChangedEventArgs : EventArgs
    {
        public Range NewRange { get; set; }
    }

    [ProtoContract]
    public class Axis
    {
        [ProtoMember(1)]
        public string Title { get; set; }
        [ProtoMember(2)]
        public UnitType Type { get; set; }
        [ProtoMember(3)]
        public AxisScaleType ScaleType { get; set; }
    }

    [ProtoContract]
    public enum UnitType
    {
        Time,
        //Kilobytes,
        Megabytes,
        Percentage,
        //Double,
        TimeInterval,
        //Kilobytes_per_second,
        Integer        
    };

    [ProtoContract]
    public enum AxisScaleType
    {
        Linear = 0,
        Logarithmic = 1,
    };


    [ProtoContract]
    public class OratorStripline
    {
        [ProtoMember(1)]
        public SerializableColor StripLineColor { get; set; }       
        [ProtoMember(2)]
        public double XAxisLocation { get; set; }
        [ProtoMember(3)]
        public double YAxisLocation { get; set; }
        [ProtoMember(4)]
        public string Text { get; set; }
        [ProtoMember(5)]
        public RenderLineStyle LineStyle { get; set; }
        [ProtoMember(6)]
        public bool Permanent { get; set; }

        public OratorStripline()
        {
            StripLineColor = new SerializableColor();
            LineStyle = new RenderLineStyle();
        }
    }

    [ProtoContract]
    public enum RenderLineStyle
    {
        Solid,
        Dash
    }

    [ProtoContract]
    public class DataPointStyle
    {
        [ProtoMember(1)]
        public double MarkerSize { get; set; }
        [ProtoMember(2)]
        public SerializableColor MarkerColor { get; set; }
        [ProtoMember(3)]
        public SerializableColor MarkerBorderColor { get; set; }
        [ProtoMember(4)]
        public double MarkerBorderWidth { get; set; }

        public DataPointStyle()
        {
            MarkerColor = new SerializableColor();
            MarkerBorderColor = new SerializableColor();
        }
    }
}
