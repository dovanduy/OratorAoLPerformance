using Managed.Adb.Logs;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Profiler.LogManager.Models
{

    public class EventContainer
    {
        private DateTime m_epoc;
        private static Regex STORAGE_PATTERN = new Regex(@"^(\d+)@(.*)$", RegexOptions.Compiled);

        public EventContainer(LogEntry entry, int tag, string tagName, object data)
        {
            this.m_epoc = new DateTime(0x7b2, 1, 1, 0, 0, 0);
            this.Data = data;
            this.Tag = tag;
            this.TagName = tagName;
            this.ProcessId = entry.ProcessId;
            this.ThreadId = entry.ThreadId;
            this.Second = entry.NanoSeconds;
            this.Nanosecond = entry.NanoSeconds;
        }

        public EventContainer(int tag, string tagName, int pid, int tid, int sec, int nsec, object data)
        {
            this.m_epoc = new DateTime(0x7b2, 1, 1, 0, 0, 0);
            this.Data = data;
            this.Tag = tag;
            this.TagName = tagName;
            this.ProcessId = pid;
            this.ThreadId = tid;
            this.Second = sec;
            this.Nanosecond = nsec;
        }

        public int GetInt()
        {
            if (this.GetType(this.Data) != EventValueType.INT)
            {
                throw new InvalidCastException();
            }
            return (int) this.Data;
        }

        public long GetLong()
        {
            if (this.GetType(this.Data) != EventValueType.LONG)
            {
                throw new InvalidCastException();
            }
            return (long) this.Data;
        }

        public static object GetObjectFromStorageString(string value)
        {
            Match match = STORAGE_PATTERN.Match(value);
            if (match.Success)
            {
                try
                {
                    EventValueType type;
                    if (!Enum.TryParse<EventValueType>(match.Groups[1].Value, out type))
                    {
                        return null;
                    }
                    switch (type)
                    {
                        case EventValueType.INT:
                            return int.Parse(match.Groups[2].Value);

                        case EventValueType.LONG:
                            return long.Parse(match.Groups[2].Value);

                        case EventValueType.STRING:
                            return match.Groups[2].Value;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static string GetStorageString(object obj)
        {
            if (obj is string)
            {
                return (EventValueType.STRING + "@" + obj.ToString());
            }
            if (obj is int)
            {
                return (EventValueType.INT + "@" + obj.ToString());
            }
            if (obj is long)
            {
                return (EventValueType.LONG + "@" + obj.ToString());
            }
            return null;
        }

        public string GetString()
        {
            if (this.GetType(this.Data) != EventValueType.STRING)
            {
                throw new InvalidCastException();
            }
            return (string) this.Data;
        }

        protected virtual EventValueType GetType(object data)
        {
            if (data is int)
            {
                return EventValueType.INT;
            }
            if (data is long)
            {
                return EventValueType.LONG;
            }
            if (data is string)
            {
                return EventValueType.STRING;
            }
            if (!(data is object[]))
            {
                return EventValueType.UNKNOWN;
            }
            object[] objArray = (object[]) data;
            foreach (object obj2 in objArray)
            {
                switch (this.GetType(obj2))
                {
                    case EventValueType.LIST:
                    case EventValueType.TREE:
                        return EventValueType.TREE;
                }
            }
            return EventValueType.LIST;
        }

        public virtual object GetValue(int valueIndex)
        {
            return this.GetValue(this.Data, valueIndex, true);
        }

        private object GetValue(object data, int valueIndex, bool recursive)
        {
            switch (this.GetType(data))
            {
                case EventValueType.INT:
                case EventValueType.LONG:
                case EventValueType.STRING:
                    return data;

                case EventValueType.LIST:
                {
                    if (!recursive)
                    {
                        break;
                    }
                    object[] objArray = (object[]) data;
                    if ((valueIndex < 0) || (valueIndex >= objArray.Length))
                    {
                        break;
                    }
                    return this.GetValue(objArray[valueIndex], valueIndex, false);
                }
            }
            return null;
        }

        public virtual double GetValueAsDouble(int valueIndex)
        {
            return this.GetValueAsDouble(this.Data, valueIndex, true);
        }

        private double GetValueAsDouble(object data, int valueIndex, bool recursive)
        {
            switch (this.GetType(data))
            {
                case EventValueType.INT:
                case EventValueType.LONG:
                    return (double) data;

                case EventValueType.STRING:
                    throw new InvalidCastException();

                case EventValueType.LIST:
                    if (recursive)
                    {
                        object[] objArray = (object[]) data;
                        if ((valueIndex >= 0) && (valueIndex < objArray.Length))
                        {
                            return this.GetValueAsDouble(objArray[valueIndex], valueIndex, false);
                        }
                    }
                    break;
            }
            throw new InvalidCastException();
        }

        public virtual string GetValueAsString(int valueIndex)
        {
            return this.GetValueAsString(this.Data, valueIndex, true);
        }

        private string GetValueAsString(object data, int valueIndex, bool recursive)
        {
            EventValueType type = this.GetType(data);
            switch (type)
            {
                case EventValueType.INT:
                case EventValueType.LONG:
                case EventValueType.STRING:
                    return data.ToString();

                case EventValueType.LIST:
                {
                    if (!recursive)
                    {
                        throw new InvalidCastException("getValueAsString() doesn't support EventValueType.TREE");
                    }
                    object[] objArray = (object[]) data;
                    if ((valueIndex < 0) || (valueIndex >= objArray.Length))
                    {
                        break;
                    }
                    return this.GetValueAsString(objArray[valueIndex], valueIndex, false);
                }
            }
            throw new InvalidCastException("getValueAsString() unsupported type:" + type);
        }

        protected virtual string ObjectToString(object obj)
        {
            if (!(obj is object[]))
            {
                return obj.ToString();
            }
            object[] objArray = obj as object[];
            StringBuilder builder = new StringBuilder("[");
            for (int i = 0; i < objArray.Length; i++)
            {
                builder.Append(this.ObjectToString(objArray[i]));
                if (i < (objArray.Length - 1))
                {
                    builder.Append(",");
                }
            }
            builder.Append("]");
            return builder.ToString();
        }

        public override string ToString()
        {
            return this.ObjectToString(this.Data);
        }

        public object Data { get; private set; }

        public Type DataType
        {
            get
            {
                return this.Data.GetType();
            }
        }

        public virtual EventValueType EventValueType
        {
            get
            {
                return this.GetType(this.Data);
            }
        }

        public int Nanosecond { get; private set; }

        public int ProcessId { get; private set; }

        public int Second { get; private set; }

        public int Tag { get; private set; }

        public string TagName { get; private set; }

        public int ThreadId { get; private set; }

        public DateTime Timestamp
        {
            get
            {
                return this.m_epoc.AddSeconds((double) this.Second).AddTicks((long) (this.Nanosecond / 100)).ToLocalTime();
            }
        }
    }
}

