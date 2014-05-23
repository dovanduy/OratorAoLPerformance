using Managed.Adb.Logs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Profiler.LogManager.Models
{
    public class GcEventContainer : EventContainer
    {
        private long actualSize;
        private long allowedSize;
        private long bytesAllocated;
        private long bytesFreed;
        private long dlmallocFootprint;
        private long externalBytesAllocated;
        private long externalLimit;
        public const int GC_EVENT_TAG = 0x4e21;
        private long gcTime;
        private long mallinfoTotalAllocatedSpace;
        private long objectsAllocated;
        private long objectsFreed;
        private string processId;
        private long softLimit;
        private long zActualSize;
        private long zAllowedSize;
        private long zBytesAllocated;
        private long zObjectsAllocated;

        public GcEventContainer(LogEntry entry, int tag, string tagName, object data) : base(entry, tag, tagName, data)
        {
            this.Init(data);
        }

        public GcEventContainer(int tag, string tagName, int pid, int tid, int sec, int nsec, object data) : base(tag, tagName, pid, tid, sec, nsec, data)
        {
            this.Init(data);
        }

        private static long Float12ToInt(int f12)
        {
            return (long) ((f12 & 0x1ff) << ((f12 >> 9) * 4));
        }

        public override object GetValue(int valueIndex)
        {
            if (valueIndex == 0)
            {
                return this.processId;
            }
            try
            {
                return this.GetValueAsLong(valueIndex);
            }
            catch (Exception)
            {
            }
            return null;
        }

        public override double GetValueAsDouble(int valueIndex)
        {
            return (double) this.GetValueAsLong(valueIndex);
        }

        private long GetValueAsLong(int valueIndex)
        {
            switch (valueIndex)
            {
                case 0:
                    throw new InvalidCastException();

                case 1:
                    return this.gcTime;

                case 2:
                    return this.objectsFreed;

                case 3:
                    return this.bytesFreed;

                case 4:
                    return this.softLimit;

                case 5:
                    return this.actualSize;

                case 6:
                    return this.allowedSize;

                case 7:
                    return this.objectsAllocated;

                case 8:
                    return this.bytesAllocated;

                case 9:
                    return (this.actualSize - this.zActualSize);

                case 10:
                    return (this.allowedSize - this.zAllowedSize);

                case 11:
                    return (this.objectsAllocated - this.zObjectsAllocated);

                case 12:
                    return (this.bytesAllocated - this.zBytesAllocated);

                case 13:
                    return this.zActualSize;

                case 14:
                    return this.zAllowedSize;

                case 15:
                    return this.zObjectsAllocated;

                case 0x10:
                    return this.zBytesAllocated;

                case 0x11:
                    return this.externalLimit;

                case 0x12:
                    return this.externalBytesAllocated;

                case 0x13:
                    return this.dlmallocFootprint;

                case 20:
                    return this.mallinfoTotalAllocatedSpace;
            }
            throw new ArgumentOutOfRangeException();
        }

        public override string GetValueAsString(int valueIndex)
        {
            if (valueIndex == 0)
            {
                return this.processId;
            }
            try
            {
                return this.GetValueAsLong(valueIndex).ToString();
            }
            catch (Exception)
            {
            }
            throw new ArgumentOutOfRangeException();
        }

        private void Init(object data)
        {
            if (data is object[])
            {
                object[] objArray = (object[]) data;
                for (int i = 0; i < objArray.Length; i++)
                {
                    if (objArray[i] is long)
                    {
                        this.ParseDvmHeapInfo((long) objArray[i], i);
                    }
                }
            }
        }

        protected override string ObjectToString(object obj)
        {
            List<object> list = new List<object> {
                this.processId
            };
            for (int i = 1; i < 20; i++)
            {
                list.Add(this.GetValueAsLong(i));
            }
            return base.ObjectToString(list.ToArray());
        }

        private void ParseDvmHeapInfo(long data, int index)
        {
            switch (index)
            {
                case 0:
                {
                    this.gcTime = Float12ToInt((int) ((data >> 12) & 0xfffL));
                    this.bytesFreed = Float12ToInt((int) (data & 0xfffL));
                    byte[] bytes = Put64bitsToArray(data);
                    this.processId = Encoding.Default.GetString(bytes, 0, 5);
                    return;
                }
                case 1:
                    this.objectsFreed = Float12ToInt((int) ((data >> 0x30) & 0xfffL));
                    this.actualSize = Float12ToInt((int) ((data >> 0x24) & 0xfffL));
                    this.allowedSize = Float12ToInt((int) ((data >> 0x18) & 0xfffL));
                    this.objectsAllocated = Float12ToInt((int) ((data >> 12) & 0xfffL));
                    this.bytesAllocated = Float12ToInt((int) (data & 0xfffL));
                    return;

                case 2:
                    this.softLimit = Float12ToInt((int) ((data >> 0x30) & 0xfffL));
                    this.zActualSize = Float12ToInt((int) ((data >> 0x24) & 0xfffL));
                    this.zAllowedSize = Float12ToInt((int) ((data >> 0x18) & 0xfffL));
                    this.zObjectsAllocated = Float12ToInt((int) ((data >> 12) & 0xfffL));
                    this.zBytesAllocated = Float12ToInt((int) (data & 0xfffL));
                    return;

                case 3:
                    this.dlmallocFootprint = Float12ToInt((int) ((data >> 0x24) & 0xfffL));
                    this.mallinfoTotalAllocatedSpace = Float12ToInt((int) ((data >> 0x18) & 0xfffL));
                    this.externalLimit = Float12ToInt((int) ((data >> 12) & 0xfffL));
                    this.externalBytesAllocated = Float12ToInt((int) (data & 0xfffL));
                    return;
            }
        }

        private static byte[] Put64bitsToArray(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return bytes;
        }

        public override string ToString()
        {
            return this.ObjectToString(null);
        }

        public override EventValueType EventValueType
        {
            get
            {
                return EventValueType.LIST;
            }
        }

        public static EventValueDescription[] ValueDescriptions
        {
            get
            {
                try
                {
                    return new EventValueDescription[] { 
                        new EventValueDescription("Process Name", EventValueType.STRING), new EventValueDescription("GC Time", EventValueType.LONG, ValueType.MILLISECONDS), new EventValueDescription("Freed Objects", EventValueType.LONG, ValueType.OBJECTS), new EventValueDescription("Freed Bytes", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Soft Limit", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Actual Size (aggregate)", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Allowed Size (aggregate)", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Allocated Objects (aggregate)", EventValueType.LONG, ValueType.OBJECTS), new EventValueDescription("Allocated Bytes (aggregate)", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Actual Size", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Allowed Size", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Allocated Objects", EventValueType.LONG, ValueType.OBJECTS), new EventValueDescription("Allocated Bytes", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Actual Size (zygote)", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Allowed Size (zygote)", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Allocated Objects (zygote)", EventValueType.LONG, ValueType.OBJECTS), 
                        new EventValueDescription("Allocated Bytes (zygote)", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("External Allocation Limit", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("External Bytes Allocated", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("dlmalloc Footprint", EventValueType.LONG, ValueType.BYTES), new EventValueDescription("Malloc Info: Total Allocated Space", EventValueType.LONG, ValueType.BYTES)
                     };
                }
                catch (InvalidCastException)
                {
                }
                return null;
            }
        }
    }
}

