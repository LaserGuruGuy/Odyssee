﻿using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Buffers.Binary;
using Audyssey.MultEQTcpStream;

namespace Audyssey
{
    namespace MultEQTcpClient
    {
        public delegate void AudysseyMultEQAvrTcpClientConnectCallback(string Result);
        public delegate void AudysseyMultEQAvrTcpClientTransmitCallback(bool IsCompleted);
        public delegate void AudysseyMultEQAvrTcpClientReceiveCallback(bool IsCompleted);

        public class AudysseyMultEQAvrTcpClient
        {
            // TCPIP
            private string _HostName;
            private int _HostPort;
            private int _TimeoutMilliseconds;
            private TcpClient _TcpClient;
            private IAsyncResult _ConnectedResult;
            private IAsyncResult _ReadResult;
            private NetworkStream _NetworkStream = null;
            private byte[] _Buffer;

            private AudysseyMultEQAvrTcpClientConnectCallback _AudysseyMultEQAvrConnectCallBack = null;
            private AudysseyMultEQAvrTcpClientTransmitCallback _AudysseyMultEQAvrTransmitCallBack = null;
            private AudysseyMultEQAvrTcpClientReceiveCallback _AudysseyMultEQAvrReceiveCallback = null;

            private AudysseyMultEQAvrTcpStream _AudysseyMultEQAvrTcpStream = null;

            //METHODS           
            public AudysseyMultEQAvrTcpClient(string HostName, int HostPort, int TimeoutMilliseconds,
                    AudysseyMultEQAvrTcpClientConnectCallback AudysseyMultEQAvrConnectCallBack = null,
                    AudysseyMultEQAvrTcpClientTransmitCallback AudysseyMultEQAvrTransmitCallBack = null,
                    AudysseyMultEQAvrTcpClientReceiveCallback AudysseyMultEQAvrReceiveCallback = null,
                    AudysseyMultEQAvrTcpStreamParseCallback AudysseyMultEQAvrTcpStreamParseCallback = null)
            {
                _AudysseyMultEQAvrConnectCallBack = AudysseyMultEQAvrConnectCallBack;
                _AudysseyMultEQAvrTransmitCallBack = AudysseyMultEQAvrTransmitCallBack;
                _AudysseyMultEQAvrReceiveCallback = AudysseyMultEQAvrReceiveCallback;

                _AudysseyMultEQAvrTcpStream = new(AudysseyMultEQAvrTcpStreamParseCallback);

                _HostName = HostName;
                _HostPort = HostPort;
                _TimeoutMilliseconds = TimeoutMilliseconds;
            }

            ~AudysseyMultEQAvrTcpClient()
            {
            }

            public void TransmitTcpAvrStream(byte[] Data)
            {
                _NetworkStream.Write(Data, 0, 1);
            }
            
            public void TransmitTcpAvrStream(string CmdStr, string DataStr)
            {
                TransmitTcpAvrStream(CmdStr, Encoding.ASCII.GetBytes(DataStr));
            }
            
            public void TransmitTcpAvrStream(byte[] Data, string CmdStr, string DataStr)
            {
                if (Data != null)
                {
                    _NetworkStream.Write(Data, 0, 1);
                }
                else
                {
                    TransmitTcpAvrStream(CmdStr, Encoding.ASCII.GetBytes(DataStr));
                }
            }

            public void TransmitTcpAvrStream(string Cmd, Int32[] Data, int CurrentPacket = 0, int TotalPackets = 0)
            {
                TransmitTcpAvrStream(Cmd, Int32ToByte(Data), CurrentPacket, TotalPackets);
            }

            public void TransmitTcpAvrStream(string Cmd, byte[] Data, int CurrentPacket = 0, int TotalPackets = 0)
            {
                const UInt16 HeaderLength = 9;
                byte[] Command;
                UInt16 CommandLength;
                UInt16 DataLength;
                UInt16 TotalLength;

                if (Cmd != null)
                {
                    Command = Encoding.ASCII.GetBytes(Cmd);
                    CommandLength = (UInt16)Encoding.ASCII.GetByteCount(Cmd);
                }
                else
                {
                    Command = null;
                    CommandLength = 0;
                }

                if (Data != null)
                {
                    DataLength = (UInt16)Data.Length;
                }
                else
                {
                    DataLength = 0;
                }

                TotalLength = (UInt16)(HeaderLength + CommandLength + DataLength);

                MemoryStream memoryStream = new();
                BinaryWriter binaryWriter = new(memoryStream);

                binaryWriter.Write((byte)'T');
                binaryWriter.Write(BinaryPrimitives.ReverseEndianness(TotalLength));
                binaryWriter.Write((byte)CurrentPacket);
                binaryWriter.Write((byte)TotalPackets);
                binaryWriter.Write(Command);
                binaryWriter.Write((byte)0);
                binaryWriter.Write(BinaryPrimitives.ReverseEndianness(DataLength));
                if (DataLength > 0) binaryWriter.Write(Data);
                binaryWriter.Write(CalculateChecksum(memoryStream.GetBuffer()));

                _NetworkStream.BeginWrite(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, WriteCallback, null);
            }
            
