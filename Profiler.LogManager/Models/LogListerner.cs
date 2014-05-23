using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Managed.Adb.Logs;
using Managed.Adb;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using Profiler.DeviceMonitor.Models;
using System.IO;
using Managed.Adb.Exceptions;
using System.Net;

namespace Profiler.LogManager.Models
{
    public class LogListerner : ILogListener, IListener
    {
        public event LogDataReceivedHandler LogDataReceivedEvent;

        private LogBuffer _logBuffer;
        private Encoding _encoding = Encoding.GetEncoding(AdbHelper.DEFAULT_ENCODING);
        private IDevice _device;
        /// <summary>
        /// /dev/log/
        /// </summary>
        private const string LOG_PATH = "/dev/log/";
        private bool _isLogging;
        private Regex _messageRegex = new Regex("^(.*)\0(.*)\0$", RegexOptions.Singleline | RegexOptions.Compiled);
        private EventLogParser m_eventParser;

        public LogListerner( IDevice device, LogBuffer logBuffer)
        {
            _device = device;
            _logBuffer = logBuffer;

            EntryDataOffset = 0;
			EntryHeaderBuffer = new byte[ENTRY_HEADER_SIZE];
			EntryHeaderOffset = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClearLogBuffer"></param>
        public void Start(bool isClearLogBuffer)
        {
            IsCancelled = false;
            ThreadStart ts = null;
            if (!_isLogging)
            {
                if (ts == null)
                {
                    ts = delegate()
                    {
                        try
                        {
                            if (isClearLogBuffer)
                            {
                                ClearLogBuffer();
                            }
                            _isLogging = true;
                            //AdbHelper.Instance.RunCatLog(AndroidDebugBridge.SocketAddress, _device, string.Format("{0}{1}", LOG_PATH, _logBuffer.ToString().ToLower()), _L_Receiver);
                            //AdbHelper.Instance.RunCatLog(AndroidDebugBridge.SocketAddress, _device, _logBuffer.ToString().ToLower(), _L_Receiver);
                            this.RunCatLog(AndroidDebugBridge.SocketAddress, _device, string.Format("{0}{1}", LOG_PATH, _logBuffer.ToString().ToLower()));
                        }
                        catch (ThreadAbortException)
                        {
                            Thread.ResetAbort();
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            _isLogging = false;
                        }
                    };
                }

                Thread logger = new Thread(ts) { 
                IsBackground = true
                };
                logger.Start(); 
            }
        }

        public void Stop(bool isClearLogBuffer)
        {
            _isLogging = false;

            IsCancelled = true;
            System.Windows.Application.Current.Dispatcher.Invoke(CancelAction);
        }



        public void ClearLogBuffer()
        {
            //this._device.ExecuteShellCommand(string.Format("logcat -c -b {0}", this._logBuffer.ToString().ToLower()), NullOutputReceiver.Instance, TABInterface.DEFAULT_ADB_TIMEOUT);
        }

        private void OnDataReceived(ILogData logData)
        {
            if (LogDataReceivedEvent != null)
            {
                LogDataReceivedEventArgs args = new LogDataReceivedEventArgs();
                args.Log = logData;
                LogDataReceivedEvent(this, args);
            }
        }

        public void RunCatLog(IPEndPoint address, IDevice device, string filePath)
        {
            if (device.IsOnline)
            {
                int num = 0x1400;
                Action action = null;
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(true, 0));
                    socket.ReceiveTimeout = -1;
                    socket.ReceiveBufferSize = num + 1;
                    socket.Blocking = true;
                    socket.Connect(AndroidDebugBridge.SocketAddress);
                    if (action == null)
                    {
                        action = delegate
                        {
                            if (socket != null)
                            {
                                socket.Close();
                            }
                        };
                    }
                    CancelAction = action;
                    AdbHelper.Instance.SetDevice(socket, device);
                    byte[] data = AdbHelper.Instance.FormAdbRequest(string.Format("shell:cat {0}", filePath));
                    //byte[] data = AdbHelper.Instance.FormAdbRequest(string.Format("log:{0}", filePath));
                    if (!AdbHelper.Instance.Write(socket, data))
                    {
                        throw new AdbException("failed submitting shell command");
                    }
                    AdbResponse response = AdbHelper.Instance.ReadAdbResponse(socket, false);
                    if (!response.IOSuccess || !response.Okay)
                    {
                        throw new AdbException("sad result from adb: " + response.Message);
                    }
                    byte[] buffer = new byte[num + 1];
                    byte num2 = 0;
                    while (device.IsOnline && !IsCancelled)
                    {
                        int num3 = socket.Receive(buffer);
                        if (num3 > 0)
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                for (int i = 0; i < num3; i++)
                                {
                                    if ((((num3 > (i + 1)) && (buffer[i] == 13)) && (buffer[i + 1] == 10)) || ((num2 == 13) && (buffer[i] == 10)))
                                    {
                                        stream.WriteByte(10);
                                        i++;
                                    }
                                    else
                                    {
                                        stream.WriteByte(buffer[i]);
                                    }
                                    if (i == (num3 - 1))
                                    {
                                        num2 = buffer[i];
                                    }
                                }
                                this.ParseNewData(stream.ToArray(), 0, (int)stream.Length);
                            }
                        }
                    }
                }
                catch (SocketException exception)
                {
                    if (exception.SocketErrorCode != SocketError.ConnectionAborted)
                    {
                        Log.e("Socket error while receiving response", exception);
                    }
                }
                finally
                {
                    if (socket != null)
                    {
                        socket.Close();
                        socket.Dispose();
                    }
                }
            }
        }

        private const int ENTRY_HEADER_SIZE = 20; // 2*2 + 4*4; see LogEntry.

		private int EntryDataOffset { get; set; }
		private int EntryHeaderOffset { get; set; }
		private byte[] EntryHeaderBuffer { get; set; }

		private LogEntry CurrentEntry { get; set; }
		public bool IsCancelled { get; private set; }

        public Action CancelAction { get; set; }
		public void ParseNewData ( byte[] data, int offset, int length ) {
            //// notify the listener of new raw data
            //if ( Listener != null ) {
            //    Listener.NewData ( data, offset, length );
            //}

			// loop while there is still data to be read and the receiver has not be cancelled.
			while ( length > 0 && !IsCancelled ) {
				// first check if we have no current entry.
				if ( CurrentEntry == null ) {
					if ( EntryHeaderOffset + length < ENTRY_HEADER_SIZE ) {
						// if we don't have enough data to finish the header, save
						// the data we have and return
						Array.Copy ( data, offset, EntryHeaderBuffer, EntryHeaderOffset, length );
						EntryHeaderOffset += length;
						return;
					} else {
						// we have enough to fill the header, let's do it.
						// did we store some part at the beginning of the header?
						if ( EntryHeaderOffset != 0 ) {
							// copy the rest of the entry header into the header buffer
							int size = ENTRY_HEADER_SIZE - EntryHeaderOffset;
							Array.Copy ( data, offset, EntryHeaderBuffer, EntryHeaderOffset, size );

							// create the entry from the header buffer
							CurrentEntry = CreateEntry ( EntryHeaderBuffer, 0 );

							// since we used the whole entry header buffer, we reset  the offset
							EntryHeaderOffset = 0;

							// adjust current offset and remaining length to the beginning
							// of the entry data
							offset += size;
							length -= size;
						} else {
							// create the entry directly from the data array
							CurrentEntry = CreateEntry ( data, offset );
							// adjust current offset and remaining length to the beginning
							// of the entry data
							offset += ENTRY_HEADER_SIZE;
							length -= ENTRY_HEADER_SIZE;
						}
					}
				}


				// at this point, we have an entry, and offset/length have been updated to skip
				// the entry header.
				if ( length >= CurrentEntry.Length - EntryDataOffset ) {
					// compute and save the size of the data that we have to read for this entry,
					// based on how much we may already have read.
					int dataSize = CurrentEntry.Length - EntryDataOffset;

					// we only read what we need, and put it in the entry buffer.
					Array.Copy ( data, offset, CurrentEntry.Data, EntryDataOffset, dataSize );

                    //// notify the listener of a new entry
                    //if ( Listener != null ) {
                    //    Listener.NewEntry ( CurrentEntry );
                    //}

                    this.NewEntry(CurrentEntry);


					// reset some flags: we have read 0 data of the current entry.
					// and we have no current entry being read.
					EntryDataOffset = 0;
					CurrentEntry = null;

					// and update the data buffer info to the end of the current entry / start
					// of the next one.
					offset += dataSize;
					length -= dataSize;
				} else {
					// we don't have enough data to fill this entry, so we store what we have
					// in the entry itself.
					Array.Copy ( data, offset, CurrentEntry.Data, EntryDataOffset, length );

					// save the amount read for the data.
					EntryDataOffset += length;
					return;
				}
			}
		}


		private LogEntry CreateEntry ( byte[] data, int offset ) {
			if ( data.Length < offset + ENTRY_HEADER_SIZE ) {
				throw new ArgumentException ( "Buffer not big enough to hold full LoggerEntry header" );
			}

			// create the new entry and fill it.
			LogEntry entry = new LogEntry ( );
			entry.Length = data.SwapU16bitFromArray ( offset );

			// we've read only 16 bits, but since there's also a 16 bit padding,
			// we can skip right over both.
			offset += 4;

			entry.ProcessId = data.Swap32bitFromArray ( offset );
			offset += 4;
			entry.ThreadId = data.Swap32bitFromArray ( offset );
			offset += 4;
			var sec = data.Swap32bitFromArray ( offset );
			
			offset += 4;
			entry.NanoSeconds = data.Swap32bitFromArray ( offset );
			offset += 4;

			// allocate the data
			entry.Data = new byte[entry.Length];

			return entry;
		}

        public void NewEntry(LogEntry entry)
        {
            DateTime now = DateTime.Now;
            if ((this._logBuffer == LogBuffer.Events) && (this.m_eventParser != null))
            {
                EventContainer container = this.m_eventParser.Parse(entry);
                if (container != null)
                {
                    LogData data = new LogData
                    {
                        LocalTimestamp = now,
                        SourceBuffer = this._logBuffer,
                        Priority = LogPriority.Info,
                        ProcessId = entry.ProcessId,
                        ThreadId = entry.ThreadId,
                        Seconds = entry.NanoSeconds,
                        NanoSeconds = entry.NanoSeconds,
                        Tag = container.TagName,
                        Data = container.ToString().TrimEnd(new char[] { '\r', '\n', ' ' }),
                        EventData = container
                    };
                    this.OnDataReceived(data);
                }
            }
            else if (entry.Data.Length > 1)
            {
                int num = 0xff & entry.Data[0];
                string input = this._encoding.GetString(entry.Data, 1, entry.Data.Length - 1);
                Match match = this._messageRegex.Match(input);
                if (match.Success)
                {
                    LogData data2 = new LogData
                    {
                        LocalTimestamp = now,
                        SourceBuffer = this._logBuffer,
                        Priority = (LogPriority)num,
                        ProcessId = entry.ProcessId,
                        ThreadId = entry.ThreadId,
                        Seconds = entry.NanoSeconds,
                        NanoSeconds = entry.NanoSeconds,
                        Tag = match.Groups[1].Value,
                        Data = match.Groups[2].Value.TrimEnd(new char[] { '\r', '\n', ' ' })
                    };
                    this.OnDataReceived(data2);
                }
            }

        }

        public void NewData(byte[] data, int offset, int length)
        {
        
        }
    }
}
