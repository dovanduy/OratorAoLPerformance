using Managed.Adb;
using Managed.Adb.Logs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Profiler.LogManager.Models
{
    public class EventLogParser
    {
        private const string EVENT_TAG_MAP_FILE = "/system/etc/event-log-tags";
        private const int EVENT_TYPE_INT = 0;
        private const int EVENT_TYPE_LIST = 3;
        private const int EVENT_TYPE_LONG = 1;
        private const int EVENT_TYPE_STRING = 2;
        private Dictionary<int, string> mTagMap = new Dictionary<int, string>();
        private Dictionary<int, EventValueDescription[]> mValueDescriptionMap = new Dictionary<int, EventValueDescription[]>();
        private Regex PATTERN_DESCRIPTION = new Regex(@"\(([A-Za-z0-9_\s]+)\|(\d+)(\|\d+){0,1}\)", RegexOptions.Compiled);
        private Regex PATTERN_SIMPLE_TAG = new Regex(@"^(\d+)\s+([A-Za-z0-9_]+)\s*$", RegexOptions.Compiled);
        private Regex PATTERN_TAG_WITH_DESC = new Regex(@"^(\d+)\s+([A-Za-z0-9_]+)\s*(.*)\s*$", RegexOptions.Compiled);

        public bool Init(Device device)
        {
            try
            {
                //device.ExecuteShellCommand("cat /system/etc/event-log-tags", new TagReceiver(new Action<string>(this.ProcessTagLine)), DdmPreferences.LongTimeout);
                if (!this.mTagMap.ContainsKey(0x4e21))
                {
                    this.mTagMap.Add(0x4e21, "GC");
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public EventContainer Parse(LogEntry entry)
        {
            string str;
            if (entry.Length < 4)
            {
                return null;
            }
            int startIndex = 0;
            int key = BitConverter.ToInt32(entry.Data, startIndex);
            startIndex += 4;
            str = this.mTagMap.ContainsKey(key) ? (str = this.mTagMap[key]) : key.ToString();
            List<object> list = new List<object>();
            if (this.ParseBinaryEvent(entry.Data, startIndex, list) == -1)
            {
                return null;
            }
            object data = (list.Count == 1) ? list[0] : list.ToArray();
            if (key != 0x4e21)
            {
                return new EventContainer(entry, key, str, data);
            }
            return new GcEventContainer(entry, key, str, data);
        }

        private int ParseBinaryEvent(byte[] eventData, int dataOffset, List<object> list)
        {
            int num;
            if ((eventData.Length - dataOffset) >= 1)
            {
                num = dataOffset;
                switch (eventData[num++])
                {
                    case 0:
                        if ((eventData.Length - num) >= 4)
                        {
                            int item = BitConverter.ToInt32(eventData, num);
                            num += 4;
                            list.Add(item);
                            goto Label_011C;
                        }
                        return -1;

                    case 1:
                        if ((eventData.Length - num) >= 8)
                        {
                            long num4 = BitConverter.ToInt64(eventData, num);
                            num += 8;
                            list.Add(num4);
                            goto Label_011C;
                        }
                        return -1;

                    case 2:
                        if ((eventData.Length - num) >= 4)
                        {
                            int count = BitConverter.ToInt32(eventData, num);
                            num += 4;
                            if ((eventData.Length - num) < count)
                            {
                                return -1;
                            }
                            try
                            {
                                string str = Encoding.GetEncoding("UTF-8").GetString(eventData, num, count);
                                list.Add(str);
                            }
                            catch (Exception)
                            {
                            }
                            num += count;
                            goto Label_011C;
                        }
                        return -1;

                    case 3:
                        if ((eventData.Length - num) >= 1)
                        {
                            int num6 = eventData[num++];
                            List<object> list2 = new List<object>();
                            for (int i = 0; i < num6; i++)
                            {
                                int num8 = this.ParseBinaryEvent(eventData, num, list2);
                                if (num8 == -1)
                                {
                                    return num8;
                                }
                                num += num8;
                            }
                            list.AddRange(list2);
                            goto Label_011C;
                        }
                        return -1;
                }
            }
            return -1;
        Label_011C:
            return (num - dataOffset);
        }

        private EventValueDescription[] ProcessDescription(string description)
        {
            string[] strArray = Regex.Split(description, @"\s*,\s*", RegexOptions.Compiled);
            List<EventValueDescription> list = new List<EventValueDescription>();
            foreach (string str in strArray)
            {
                Match match = this.PATTERN_DESCRIPTION.Match(str);
                if (match.Success)
                {
                    try
                    {
                        string name = match.Groups[1].Value;
                        EventValueType type = (EventValueType) int.Parse(match.Groups[2].Value);
                        string str3 = match.Groups[3].Value;
                        if (!string.IsNullOrEmpty(str3))
                        {
                            ValueType valueType = (ValueType) int.Parse(str3.Substring(1));
                            list.Add(new EventValueDescription(name, type, valueType));
                        }
                        else
                        {
                            list.Add(new EventValueDescription(name, type));
                        }
                    }
                    catch (FormatException)
                    {
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (list.Count != 0)
            {
                return list.ToArray();
            }
            return null;
        }

        private void ProcessTagLine(string line)
        {
            if (!string.IsNullOrEmpty(line) && (line[0] != '#'))
            {
                Match match = this.PATTERN_TAG_WITH_DESC.Match(line);
                if (match.Success)
                {
                    try
                    {
                        int key = int.Parse(match.Groups[1].Value);
                        string str = match.Groups[2].Value;
                        if ((str != null) && !this.mTagMap.ContainsKey(key))
                        {
                            this.mTagMap.Add(key, str);
                        }
                        if (key == 0x4e21)
                        {
                            this.mValueDescriptionMap.Add(key, GcEventContainer.ValueDescriptions);
                        }
                        else
                        {
                            string str2 = match.Groups[3].Value;
                            if (!string.IsNullOrEmpty(str2))
                            {
                                EventValueDescription[] descriptionArray = this.ProcessDescription(str2);
                                if ((descriptionArray != null) && !this.mValueDescriptionMap.ContainsKey(key))
                                {
                                    this.mValueDescriptionMap.Add(key, descriptionArray);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    match = this.PATTERN_SIMPLE_TAG.Match(line);
                    if (match.Success)
                    {
                        int num2 = int.Parse(match.Groups[1].Value);
                        string str3 = match.Groups[2].Value;
                        if ((str3 != null) && !this.mTagMap.ContainsKey(num2))
                        {
                            this.mTagMap.Add(num2, str3);
                        }
                    }
                }
            }
        }

        public Dictionary<int, string> TagMap
        {
            get
            {
                return this.mTagMap;
            }
        }

        public Dictionary<int, EventValueDescription[]> ValueDescriptionMap
        {
            get
            {
                return this.mValueDescriptionMap;
            }
        }
    }
}

