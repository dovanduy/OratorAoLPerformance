using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using ProtoBuf;

namespace OratorCommonUtilities
{
    [DataContract]
    [ProtoContract]
    public class OratorReport
    {
        public OratorReport()
        {
            DynamicDoubleValues = new Dictionary<string, double>();
        }

        [DataMember(Order = 1)]
        [ProtoMember(1)]
        public string FileFullPath { get; set; }
        public Uri FileAddress
        {
            get
            {
                return new Uri(FileFullPath);
            }
        }
        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(FileFullPath);
            }
        }

        [DataMember(Order = 11)]
        [ProtoMember(11)]
        public DateTime Created { get; set; }

        [DataMember(Order = 2)]
        [ProtoMember(2)]
        public DateTime Modified { get; set; }

        [DataMember(Order = 3)]
        [ProtoMember(3)]
        public string Label { get; set; }

        [DataMember(Order = 4)]
        [ProtoMember(4)]
        public string OratorVersion { get; set; }

        [DataMember(Order = 5)]
        [ProtoMember(5)]
        public bool FileAvailable { get; set; }

        [DataMember(Order = 6)]
        [ProtoMember(6)]
        public string MCUSWVersion { get; set; }

        [DataMember(Order = 7)]
        [ProtoMember(7)]
        public List<string> TestCases { get; set; }

        [DataMember(Order = 8)]
        [ProtoMember(8)]
        public bool ResetTerminated { get; set; }

        [DataMember(Order = 9)]
        [ProtoMember(9)]
        public bool FromDeviceBootup { get; set; }

        [DataMember(Order = 10)]
        [ProtoMember(10)]
        public List<string> LogFiles { get; set; }
        
        //--- Plugin items ---
        [DataMember(Order = 100)]
        [ProtoMember(100)]
        public double TotalPeak { get; set; }

        [DataMember(Order = 101)]
        [ProtoMember(101)]
        public double NOSHeapPeak { get; set; }

        [DataMember(Order = 102)]
        [ProtoMember(102)]
        public double NOSHeapFragmentationMinimum { get; set; }

        [DataMember(Order = 103)]
        [ProtoMember(103)]
        public double NOSHeapFragmentationEndValue { get; set; }

        [DataMember(Order = 104)]
        [ProtoMember(104)]
        public double NOSPoolPeak { get; set; }

        [DataMember(Order = 105)]
        [ProtoMember(105)]
        public double YapasHeapPeak { get; set; }

        [DataMember(Order = 106)]
        [ProtoMember(106)]
        public double GraphicsCacheSizePeak { get; set; }

        [DataMember(Order = 107)]
        [ProtoMember(107)]
        public double SXCellsUsagePeak { get; set; }

        [DataMember(Order = 108)]
        [ProtoMember(108)]
        public double SXCellsSizeUsagePeak { get; set; }

        [DataMember(Order = 109)]
        [ProtoMember(109)]
        public double JavaUsagePeak { get; set; }

        [DataMember(Order = 110)]
        [ProtoMember(110)]
        public double MemoryLeaks { get; set; }

        [DataMember(Order = 111)]
        [ProtoMember(111)]
        public double VMMUsagePeak { get; set; }

        [DataMember(Order = 200)]
        [ProtoMember(200)]
        public Dictionary<string, double> DynamicDoubleValues { get; set; }
    }

    [DataContract]
    [ProtoContract]
    public class ReportItemDescriptor
    {
        [DataMember(Order = 1)]
        [ProtoMember(1)]
        public string ItemPropertyName { get; set; }

        [DataMember(Order = 2)]
        [ProtoMember(2)]
        public string Description { get; set; }

        [DataMember(Order = 3)]
        [ProtoMember(3)]
        public string DisplayName { get; set; }

        [DataMember(Order = 4)]
        [ProtoMember(4)]
        public string Unit { get; set; }

        [DataMember(Order = 5)]
        [ProtoMember(5)]
        public Type ValueType { get; set; }

        [DataMember(Order = 6)]
        [ProtoMember(6)]
        public bool Hide { get; set; }
    }
}
