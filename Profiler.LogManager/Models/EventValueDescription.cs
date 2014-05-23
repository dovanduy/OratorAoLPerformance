using System;
using System.Runtime.CompilerServices;

namespace Profiler.LogManager.Models
{

    public class EventValueDescription
    {
        public EventValueDescription(string name, EventValueType type)
        {
            this.Name = name;
            this.EventValueType = type;
            this.ValueType = ((this.EventValueType == EventValueType.INT) || (this.EventValueType == EventValueType.LONG)) ? ValueType.BYTES : ValueType.NOT_APPLICABLE;
        }

        public EventValueDescription(string name, EventValueType type, ValueType valueType)
        {
            this.Name = name;
            this.EventValueType = type;
            this.ValueType = valueType;
            this.ValueType.CheckType(this.EventValueType);
        }

        public bool CheckForType(object value)
        {
            switch (this.EventValueType)
            {
                case EventValueType.INT:
                    return (value is int);

                case EventValueType.LONG:
                    return (value is long);

                case EventValueType.STRING:
                    return (value is string);

                case EventValueType.LIST:
                    return (value is object[]);
            }
            return false;
        }

        public object GetObjectFromString(string value)
        {
            switch (this.EventValueType)
            {
                case EventValueType.INT:
                    try
                    {
                        return int.Parse(value);
                    }
                    catch (FormatException)
                    {
                        return null;
                    }
                    break;

                case EventValueType.LONG:
                    break;

                case EventValueType.STRING:
                    return value;

                default:
                    return null;
            }
            try
            {
                return long.Parse(value);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public override string ToString()
        {
            if (this.ValueType != ValueType.NOT_APPLICABLE)
            {
                return string.Format("{0} ({1}, {2})", this.Name, this.EventValueType.ToString().ToLower(), this.ValueType.ToString().ToLower());
            }
            return string.Format("{0} ({1})", this.Name, this.EventValueType.ToString());
        }

        public EventValueType EventValueType { get; private set; }

        public string Name { get; private set; }

        public ValueType ValueType { get; private set; }
    }
}

