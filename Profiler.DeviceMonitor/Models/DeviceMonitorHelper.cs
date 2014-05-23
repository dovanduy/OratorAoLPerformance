using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Threading;
using System.IO;
using Managed.Adb;
using Managed.Adb.Logs;
using System.Net;

namespace Profiler.DeviceMonitor.Models
{
    public class DeviceMonitorHelper
    {
        /// <summary>
        /// The ADB executive
        /// </summary>
        public const String ADB = "adb.exe";
        /// <summary>
        /// connect adb.exe port
        /// </summary>
        public const int ADB_PORT = 5037;
        /// <summary>
        /// default encoding 
        /// </summary>
        public static string DEFAULT_ENCODING = "ISO-8859-1";
        /// <summary>
        /// The time to wait
        /// </summary>
        private const int WAIT_TIME = 5;
        /// <summary>
        /// Default timeout values for adb connection (milliseconds)
        /// </summary>
        public const int DEFAULT_TIMEOUT = 5000; // standard delay, in ms

        public IPEndPoint SocketAddress {get; private set;}
        /// <summary>
        /// Single mode
        /// </summary>
        private static DeviceMonitorHelper _instance;
        /// <summary>
        /// synchronization Lock 
        /// </summary>
        private static object _lock = new object();
        /// <summary>
        /// Single DeviceMonitorHelper
        /// </summary>
        /// <returns></returns>
        public static DeviceMonitorHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = new DeviceMonitorHelper();
                }
            }
            return _instance;
        }

        private DeviceMonitorHelper()
        {
            SocketAddress = new IPEndPoint(IPAddress.Loopback, ADB_PORT);
        }

        /// <summary>
        /// Reads the adb response.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="readDiagString">if set to <c>true</c> [read diag string].</param>
        /// <returns></returns>
        public SocketResponse ReadAdbResponse(Socket socket, bool readDiagString)
        {
            SocketResponse resp = new SocketResponse();

            byte[] reply = new byte[4];
            if (!Read(socket, reply))
            {
                return resp;
            }
            resp.IOSuccess = true;

            if (IsOkay(reply))
            {
                resp.Okay = true;
            }
            else
            {
                readDiagString = true; // look for a reason after the FAIL
                resp.Okay = false;
            }

            // not a loop -- use "while" so we can use "break"
            while (readDiagString)
            {
                // length string is in next 4 bytes
                byte[] lenBuf = new byte[4];
                if (!Read(socket, lenBuf))
                {
                    Console.WriteLine("Expected diagnostic string not found");
                    break;
                }

                String lenStr = ReplyToString(lenBuf);

                int len;
                try
                {
                    len = int.Parse(lenStr, System.Globalization.NumberStyles.HexNumber);
                }
                catch (FormatException)
                {
                    break;
                }

                byte[] msg = new byte[len];
                if (!Read(socket, msg))
                {
                    break;
                }

                resp.Message = ReplyToString(msg);
                break;
            }

            return resp;
        }

        /// <summary>
        /// Reads the data from specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public bool Read(Socket socket, byte[] data)
        {
            try
            {
                Read(socket, data, -1);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads the data from specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="data">The data.</param>
        /// <param name="length">The length.</param>
        /// <param name="timeout">The timeout.</param>
        public void Read(Socket socket, byte [] data, int length)
        {
            int expLen = length != -1 ? length : data.Length;
            int count = -1;
            int totalRead = 0;

            while (count != 0 && totalRead < expLen)
            {
                try
                {
                    int left = expLen - totalRead;
                    int buflen = left < socket.ReceiveBufferSize ? left : socket.ReceiveBufferSize;

                    byte[] buffer = new byte[buflen];
                    socket.ReceiveBufferSize = expLen;
                    count = socket.Receive(buffer, buflen, SocketFlags.None);
                    if (count < 0)
                    {
                        throw new Exception("EOF");
                    }
                    else if (count == 0)
                    {
                        Console.WriteLine("DONE with Read");
                    }
                    else
                    {
                        Array.Copy(buffer, 0, data, totalRead, count);
                        totalRead += count;
                    }
                }
                catch (SocketException sex)
                {
                    throw new Exception(String.Format("No Data to read: {0}", sex.Message));
                }
            }

        }

        public bool Write(Socket socket, byte[] data)
        {
            bool isWritted = true;
            try
            {
                Write(socket, data, -1, DEFAULT_TIMEOUT);
            }
            catch (IOException)
            {
                isWritted = false;
            }
            return isWritted;
        }


        /// <summary>
        /// Writes the specified data to the specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="data">The data.</param>
        /// <param name="length">The length.</param>
        /// <param name="timeout">The timeout.</param>
        public void Write(Socket socket, byte [] data, int length, int timeout)
        {
            int count = -1;
            int numWaits = 0;
            try 
	        {	        
		         count = socket.Send(data,length !=-1 ? length : data.Length, SocketFlags.None);
                if(count < 0)
                {
                     throw new Exception("channel EOF");
                }
                else if(count == 0)
                {
                     if(timeout != 0 && numWaits * WAIT_TIME > timeout)
                     {
                      throw new Exception("timeout");
                     }
                     Thread.Sleep(WAIT_TIME);
                     numWaits++;
                }
                else
                {
                     numWaits = 0;
                }
	        }
	        catch (SocketException)
	        {
	        }
        }
    


        /// <summary>
        /// Convert request from string object to byte array
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public byte[] FormAdbRequest(string req)
        {
            String resultStr = string.Format("{0}{1}\n", req.Length.ToString("X4"), req);
            byte[] result = null;
            try
            {
                result = resultStr.GetBytes(DeviceMonitorHelper.DEFAULT_ENCODING);
            }
            catch (EncoderFallbackException)
            {
            }
            return result;
        }

        /// <summary>
        /// Determines whether the specified reply is okay.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns>
        ///   <c>true</c> if the specified reply is okay; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOkay(byte[] reply)
        {
            return reply[0] == (byte)'O' && reply[1] == (byte)'K'
                                && reply[2] == (byte)'A' && reply[3] == (byte)'Y';
        }

        /// <summary>
        /// Replies to string.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <returns></returns>
        public String ReplyToString(byte[] reply)
        {
            String result;
            try
            {
                result = Encoding.Default.GetString(reply);
            }
            catch (DecoderFallbackException uee)
            {
                result = "";
            }
            return result;
        }

        /// <summary>
        /// Runs the Event log service on the Device, and provides its output to the LogReceiver.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="device">The device.</param>
        /// <param name="logName">Name of the log.</param>
        /// <param name="rcvr">The RCVR.</param>
        /// <exception cref="AdbException">failed asking for log</exception>
        /// <exception cref="Managed.Adb.Exceptions.AdbCommandRejectedException"></exception>
        public void RunLogService(IPEndPoint address, IDevice device, String logName, LogReceiver rcvr)
        {
            try
            {
                using (Socket adbChan = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    adbChan.Connect(address);
                    adbChan.Blocking = true;


                    SetDevice(adbChan, device);

                    var request = FormAdbRequest("log:" + logName);
                    if (!Write(adbChan, request))
                    {
                        throw new Exception("failed asking for log");
                    }

                    var resp = ReadAdbResponse(adbChan, false /* readDiagString */);
                    if (resp.Okay == false)
                    {
                        throw new Exception(resp.Message);
                    }

                    byte[] data = new byte[16384];
                    using (var ms = new MemoryStream(data))
                    {
                        int offset = 0;

                        while (true)
                        {
                            int count;
                            if (rcvr != null && rcvr.IsCancelled)
                            {
                                break;
                            }
                            var buffer = new byte[4 * 1024];

                            count = adbChan.Receive(buffer);
                            if (count < 0)
                            {
                                break;
                            }
                            else if (count == 0)
                            {
                                try
                                {
                                    Thread.Sleep(WAIT_TIME * 5);
                                }
                                catch (ThreadInterruptedException ie)
                                {
                                }
                            }
                            else
                            {
                                ms.Write(buffer, offset, count);
                                offset += count;
                                if (rcvr != null)
                                {
                                    var d = ms.ToArray();
                                    rcvr.ParseNewData(d, 0, d.Length);
                                }
                            }
                        }
                    }

                }
            }
            finally
            {

            }
        }

        /// <summary>
        /// Sets the device.
        /// </summary>
        /// <param name="adbChan">The adb chan.</param>
        /// <param name="device">The device.</param>
        public void SetDevice(Socket adbChan, IDevice device)
        {
            // if the device is not null, then we first tell adb we're looking to talk
            // to a specific device
            if (device != null)
            {
                String msg = string.Format("{0}{1}",RequestCommands.HOST_TRANSPORT,device.SerialNumber);
                byte[] device_query = FormAdbRequest(msg);

                if (!Write(adbChan, device_query))
                {
                    throw new Exception("failed submitting device (" + device + ") request to ADB");
                }

                SocketResponse resp = ReadAdbResponse(adbChan, false /* readDiagString */);
                if (!resp.Okay)
                {
                    if (String.Compare("device not found", resp.Message, true) == 0)
                    {
                        throw new Exception(device.SerialNumber);
                    }
                    else
                    {
                        throw new Exception("device (" + device + ") request rejected: " + resp.Message);
                    }
                }
            }
        }
    }
}
