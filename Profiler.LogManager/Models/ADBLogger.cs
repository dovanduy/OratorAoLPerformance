using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Profiler.LogManager.Models
{
    public class ADBLogger
    {
        private const string LOGGER_ADB = "adb.exe";

        public event LogDataReceivedHandler LogDataReceived;

        public string ADBFullName { get; private set; }

        public bool IsCanel { get; set; }

        public string Arguments { get; set; }

        private Action _killAdbProc;

        private bool _isKilled;

        public ADBLogger(string adbLocation)
        {
            if (!Directory.Exists(adbLocation))
            {
                throw new ArgumentNullException("adbLocation");
            }

            string adbFullName = Path.Combine(adbLocation, LOGGER_ADB);
            if (!File.Exists(adbFullName))
            {
                throw new ArgumentNullException("adbFullName");
            } 
            ADBFullName = adbFullName;
            _isKilled = false;
        }
        
        /// <summary>
        /// Lanch adb.exe by logcat command argument
        /// default command argument : logcat
        /// </summary>
        public void Start()
        {
            Start("logcat -v time");
        }

        /// <summary>
        /// Lanch adb.exe by logcat command argument
        /// </summary>
        /// <param name="arguments">arguments</param>
        public void Start(string arguments)
        {
            Arguments = arguments;
            Action adbAction = () =>
            {
                IsCanel = false;
                Process adbProc = new Process();
                adbProc.StartInfo = new ProcessStartInfo()
                {
                    Arguments = Arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = ADBFullName,
                    //StandardOutputEncoding = Encoding.GetEncoding("utf-8"),
                    StandardOutputEncoding = Encoding.GetEncoding("ISO-8859-1"),
                    RedirectStandardOutput = true,
                };
                adbProc.Exited += (sendor, e) => { 
                    _isKilled = true; 
                };

                _killAdbProc = () =>
                {
                    if (_isKilled == false)
                    {
                        adbProc.Kill();
                    }
                };

                try
                {
                    _isKilled = false;
                    adbProc.Start();
                    using (StreamReader sr = adbProc.StandardOutput)
                    {
                        while (!IsCanel && !adbProc.HasExited)
                        {
                            string log = sr.ReadLine();
                            if (string.IsNullOrWhiteSpace(log) == true)
                            { continue; }
                            if (log.Contains("\\r\\n"))
                            {
                                log = log.Replace("\\r\\n", "\\n");
                            }


                            if (LogDataReceived != null)
                            {
                                ILogData logData = new LogData();
                                logData.Data = log;
                                LogDataReceivedEventArgs args = new LogDataReceivedEventArgs();
                                args.Log = logData;
                                LogDataReceived(this, args);
                            }
                        }
                    }
                }
                finally
                {
                    adbProc.WaitForExit();
                    adbProc.Close();
                }
            };
            Task.Factory.StartNew(adbAction).Start();
        }

        /// <summary>
        /// Stop reveive logs and kill the adb.exe process
        /// </summary>
        public void Stop()
        {
            IsCanel = true;
            if (_killAdbProc != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(_killAdbProc);
            }
        }
    }
}
