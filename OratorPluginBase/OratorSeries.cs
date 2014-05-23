using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;
using System.Collections.ObjectModel;

using ProtoBuf;

namespace OratorPluginBase
{
    [ProtoContract]
    public class OratorSeries
    {
        public OratorSeries()
        {
            Points = new List<OratorDataPoint>();
            PointsMaximum = 50000;
            EliminateConsecutiveDuplicatePoints = true;
            MouseoverEventsEnabled = false;
            DataPointCombiningMode = CombiningModeOptions.TroughAndPeak;

            Rendering = new RenderingInfo();
            Rendering.SeriesType = SeriesRenderType.StaircaseSeries;
            Rendering.LineWidth = 1;
            Rendering.LineStyle = RenderLineStyle.Solid;

            MinValue = double.NaN;
            MaxValue = double.NaN;            
        }

        [ProtoMember(1, OverwriteList=true)]
        public List<OratorDataPoint> Points {get; set;}
        [ProtoMember(2)]
        public int PointsMaximum { get; set; }
        [ProtoMember(3)]
        public bool EliminateConsecutiveDuplicatePoints { get; set; }
        [ProtoMember(4)]
        public RenderingInfo Rendering { get; set; }
        [ProtoMember(5)]
        public bool MouseoverEventsEnabled{ get; set; }
        [ProtoMember(6)]
        public CombiningModeOptions DataPointCombiningMode { get; set; }
        [ProtoMember(7)]
        public string Name { get; set; }
        [ProtoMember(8)]
        public double MinValue { get; set; }
        [ProtoMember(9)]
        public double MaxValue { get; set; }
        [ProtoMember(10)]
        public bool TrackballEnabled { get; set; }

        [ProtoMember(11)]
        public DateTime BeginningTime { get; set; }
        [ProtoMember(12)]
        public DateTime EndTime { get; set; }

        [ProtoMember(13)]
        public bool Active { get; set; }

        public double Average
        {
            get
            {
                if (Points.Count > 0)
                {
                    return Points.Average(x => x.Y1);
                }
                else return 0.0;                
            }
        }