            public void Open()
            {
                try
                {
                    _TcpClient = new(AddressFamily.InterNetwork);
                    _TcpClient.SendTimeout = 500;
                }
                catch (Exception ex)
                {
                    _AudysseyMultEQAvrConnectCallBack?.Invoke(ex.Message);
                    return;
                }
                try
                {
                    _ConnectedResult = _TcpClient.BeginConnect(_HostName, _HostPort, new AsyncCallback(ConnectCallback), _TcpClient);
                }
                catch (Exception ex)
                {
                    _AudysseyMultEQAvrConnectCallBack?.Invoke(ex.Message);
                }
            }

            public void Close()
            {
                _AudysseyMultEQAvrConnectCallBack?.Invoke("Disconnected from " + _TcpClient.Client.RemoteEndPoint.ToString() + "\n");
                // workaround for a .net bug: http://support.microsoft.com/kb/821625
                if (_NetworkStream != null)
                {
                    try
                    {
                        _NetworkStream.Close();
                    }
                    catch (Exception ex)
                    {
                        _AudysseyMultEQAvrConnectCallBack?.Invoke(ex.Message);
                    }
                }
                if (_TcpClient != null)
                {
                    try
                    {
                        _TcpClient.Close();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (Exception ex)
                    {
                        _AudysseyMultEQAvrConnectCallBack?.Invoke(ex.Message);
                    }
                }
            }

            private void ConnectCallback(IAsyncResult result)
            {
                try
                {
                    _NetworkStream = _TcpClient.GetStream();
                }
                catch (Exception ex)
                {
                    _AudysseyMultEQAvrConnectCallBack?.Invoke(ex.Message);
                    return;
                }
                if (_NetworkStream.CanRead)
                {
                    try
                    {
                        _Buffer = new byte[_TcpClient.ReceiveBufferSize];
                        _ReadResult = _NetworkStream.BeginRead(_Buffer, 0, _Buffer.Length, ReadCallback, _Buffer);
                        _AudysseyMultEQAvrConnectCallBack?.Invoke("Connected to " + _TcpClient.Client.RemoteEndPoint.ToString() + "\n");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _AudysseyMultEQAvrConnectCallBack?.Invoke(ex.Message);
                    }
                }
            }
            
            private void ReadCallback(IAsyncResult result)
            {
                ushort _numberOfBytesRead = 0;
                try
                {
                    _NetworkStream = _TcpClient.GetStream();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                try
                {
                    _numberOfBytesRead = (ushort)_NetworkStream.EndRead(result);
                    _Buffer = result.AsyncState as byte[];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (_NetworkStream.DataAvailable)
                {
                    Console.WriteLine("More data available, that is weird...");
                }
                _AudysseyMultEQAvrTcpStream.Unpack(_Buffer, _numberOfBytesRead);
                _AudysseyMultEQAvrReceiveCallback?.Invoke(result.IsCompleted);
                _ReadResult = _NetworkStream.BeginRead(_Buffer, 0, _Buffer.Length, ReadCallback, _Buffer);
            }

            private void WriteCallback(IAsyncResult result)
            {
                _AudysseyMultEQAvrTransmitCallBack?.Invoke(result.IsCompleted);
            }

            private byte CalculateChecksum(byte[] dataToCalculate)
            {
                return dataToCalculate.Aggregate((r, n) => r += n);
            }

            private Int32[] ByteToInt32(byte[] Byte)
            {
                Int32[] Int32s = null;
                if (Byte.Length % 4 == 0)
                {
                    Int32s = new Int32[Byte.Length / 4];
                    for (int i = 0; i < Byte.Length / 4; i++)
                    {
                        Int32s[i] = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(Byte, i * 4));
                    }
                }
                return Int32s;
            }

            private byte[] Int32ToByte(Int32[] Int32s)
            {
                byte[] Byte = new byte[4 * Int32s.Length];
                for (int i = 0; i < Int32s.Length; i++)
                {
                    Array.Copy(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(Int32s[i])), 0, Byte, i * 4, 4);
                }
                return Byte;
            }
        }
    }
}