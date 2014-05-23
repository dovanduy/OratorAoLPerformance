using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using ProtoBuf;

namespace OratorPluginBase
{
    [ProtoContract]
    public class OratorDataPoint
    {
        [ProtoMember(1)]
        public double X { get; set; }
        [ProtoMember(2)]
        public double Y1 { get; set; }
        [ProtoMember(5)]
        public double Y2 { get; set; }
        [ProtoMember(3)]
        public string Line { get; set; }

        //[ProtoMember(4)]
        //private byte[] screenContentBytes;
        //public Bitmap ScreenContent
        //{
        //    get
        //    {
        //        ImageConverter converter = new ImageConverter();
        //        if (screenContentBytes != null)
        //        {
        //            Bitmap bm = null;
        //            try
        //            {
        //                 bm = (Bitmap)converter.ConvertFrom(screenContentBytes);
        //            }
        //            catch (Exception ex)
        //            {
        //                string msg = ex.Message;
        //            }
        //            return bm;
        //            return (Bitmap)converter.ConvertFrom(screenContentBytes);
        //        }
        //        else
        //        {
        //            return null;
        //        }                
        //    }
        //    set
        //    {
        //        ImageConverter converter = new ImageConverter();
        //        screenContentBytes = (byte[])converter.ConvertTo(value, typeof(byte[]));
        //    }
        //}

        [ProtoMember(6)]
        public OratorFrameRateGraph FrameRateGraph { get; set; }

        public object Tag { get; set; }

        public OratorDataPoint()
        {
        }

        public OratorDataPoint(double x, double y1)
        {
            X = x;
            Y1 = y1;
        }

        public OratorDataPoint(double x, double y1, double y2)
        {
            X = x;
            Y1 = y1;
            Y2 = y2;
        }

        public OratorDataPoint(double x, double y1, string line)
        {
            X = x;
            Y1 = y1;
            Line = line;
        }
    }

    [ProtoContract]
    public class OratorFrameRateGraph
    {
        
        [ProtoMember(1)]
        public FrameRateGraphDump DumpType { get; set; }
        [ProtoMember(2)]
        private byte[] screenContentBytes;
        public Bitmap ScreenContent
        {
            get
            {
                ImageConverter converter = new ImageConverter();
                if (screenContentBytes != null)
                {
                    return (Bitmap)converter.ConvertFrom(screenContentBytes);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                ImageConverter converter = new ImageConverter();
                screenContentBytes = (byte[])converter.ConvertTo(value, typeof(byte[]));
            }
        }

        [ProtoMember(3)]
        public int X { get; set; }

        [ProtoMember(4)]
        public int Y { get; set; }

        [ProtoMember(5)]
        public int Width { get; set; }

        [ProtoMember(6)]
        public int Height { get; set; }
    }

     [ProtoContract]
     public enum FrameRateGraphDump
     {
         None,
         Known,
         Unknown
     }
}
