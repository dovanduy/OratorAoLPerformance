using System;

namespace Profiler.LogManager.Models
{
    public class TagReceiver : MultiLineReceiver
    {
        private Action<string> m_action;

        public TagReceiver(Action<string> lineProcessor)
        {
            this.m_action = lineProcessor;
        }

        protected override void ProcessNewLines(string[] lines)
        {
            foreach (string str in lines)
            {
                this.m_action(str);
            }
        }
    }
}

