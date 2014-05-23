using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Profiler.LogManager.Models
{
    public class LogDataCollection : List<LogData>
    {
        //private static object _obj = new object();

        //public static LogDataCollection Instance;
        //static LogDataCollection(int capacity)
        //{
        //    if (Instance == null)
        //    {
        //        lock (_obj)
        //        {
        //            Instance = new LogDataCollection(capacity);
        //        }
        //    }
        //}



        //public bool IsNew { get; private set; }

        //public int Capacity { get; private set; }
        //private LogDataCollection(int capacity)
        //{
        //    Capacity = capacity;
        //    Instance = new LogDataCollection(capacity);
        //    IsNew = true;
        //}

        //public void Add(LogData logData)
        //{
        //    if (Value.Count <= Capacity)
        //    {
        //        Value.Add(logData);
        //        IsNew = false;
        //    }
        //    else
        //    {
        //        Value = new LogDataCollection(Capacity);
        //        IsNew = true;
        //        Value.Add(logData);
        //    }
        //}

        public void Add(ILogData logData)
        {
            this.Add(logData);
        }

        public void AddRange(List<ILogData> logDataArray)
        {
            this.AddRange(logDataArray);
        }
    }
}
