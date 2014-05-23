using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Managed.Adb;
using Managed.Adb.Logs;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Profiler.LogManager.Models
{
    public class Logger : ILogger
    {
        public event LogDataReceivedHandler LogDataReceivedEvent;
        public bool IsCancel{get;set;}
        private IDevice _device;
        public LogBuffer L_Buffer{get;set;}

        private List<IListener> l_listeners;

        private bool _isListener;

        private BlockingCollection<ILogData> _receivedLogQueue;


        public Logger(IDevice device)
        {
            _device = device; 
            this.m_writeSourceBufferName = true;
            this.m_format = OutputFormat.Brief;
            this.m_timestampMpde = LogTimestampMode.Remote;
            _receivedLogQueue = new BlockingCollection<ILogData>(1000);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClearBuffer"></param>
        public void Start(bool isClearBuffer = false)
        {
            this.Start(LogBuffer.All, isClearBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClearBuffer"></param>
        public void Start(bool isClearBuffer =false, bool isRecording =false, string recordingPath = "")
        {
            //string logFilePath;
            //if (File.Exists(recordingPath))
            //{
            //    logFilePath = recordingPath;
            //}
            //else
            //{
            //    logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logger.txt");
            //}
            //StartRecording(logFilePath);

            this.Start(LogBuffer.All, isClearBuffer);
        }

        Action<IListener> stopListener = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logBuffer"></param>
        /// <param name="isClearBuffer">True : clear log buffer, False : unclear log buffer</param>
        public void Start(LogBuffer logBuffer, bool isClearBuffer)
        {
            if (!_isListener)
            {
                Action<IListener> startListener = null;
                l_listeners = new List<IListener>();
                L_Buffer = logBuffer;
                if (L_Buffer.HasFlag(LogBuffer.Events))
                {
                    l_listeners.Add(new LogListerner(_device, LogBuffer.Events));
                }
                if (L_Buffer.HasFlag(LogBuffer.Main))
                {
                    l_listeners.Add(new LogListerner(_device, LogBuffer.Main));
                }
                if (L_Buffer.HasFlag(LogBuffer.Radio))
                {
                    l_listeners.Add(new LogListerner(_device, LogBuffer.Radio));
                }
                if (L_Buffer.HasFlag(LogBuffer.System))
                {
                    l_listeners.Add(new LogListerner(_device, LogBuffer.System));
                }
                if (startListener == null)
                {
                    startListener = delegate(IListener l)
                    {
                        l.LogDataReceivedEvent += this.OnLogDataReceived;
                        l.Start(isClearBuffer);
                    };
                }

                if (stopListener == null)
                {
                    stopListener = (l) => {
                        l.LogDataReceivedEvent -= this.OnLogDataReceived;
                        l.Stop(isClearBuffer);
                    };
                }

                Thread blockingT = new Thread(new ThreadStart(this.StartReceivedLogQueue));
                blockingT.Start();

                Parallel.ForEach<IListener>(l_listeners, startListener);

                _isListener = true;
            }
        }

        public void Stop(bool isClearBuffer)
        {
            _isListener = false;
            Parallel.ForEach<IListener>(l_listeners, stopListener);
            //m_isRecording = false;
            //lock (_writerLock)
            //{
            //    try
            //    {
            //        if (_writer != null)
            //        {
            //            _writer.Flush();
            //            _writer.Close();
            //            _writer.Dispose();
            //        }
            //    }
            //    catch (Exception)
            //    {
                 
            //    }
            //}
            if (!_receivedLogQueue.IsAddingCompleted)
            {
                _receivedLogQueue.CompleteAdding();
            }
        }

        private static object _writerLock = new object();
        private TextWriter _writer;
        private string m_logFile;
        private int m_logIndex = 0;
        private bool m_isRecording;
        private bool m_writeSourceBufferName; 
        private OutputFormat m_format;
        private LogTimestampMode m_timestampMpde;


        private void OnLogDataReceived(object sendor, LogDataReceivedEventArgs args)
        {
            if (args == null || args.Log == null)
            { return; }
            ILogData logData = args.Log;

            //lock (_writerLock)
            //{
            //    try
            //    {
            //        if (this.m_isRecording && (this._writer != null))
            //        {
            //            this.CheckLogDataSize();
            //            if (this.m_writeSourceBufferName)
            //            {
            //                this._writer.WriteLine(string.Format("[{0}] [{1}:] {2}", logData.LocalTimestamp.ToString("MM-dd HH:mm:ss.fff"), logData.SourceBuffer, logData.ToString(this.OutputFormat, this.TimestampMode)));
            //            }
            //            else
            //            {
            //                this._writer.WriteLine(string.Format("[{0}] {1}", logData.LocalTimestamp.ToString("MM-dd HH:mm:ss.fff"), logData.ToString(this.OutputFormat, this.TimestampMode)));
            //            }
            //            this.m_logIndex++;
            //            this._writer.Flush();
            //        }


            //        //if (this.LogDataReceivedEvent != null)
            //        //{
            //        //    this.LogDataReceivedEvent(this, args);
            //        //}
            //    }
            //    catch (Exception)
            //    {
            //        if (_writer != null)
            //        {
            //            _writer.Close();
            //            _writer.Dispose();
            //        }
            //    }

            //}

            _receivedLogQueue.Add(logData);
        }

        private void CheckLogDataSize()
        {
            if ((this.m_logFile != null) && ((this.m_logIndex % 0x3e8) == 0))
            {
                FileInfo info = new FileInfo(this.m_logFile);
                if (info.Length > 0x5f5e100L)
                {
                    int num;
                    this.m_isRecording = false;
                    this.m_logIndex = 0;
                    if (this._writer != null)
                    {
                        try
                        {
                            this._writer.Flush();
                            this._writer.Dispose();
                        }
                        catch (ObjectDisposedException)
                        {
                        }
                    }
                    string directoryName = Path.GetDirectoryName(this.m_logFile);
                    string fileName = Path.GetFileName(this.m_logFile);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    if (fileName.Contains("_"))
                    {
                        num = Convert.ToInt16(fileNameWithoutExtension.Split(new char[] { '_' }).Last<string>());
                        string str4 = fileNameWithoutExtension.Split(new char[] { '_' }).First<string>();
                        num++;
                        fileName = string.Join("", new string[] { str4, "_", num.ToString(), ".txt" });
                    }
                    else
                    {
                        num = 2;
                        fileName = string.Join("", new string[] { fileNameWithoutExtension, "_", num.ToString(), ".txt" });
                    }
                    this.m_logFile = Path.Combine(directoryName, fileName);
                    this.CreateLogWriter();
                    this.m_isRecording = true;
                }
            }
        }

        private void CreateLogWriter()
        {
            if (this._writer != null)
            {
                try
                {
                    this._writer.Flush();
                    this._writer.Dispose();
                }
                catch (ObjectDisposedException)
                {
                }
            }

            //this._writer = new StreamWriter(new FileStream(this.m_logFile, System.IO.FileMode.Create, FileAccess.Write, FileShare.Read));
            this._writer = TextWriter.Synchronized(new StreamWriter(new FileStream(this.m_logFile, System.IO.FileMode.Create, FileAccess.Write, FileShare.Read)));
        }

        public OutputFormat OutputFormat
        {
            get
            {
                return this.m_format;
            }
            set
            {
                if (!this.m_isRecording)
                {
                    this.m_format = value;
                }
            }
        }

        public LogTimestampMode TimestampMode
        {
            get
            {
                return this.m_timestampMpde;
            }
            set
            {
                if (!this.m_isRecording)
                {
                    this.m_timestampMpde = value;
                }
            }
        }

        public void StartRecording(string fileLocation)
        {
            this.StartRecording(fileLocation, System.IO.FileMode.Create);
        }

        public void StartRecording(string fileLocation, System.IO.FileMode fileMode)
        {
            if (string.IsNullOrEmpty(fileLocation))
            {
                throw new ArgumentNullException("logFileLocation");
            }
            if (!this.m_isRecording)
            {
                this.m_logFile = fileLocation;
                lock (_writerLock)
                {
                    this.m_isRecording = false;
                    this.CreateLogWriter();
                    this.m_isRecording = true;
                }
            }
        }


        private void StartReceivedLogQueue()
        {
            while (!_receivedLogQueue.IsCompleted)
            {
                ILogData logData = _receivedLogQueue.Take();
                if (logData != null)
                {
                    if (this.LogDataReceivedEvent != null)
                    {
                        LogDataReceivedEventArgs args = new LogDataReceivedEventArgs();
                        args.Log = logData;
                        this.LogDataReceivedEvent(this, args);
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
