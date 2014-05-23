using System;
using System.Runtime.CompilerServices;

namespace Profiler.LogManager.Models
{
    public static class ValueTypeMethods
    {
        public static void CheckType(this ValueType vt, EventValueType type)
        {
            if (((type != EventValueType.INT) && (type != EventValueType.LONG)) && (vt != ValueType.NOT_APPLICABLE))
            {
                throw new ArgumentException(string.Format("{0} doesn't support type {1}", type, vt));
            }
        }
    }
}

