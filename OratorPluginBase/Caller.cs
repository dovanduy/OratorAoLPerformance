using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace OratorPluginBase
{
    [ProtoContract(AsReferenceDefault = true)]
    public class OSTask
    {
        //at the moment only needed for deep copying of memorystatuses in memory leak processing. consider dropping after proper revision of memory leak sequences parsing.
        //[ProtoMember(1)]
        public int ID { get; set; }
        [ProtoMember(2, AsReference = true)]
        public string Name { get; set; }

        public bool Solved { get; set; }
    }

    [ProtoContract(AsReferenceDefault = true)]
    public class Caller
    {
        //at the moment only needed for deep copying of memorystatuses in memory leak processing. consider dropping after proper revision of memory leak sequences parsing.
        //[ProtoMember(1)]
        public UInt32 Pointer { get; set; }
        [ProtoMember(2, AsReference = true)]
        public string Path { get; set; }
        [ProtoMember(3, AsReference = true)]
        public string File { get; set; }
        [ProtoMember(4, AsReference = true)]
        public string Function { get; set; }

        public string FullName
        {
            get
            {
                string name = "";
                if (!string.IsNullOrWhiteSpace(Function))
                {
                    if (!string.IsNullOrWhiteSpace(File))
                    {
                        name = File + " : ";
                    }
                    name += Function;
                }
                return name;
            }
        }

        public bool Solved { get; set; }
    }

    //Easier to databind to this instead of standard dictionary due to no KeyNotFoundExceptions
    [ProtoContract(SkipConstructor = true)]
    public class CallerDictionary : IEquatable<CallerDictionary>
    {
        [ProtoMember(1)]
        public Dictionary<byte, Caller> Dictionary;

        public int Count
        {
            get
            {
                if (Dictionary == null)
                {
                    return 0;
                }
                else
                {
                    return Dictionary.Count;
                }
            }
        }

        public CallerDictionary()
        {
            Dictionary = new Dictionary<byte, Caller>();
        }

        public bool Equals(CallerDictionary other)
        {
            if (this == other) return true;
            if (this.Dictionary == null || other.Dictionary == null) return false;
            if (this.Count != other.Count) return false;

            foreach (var item in Dictionary)
            {
                Caller otherValue;
                //othervalue is null?
                if (!other.TryGetValue(item.Key, out otherValue)) return false;
                if (!item.Value.Equals(otherValue)) return false;
            }
            return true;
        }

        public Caller this[byte key]
        {
            get
            {
                Caller value = null;
                if (Dictionary != null && Dictionary.TryGetValue(key, out value))
                {
                    return value;
                }
                else return null;
            }
            set
            {
                Dictionary[key] = value;
            }
        }

        public void Add(byte key, Caller value)
        {
            Dictionary.Add(key, value);
        }

        public bool ContainsKey(byte key)
        {
            if (Dictionary.ContainsKey(key)) return true;
            else return false;
        }

        public ICollection<byte> Keys
        {
            get { return Dictionary.Keys; }
        }

        public bool Remove(byte key)
        {
            Dictionary.Remove(key);
            return true;
        }

        public bool TryGetValue(byte key, out Caller value)
        {
            value = null;
            if (Dictionary.TryGetValue(key, out value)) return true;
            else return false;
        }

        public ICollection<Caller> Values
        {
            get { return Dictionary.Values; }
        }

        public void Add(KeyValuePair<byte, Caller> item)
        {
            Dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Dictionary.Clear();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<KeyValuePair<byte, Caller>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
    }
}
