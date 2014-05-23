using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

using ProtoBuf;

namespace OratorPluginBase
{
    [DataContract]
    [ProtoContract]
    public class SummaryReport
    {
        [DataMember(Order = 0)]
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

        [DataMember(Order = 1)]
        [ProtoMember(2)]
        public DateTime TimeStamp { get; set; }

        [DataMember(Order = 2)]
        [ProtoMember(3)]
        public string Label { get; set; }

        [DataMember(Order = 3)]
        [ProtoMember(4)]
        public Version OratorVersion { get; set; }

        [DataMember(Order = 4)]
        [ProtoMember(5)]
        public Dictionary<string, SummaryItem> Items { get; set; }

        [DataMember(Order = 5)]
        [ProtoMember(6)]
        public bool FileAvailable { get; set; }

        [DataMember(Order = 6)]
        [ProtoMember(7)]
        public string MCUSWVersion { get; set; }

        [DataMember(Order = 7)]
        [ProtoMember(8)]
        public string GraniteUseCase { get; set; }

        [DataMember(Order = 8)]
        [ProtoMember(9)]
        public bool FromDeviceBootup { get; set; }

        [DataMember(Order = 9)]
        [ProtoMember(10)]
        public bool ResetTerminated { get; set; }

        public SummaryReport()
        {
            Items = new Dictionary<string, SummaryItem>();
        }
    }

    [DataContractAttribute]
    [KnownType(typeof(SummaryItem<int>))]
    [KnownType(typeof(SummaryItem<double>))]
    [KnownType(typeof(SummaryItem<bool>))]
    [KnownType(typeof(SummaryItem<string>))]
    [KnownType(typeof(SummaryItem<DateTime>))]
    [ProtoContract]
    [ProtoInclude(1, typeof(SummaryItem<int>))]
    [ProtoInclude(2, typeof(SummaryItem<double>))]
    [ProtoInclude(3, typeof(SummaryItem<bool>))]
    [ProtoInclude(4, typeof(SummaryItem<string>))]
    [ProtoInclude(5, typeof(SummaryItem<DateTime>))]
    //...known types...
    public abstract class SummaryItem
    {
        public abstract object UntypedValue { get; set; }
        public static SummaryItem<T> Create<T>(T value)
        {
            return new SummaryItem<T> { Value = value };
        }
        public static SummaryItem CreateDynamic(object value)
        {
            Type type = value.GetType();
            switch (Type.GetTypeCode(value.GetType()))
            {
                // special cases
                case TypeCode.Int32: return Create((int)value);
                case TypeCode.Double: return Create((double)value);
                case TypeCode.Boolean: return Create((bool)value);
                case TypeCode.String: return Create((string)value);
                case TypeCode.DateTime: return Create((DateTime)value);
                // fallback in case we forget to add one, or it isn't a TypeCode
                default:
                    SummaryItem param = (SummaryItem)Activator.CreateInstance(
                        typeof(SummaryItem<>).MakeGenericType(type));
                    param.UntypedValue = value;
                    return param;
            }
        }
    }

    [DataContract]
    [ProtoContract]
    public sealed class SummaryItem<T> : SummaryItem
    {
        //[DataMember(Order = 0)]
        //[ProtoMember(1)]
        //public string Name { get; set; }

        [DataMember(Order = 1)]
        [ProtoMember(2)]
        public T Value { get; set; }
        public override object UntypedValue
        {
            get { return Value; }
            set { Value = (T)value; }
        }

        //public SummaryItem(T t)
        //{
        //    Value = t;
        //}

        //public SummaryItem()
        //{ }
    }

    [DataContract]
    [ProtoContract]
    public class SummaryItemDescriptor
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
    }
}