        public double PeakValue
        {
            get
            {
                if (Points.Count > 0)
                {
                    return Points.Max(x => x.Y1);
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public double TroughValue
        {
            get
            {
                if (Points.Count > 0)
                {
                    return Points.Min(x => x.Y1);
                }
                else
                {
                    return Double.NaN;
                }
            }
        }

        public void Add(OratorDataPoint point)
        {
            if (point != null)
            {
                if (point.X > 0 && Points.Count > 0 && EliminateConsecutiveDuplicatePoints == true)
                {
                    OratorDataPoint last = Points[Points.Count - 1];

                    if (point.X >= last.X)
                    {
                        if (Points.Count > 1)
                        {
                            OratorDataPoint previous = Points[Points.Count - 2];
                            if ((last.Y1 == previous.Y1 && last.Y1 == point.Y1) || last.X == point.X)
                            {
                                Points.RemoveAt(Points.Count - 1);
                            }
                        }

                        Points.Add(point);
                    }
                }
                else
                {
                    Points.Add(point);
                }

                if (Points.Count > PointsMaximum * 4)
                {
                    Trim();
                }
            }
        }

        public void Trim()
        {
            if (Points.Count > PointsMaximum)
            {
                List<OratorDataPoint> resultList = new List<OratorDataPoint>();

                double minTime = 0.0;
                if (BeginningTime != DateTime.MinValue)
                {
                    minTime = BeginningTime.ToOADate();    
                }
                else
                {
                    minTime = Points[0].X;
                }

                double maxTime = 0.0;
                if (EndTime != DateTime.MinValue)
                {
                    maxTime = EndTime.ToOADate();
                }
                else
                {
                    maxTime = Points[Points.Count - 1].X;
                }                

                double timeInterval = (maxTime - minTime) / PointsMaximum;
                double peakValue = 0;
                double peakValueTime = 0;
                double troughValue = Points[0].Y1;
                double troughValueTime = Points[0].X;
                double timePeriod = minTime + timeInterval;
                double runningTotal = 0.0;
                int pointCount = 0;
                DateTime startTime = DateTime.FromOADate(minTime);
                DateTime endTime = DateTime.FromOADate(maxTime);

                //switch (DataPointCombiningMode)
                //{
                //    case CombiningModeOptions.TroughAndPeak:
                //        foreach (OratorDataPoint dataPoint in Points)
                //        {
                //            if (dataPoint.Y1 >= peakValue)
                //            {
                //                peakValue = dataPoint.Y1;
                //                peakValueTime = dataPoint.X;
                //            }
                //            else if (dataPoint.Y1 <= troughValue)
                //            {
                //                troughValue = dataPoint.Y1;
                //                troughValueTime = dataPoint.X;
                //            }
                //            if (dataPoint.X >= timePeriod)
                //            {
                //                if (troughValueTime < peakValueTime)
                //                {
                //                    resultList.Add(new OratorDataPoint(troughValueTime, troughValue));
                //                    resultList.Add(new OratorDataPoint(peakValueTime, peakValue));
                //                }
                //                else
                //                {
                //                    resultList.Add(new OratorDataPoint(peakValueTime, peakValue));
                //                    resultList.Add(new OratorDataPoint(troughValueTime, troughValue));
                //                }

                //                peakValue = 0;
                //                troughValue = dataPoint.Y1;
                //                troughValueTime = dataPoint.X;
                //                timePeriod += timeInterval;
                //            }
                //        }

                //        //Last point
                //        if (troughValueTime < peakValueTime)
                //        {
                //            resultList.Add(new OratorDataPoint(troughValueTime, troughValue));
                //            resultList.Add(new OratorDataPoint(peakValueTime, peakValue));
                //        }
                //        else
                //        {
                //            resultList.Add(new OratorDataPoint(peakValueTime, peakValue));
                //            resultList.Add(new OratorDataPoint(troughValueTime, troughValue));
                //        }

                //        break;

                //    case CombiningModeOptions.Average:
                //        foreach (OratorDataPoint dataPoint in Points)
                //        {
                //            pointCount++;
                //            runningTotal += dataPoint.Y1;
                //            if (dataPoint.X >= timePeriod)
                //            {
                //                double timeStamp = timePeriod - (timeInterval / 2);
                //                resultList.Add(new OratorDataPoint(timeStamp, runningTotal / (double)pointCount));
                //                pointCount = 0;
                //                runningTotal = 0.0;
                //                timePeriod += timeInterval;
                //            }
                //        }
                //        break;
                //}

                //foreach (OratorDataPoint dataPoint in Points)
                //{
                //    switch (DataPointCombiningMode)
                //    {
                //        case CombiningModeOptions.TroughAndPeak:
                //            if (dataPoint.Y1 >= peakValue)
                //            {
                //                peakValue = dataPoint.Y1;
                //                peakValueTime = dataPoint.X;
                //            }
                //            else if (dataPoint.Y1 <= troughValue)
                //            {
                //                troughValue = dataPoint.Y1;
                //                troughValueTime = dataPoint.X;
                //            }
                //            if (dataPoint.X >= timePeriod)
                //            {
                //                if (troughValueTime < peakValueTime)
                //                {
                //                    resultList.Add(new OratorDataPoint(troughValueTime, troughValue));
                //                    resultList.Add(new OratorDataPoint(peakValueTime, peakValue));
                //                }
                //                else
                //                {
                //                    resultList.Add(new OratorDataPoint(peakValueTime, peakValue));
                //                    resultList.Add(new OratorDataPoint(troughValueTime, troughValue));
                //                }

                //                peakValue = 0;
                //                troughValue = dataPoint.Y1;
                //                troughValueTime = dataPoint.X;
                //                timePeriod += timeInterval;
                //            }
                //            break;

                //        case CombiningModeOptions.Average:
                //            pointCount++;
                //            runningTotal += dataPoint.Y1;
                //            if (dataPoint.X >= timePeriod)
                //            {
                //                double timeStamp = timePeriod - (timeInterval / 2);
                //                resultList.Add(new OratorDataPoint(timeStamp, runningTotal / (double)pointCount));
                //                pointCount = 0;
                //                runningTotal = 0.0;
                //                timePeriod += timeInterval;
                //            }
                //            break;
                //    }
                //}

                #region fix the Bug70964 by Zhao xingman
                switch (DataPointCombiningMode)
                {
                    case CombiningModeOptions.TroughAndPeak:
                        foreach (OratorDataPoint dataPoint in Points)
                        {
                            if (dataPoint.Y1 >= peakValue)
                            {
                                peakValue = dataPoint.Y1;
                                peakValueTime = dataPoint.X;
                            }
                            else if (dataPoint.Y1 <= troughValue)
                            {
                                troughValue = dataPoint.Y1;
                                troughValueTime = dataPoint.X;
                            }
                            if (dataPoint.X >= timePeriod)
                            {
                                if (troughValueTime < peakValueTime)
                                {
                                    resultList.Add(new OratorDataPoint(troughValueTime, troughValue));
                                    resultList.Add(new OratorDataPoint(peakValueTime, peakValue));
                                }
                                else
                                {
                                    resultList.Add(new OratorDataPoint(peakValueTime, peakValue));
                                    resultList.Add(new OratorDataPoint(troughValueTime, troughValue));
                                }

                                peakValue = 0;
                                troughValue = dataPoint.Y1;
                                troughValueTime = dataPoint.X;
                                timePeriod += timeInterval;
                            }
                        }
                        break;
                    case CombiningModeOptions.Average:
                        int pointIndex = 0;
                        int pointNumber = 1;
                        while (pointNumber <= PointsMaximum)
                        {
                            timePeriod = minTime + timeInterval * pointNumber;
                            pointCount = 0;
                            runningTotal = 0.0;
                            for (int i = pointIndex; i < Points.Count; i++)
                            {
                                OratorDataPoint dataPoint = Points[i];
                                if (dataPoint.X <= timePeriod)
                                {
                                    pointCount++;
                                    runningTotal += dataPoint.Y1;
                                }
                                else
                                {
                                    double timeStamp = timePeriod - (timeInterval / 2);
                                    double y1 = 0;
                                    if (pointCount > 0)
                                    {
                                        y1 = runningTotal / (double)pointCount;
                                    }
                                    resultList.Add(new OratorDataPoint(timeStamp, y1));
                                    pointIndex = i;
                                    break;
                                }
                            }

                            if (pointIndex >= Points.Count)
                            {
                                double timeStamp = timePeriod - (timeInterval / 2);
                                resultList.Add(new OratorDataPoint(timeStamp, 0d));
                            }
                            pointNumber++;
                        }
                        break;
                }
                #endregion

                Points = resultList; 
            }
        }

        [ProtoContract]
        public class RenderingInfo
        {
            [ProtoMember(1)]
            public SeriesRenderType SeriesType { get; set; }
            [ProtoMember(2)]
            public RenderLineStyle LineStyle { get; set; }
            [ProtoMember(3)]
            public int LineWidth { get; set; }
            [ProtoMember(4)]
            public SerializableColor PreferredColor { get; set; }
            [ProtoMember(5)]
            public DataPointStyle DefaultDatapointStyle { get; set; }
            [ProtoMember(6)]
            public DataPointStyle SelectedDatapointStyle { get; set; }
            [ProtoMember(7)]
            public bool ShowPoints { get; set; }
            [ProtoMember(8)]
            public bool ShowArea { get; set; }

            public RenderingInfo()
            {
                SeriesType = SeriesRenderType.FastLineSeries;
                LineStyle = RenderLineStyle.Solid;
                PreferredColor = new SerializableColor();
                DefaultDatapointStyle = new DataPointStyle();
                SelectedDatapointStyle = new DataPointStyle();
            }
        }
    }

    [ProtoContract]
    public class OratorSeriesCollection : List<OratorSeries>
    {
        [ProtoMember(1)]
        public double MinValue { get; set; }
        [ProtoMember(2)]
        public double MaxValue { get; set; }
        [ProtoMember(3)]
        public MultiSeriesType Type { get; set; }
    }

    [ProtoContract]
    public enum MultiSeriesType
    {
        StackedArea,
        YapasPoolSpecial
    };

    [ProtoContract]
    public enum SeriesRenderType
    {
        LineSeries,
        StaircaseSeries,
        SplineSeries,
        DiscontinuousLineSeries,
        FastLineSeries,
        FastStaircaseSeries,
    };

    [ProtoContract]
    public enum CombiningModeOptions
    {
        Average,
        TroughAndPeak,
    };
}
