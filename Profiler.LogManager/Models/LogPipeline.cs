using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Profiler.LogManager.Models
{
    public class LogPipeline
    {
        private BlockingCollection<LogData> _inputQuene;
        private BlockingCollection<LogData> _outputQuene;
        public LogPipeline()
        {
            _inputQuene = new BlockingCollection<LogData>();
            _outputQuene = new BlockingCollection<LogData>();
        }


        public void SetPipeline(BlockingCollection<LogData> inputQuene, BlockingCollection<LogData> outputQuenu)
        {
            if (inputQuene == null)
            {
                throw new ArgumentNullException("inputQuene");
            }
            if (outputQuenu == null)
            {
                throw new ArgumentNullException("outputQuenu");
            }

            _inputQuene = inputQuene;
            _outputQuene = outputQuenu;
            _opentimeMatchResult = new Dictionary<string, object>();
        }

        public void Start()
        {
            Task.Factory.StartNew(() => { this.OpentimeLog(); });
        }

        private void OpentimeLog()
        {
            if (!_inputQuene.IsAddingCompleted)
            {
                LogData log = _inputQuene.Take();
                // filter saved as

                 Match startMatch = REGEX_OPEN_TIME_START.Match(log.Data);
                 if (startMatch.Success)
                 {
                     string cmp = startMatch.Value;
                     if (!_opentimeMatchResult.ContainsKey(cmp))
                     {
                         _opentimeMatchResult.Add(cmp, null);
                     }
                 }

                 Match displayedMatch = REGEX_OPEN_TIME_DISPALYED.Match(log.Data);
                 if (displayedMatch.Success)
                 {
                     string cmp = displayedMatch.Groups["CMP"].Value;
                     int minuteSpan = 0;
                     int.TryParse(displayedMatch.Groups["MinuteSpan"].Value, out minuteSpan);
                     int secondSpan = 0;
                     int.TryParse(displayedMatch.Groups["SecondSpan"].Value, out  secondSpan);
                     int millisecondSpan = 0;
                     int.TryParse(displayedMatch.Groups["MillisecondSpan"].Value, out millisecondSpan);
                     int totalSpan = (minuteSpan * 60 + secondSpan) * 1000 + millisecondSpan;
                     if (_opentimeMatchResult.ContainsKey(cmp))
                     {
                         _opentimeMatchResult[cmp] = totalSpan;
                     }
                 }

                // filter opentime logs
                _outputQuene.Add(log);
            }
        }

        private readonly Regex REGEX_OPEN_TIME_START = new Regex(@"(?<TimeStamp>[\d-:\s\.]*)\sI/ActivityManager(.*)START(.*)cmp=(?<CMP>(\w+\.)*\w+/(\w+\.)*(\.)?\w+)", RegexOptions.Compiled);
        private readonly Regex REGEX_OPEN_TIME_DISPALYED = new Regex(@"(?<TimeStamp>[\d-:\s\.]*)(.*)I/ActivityManager(.*)Displayed\s(?<CMP>(\w+\.)*\w+/((\w+\.)*)?\.?\w+):\s\+((?<MinuteSpan>\d+)min)?((?<SecondSpan>\d+)s)?((?<MillisecondSpan>\d+)ms)?", RegexOptions.Compiled);
        Dictionary<string, object> _opentimeMatchResult;
    }
}
