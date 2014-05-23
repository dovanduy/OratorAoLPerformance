using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Managed.Adb;
using System.Diagnostics;

using Managed.Adb.MoreLinq;

namespace Profiler.DeviceMonitor.Models
{
    public class DeviceListener
    {
        public event UpdateDevicesHandler UpdateDevicesEvent;

        public string AdbLocation { get;private set; }

        private Socket _socket;

        public int AttemptOpenSocketCount { get; set; }

        public int AttemptStartAdbCount { get; set; }

        private AndroidDebugBridge _bridge;

        public bool IsMonitoring { get; set; }

        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets the devices.
        /// </summary>
        public IList<IDevice> Devices { get; private set; }

        public DeviceListener(string adbLocation)
        {
            if (string.IsNullOrWhiteSpace(adbLocation))
            {
                throw new ArgumentNullException("location of 'adb.exe' is empty");
            }

            if (File.Exists(adbLocation) == false)
            {
                throw new FileNotFoundException("'adb.exe' is not found");
            }

            AdbLocation = adbLocation;
        }

        public void Start()
        {
            Thread listener = new Thread(MonitorDevice);
            listener.Start();
        }

        private void MonitorDevice()
        {
            IsRunning = true;
            while (IsRunning)
            {
                try
                {
                    if (_socket == null)
                    {
                        _socket = this.OpenAdbConnection();
                        if (_socket == null)
                        {
                            AttemptOpenSocketCount++;
                            if (AttemptOpenSocketCount > 10)
                            {
                                if (this.StartAdb() == false)
                                {
                                    AttemptStartAdbCount++;
                                    if (AttemptStartAdbCount >= int.MaxValue)
                                    {
                                        AttemptStartAdbCount = 0;
                                    }
                                }
                                else
                                {
                                    AttemptStartAdbCount = 0;
                                }
                            }
                            this.WaitBeforeContinue();
                        }
                        else
                        {
                            AttemptOpenSocketCount = 0;
                        }
                    }

                    if (_socket != null && _socket.Connected && !IsMonitoring)
                    {
                        // Monitor
                        IsMonitoring = this.SendDeviceMonitoringRequest();

                    }

                    if (IsMonitoring)
                    {
                        byte[] lengthBuffer = new byte[4];
                        // store devices
                        // read the length of the incoming message
                        int length = ReadLength(_socket, lengthBuffer);

                        if (length >= 0)
                        {
                            // read the incoming message
                            ProcessIncomingDeviceData(length);
                        }
                    }
                }
                catch (IOException)
                {
                    if (_socket != null)
                    {
                        _socket.Close();
                    }
                    _socket = null;
                    IsMonitoring = false;
                }
                catch (Exception)
                {
                } 
            }
        }


        private Socket OpenAdbConnection()
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(DeviceMonitorHelper.GetInstance().SocketAddress);
                socket.NoDelay = true;
                return socket;
            }
            catch (IOException ex)
            {
                // return null
            }
            return null;
        }

        private bool StartAdb()
        {
            int status = -1;
            try
            {
                string commands = "start-server";
                ProcessStartInfo psi = new ProcessStartInfo(AdbLocation, commands);
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                using (Process proc = Process.Start(psi))
                {
                    List<string> errorOutput = new List<string>();
                    List<string> stdOutput = new List<string>();
                    status = this.GrabProcessOutput(proc, stdOutput, errorOutput, true);
                }
            }
            catch (Exception)
            {
                // return false
            }

            if (status != 0)
            {
                return false;
            }
            return true;
        }

        private bool StopAdb()
        {
            int status = -1;
            try
            {
                string commands = "stop-server";
                ProcessStartInfo psi = new ProcessStartInfo(AdbLocation, commands);
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;

                using (Process proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                    status = proc.ExitCode;
                }
            }
            catch (Exception)
            {
                // return false
            }

            if (status != 0)
            { return false; }
            return true;
        }

        public bool SendDeviceMonitoringRequest()
        {
            byte[] request = DeviceMonitorHelper.GetInstance().FormAdbRequest(RequestCommands.HOST_TRACK_DEVICES);

            if (DeviceMonitorHelper.GetInstance().Write(_socket, request) == false)
            {
                _socket.Close();
                throw new IOException("Sending Tracking request failed!");
            }

            SocketResponse resp = DeviceMonitorHelper.GetInstance().ReadAdbResponse(_socket, false /* readDiagString */);

            if (!resp.IOSuccess)
            {
                _socket.Close();
                throw new IOException("Failed to read the adb response!");
            }

            if (!resp.Okay)
            {
                // 
            }

            return resp.Okay;
        }


        private int GrabProcessOutput(Process proc , List<string> stdOutput, List<string> errorOutput, bool waitFor)
        {
            if (stdOutput == null)
            {
                throw new ArgumentNullException("stdOutput");
            }
            if (errorOutput == null)
            {
                throw new ArgumentNullException("errorOutput");
            }

            Thread stdOutputT = new Thread(new ThreadStart(delegate {
                try
                {
                    using (StreamReader sr = proc.StandardOutput)
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            if(!string.IsNullOrWhiteSpace(line))
                            {
                                stdOutput.Add(line);
                            }
                        }
                    }
                }
                catch (Exception)
                { 
                
                }
            }));
            stdOutputT.Start();

            Thread errorOutputT = new Thread(new ThreadStart(delegate {
                try
                {
                    using (StreamReader sr = proc.StandardError)
                    {
                        while (!sr.EndOfStream)
                        {
                            string error = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(error))
                            {
                                errorOutput.Add(error);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }));

            if (waitFor)
            {
                try
                {
                    stdOutputT.Join();
                }
                catch (Exception)
                {
                }
                try
                {
                    errorOutputT.Join();
                }
                catch (Exception)
                {
                }
            }
            proc.WaitForExit();
            return proc.ExitCode;
        }

        public void Stop()
        { 
        
        }

        private void WaitBeforeContinue()
        {
            Thread.Sleep(1000);
        }

        private int ReadLength(Socket socket, byte[] buffer)
        {
            int length = -1;
            string msg = this.Read(socket, buffer);
            if (msg != null)
            {
                try
                {
                    length = int.Parse(msg, System.Globalization.NumberStyles.HexNumber);
                }
                catch (FormatException)
                {
                }
            }
            return length;
        }

        private string Read(Socket socket, byte[] buffer)
        {
            bool isRead = DeviceMonitorHelper.GetInstance().Read(socket, buffer);
            return isRead == false ? string.Empty : buffer.GetString(DeviceMonitorHelper.DEFAULT_ENCODING);
        }

        /// <summary>
        /// Processes the incoming device data.
        /// </summary>
        /// <param name="length">The length.</param>
        private void ProcessIncomingDeviceData(int length)
        {
            List<Device> list = new List<Device>();

            if (length > 0)
            {
                byte[] buffer = new byte[length];
                String result = Read(_socket, buffer);

                String[] devices = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                devices.ForEach(d =>
                {
                    try
                    {
                        var dv = Device.CreateFromAdbData(d);
                        if (dv != null)
                        {
                            list.Add(dv);
                        }
                    }
                    catch (ArgumentException ae)
                    {
                    }
                });
            }

            // now merge the new devices with the old ones.
            //UpdateDevices(list);

            if (UpdateDevicesEvent != null)
            {
                UpdateDevicesEventArgs args = new UpdateDevicesEventArgs();
                args.Devices = new List<IDevice>();
                foreach (var item in list)
                {
                    args.Devices.Add(item);
                }
                UpdateDevicesEvent(this, args);
            }
        }

     

    }
}
