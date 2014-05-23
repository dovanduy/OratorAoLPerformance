using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Profiler.LogManager.Models
{
    public abstract class MultiLineReceiver : IShellOutputReceiver
    {
        protected const string ENCODING = "ISO-8859-1";
        protected const string NEWLINE = "\r\n";

        public MultiLineReceiver()
        {
            this.Lines = new List<string>();
        }

        protected virtual void AddLine(string line)
        {
            this.Lines.Add(line);
        }

        public void AddOutput(byte[] data, int offset, int length)
        {
            if (!this.IsCancelled)
            {
                string str = null;
                try
                {
                    str = Encoding.GetEncoding("ISO-8859-1").GetString(data, offset, length);
                }
                catch (DecoderFallbackException)
                {
                    str = Encoding.Default.GetString(data, offset, length);
                }
                if (!string.IsNullOrEmpty(str))
                {
                    if (!string.IsNullOrEmpty(this.UnfinishedLine))
                    {
                        str = this.UnfinishedLine + str;
                        this.UnfinishedLine = null;
                    }
                    int startIndex = 0;
                    while (true)
                    {
                        int index = str.IndexOf("\r\n", startIndex);
                        if (index == -1)
                        {
                            this.UnfinishedLine = str.Substring(startIndex);
                            return;
                        }
                        string line = str.Substring(startIndex, index - startIndex);
                        if (this.TrimLines)
                        {
                            line = line.Trim();
                        }
                        this.AddLine(line);
                        startIndex = index + 2;
                    }
                }
            }
        }

        protected virtual void Done()
        {
        }

        public void Flush()
        {
            if (!this.IsCancelled && (this.Lines.Count > 0))
            {
                string[] lines = this.Lines.ToArray<string>();
                this.ProcessNewLines(lines);
                this.Lines.Clear();
            }
            if (!this.IsCancelled && !string.IsNullOrEmpty(this.UnfinishedLine))
            {
                this.ProcessNewLines(new string[] { this.UnfinishedLine });
            }
            this.Done();
        }

        protected abstract void ProcessNewLines(string[] lines);

        public virtual bool IsCancelled { get; protected set; }

        protected ICollection<string> Lines { get; set; }

        public bool TrimLines { get; set; }

        protected string UnfinishedLine { get; set; }
    }
}

