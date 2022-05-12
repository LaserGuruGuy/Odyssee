using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Audyssey.MultEQTcpStream;

namespace Audyssey
{
    namespace MultEQTcpSniffer
    {
        public delegate void AudysseyMultEQAvrTcpSnifferConnectCallback(bool IsConnected, string Result);
        public delegate void AudysseyMultEQAvrTcpSnifferReceiveCallback(bool IsCompleted);

        public class AudysseyMultEQTcpSniffer
        {
            private string _ClientName;
            private int _ClientPort;

            private string _HostName;
            private int _HostPort;

            private Socket _Socket = null;

            private byte[] _PacketData = new byte[65536];

            AudysseyMultEQAvrTcpSnifferConnectCallback _AudysseyMultEQAvrTcpSnifferConnectCallback = null;
            private AudysseyMultEQAvrTcpStream _AudysseyMultEQAvrTcpStream = null;

            public AudysseyMultEQTcpSniffer(string HostName, string ClientName, int HostPort = 0, int ClientPort = 1256, int HostTimeout = 0, int ClientTimeout = 0,
                    AudysseyMultEQAvrTcpSnifferConnectCallback AudysseyMultEQAvrTcpSnifferConnectCallback = null,
                    AudysseyMultEQAvrTcpStreamParseCallback AudysseyMultEQAvrTcpSnifferReceiveCallback = null)
            {
                _AudysseyMultEQAvrTcpSnifferConnectCallback = AudysseyMultEQAvrTcpSnifferConnectCallback;
                _AudysseyMultEQAvrTcpStream = new(AudysseyMultEQAvrTcpSnifferReceiveCallback);

                _HostName = HostName;
                _HostPort = HostPort;

                _ClientName = ClientName;
                _ClientPort = ClientPort;
            }

            ~AudysseyMultEQTcpSniffer()
            {
            }

            public void Open()
            {
                //For sniffing the socket to capture the packets has to be a raw socket, with the
                //address family being of type internetwork, and protocol being IP
                try
                {
                    IPEndPoint hostEndPoint = new(IPAddress.Parse(_HostName), _HostPort);

                    _Socket = new(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);

                    //Don't allow another socket to bind to this port.
                    _Socket.ExclusiveAddressUse = true;

                    //Set the receive buffer size
                    _Socket.ReceiveBufferSize = _PacketData.Length;

                    //Set the send buffer size to 0.
                    _Socket.SendBufferSize = 0;

                    //Set the Time To Live (TTL) to 42 router hops.
                    _Socket.Ttl = 8;

                    //Bind the socket to the host computer IP endpoint address
                    _Socket.Bind(hostEndPoint);

                    //Set the socket  options
                    _Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

                    _Socket.IOControl(IOControlCode.ReceiveAll, new byte[4] { (byte)ReceiveAll.RCVALL_ON, 0, 0, 0 }, new byte[4] { (byte)ReceiveAll.RCVALL_ON, 0, 0, 0 });

                    //Start receiving the packets asynchronously
                    _Socket.BeginReceive(_PacketData, 0, _PacketData.Length, SocketFlags.None, new AsyncCallback(OnSocketReceive), null);

                    //Log
                    _AudysseyMultEQAvrTcpSnifferConnectCallback?.Invoke(true, "Attached " + _HostName + ":" + _HostPort + " to " + _ClientName + ":" + _ClientPort);
                }
                catch (ObjectDisposedException)
                {
                    // Handle the socket being closed with an async receive pending
                }
                catch (Exception ex)
                {
                    _AudysseyMultEQAvrTcpSnifferConnectCallback?.Invoke(false, ex.Message);
                }
            }

            public void Close()
            {
                if (_Socket != null)
                {
                    try
                    {
                        _Socket.Close();
                        _AudysseyMultEQAvrTcpSnifferConnectCallback?.Invoke(false, "Detached " + _HostName + ":" + _HostPort + " from " + _ClientName + ":" + _ClientPort);
                    }
                    catch (ObjectDisposedException)
                    {
                        // Handle the socket being closed with an async receive pending
                    }
                    catch (Exception ex)
                    {
                        _AudysseyMultEQAvrTcpSnifferConnectCallback?.Invoke(false, ex.Message);
                    }
                }
            }

            private void OnSocketReceive(IAsyncResult ar)
            {
                int NumberOfBytesReceived = 0;
                try
                {
                    NumberOfBytesReceived = _Socket.EndReceive(ar);
                    if (NumberOfBytesReceived > 0)
                    {
                        FilterTcpIPPacket(_PacketData, NumberOfBytesReceived);
                        _Socket.BeginReceive(_PacketData, 0, _PacketData.Length, SocketFlags.None, new AsyncCallback(OnSocketReceive), null);
                    }
                    else
                    {
                        _Socket.Close();
                        _AudysseyMultEQAvrTcpSnifferConnectCallback?.Invoke(false, "Detached " + _HostName + ":" + _HostPort + " from " + _ClientName + ":" + _ClientPort);
                    }
                }
                catch (ObjectDisposedException)
                {
                    _AudysseyMultEQAvrTcpSnifferConnectCallback?.Invoke(false, "Socket being closed whilst an async receive pending");
                }
                catch (Exception ex)
                {
                    _AudysseyMultEQAvrTcpSnifferConnectCallback?.Invoke(true, ex.Message + ", received " + NumberOfBytesReceived + ", available " + _Socket.Available + ", buffer " + _PacketData.Length);
                }
            }

            private void FilterTcpIPPacket(byte[] byteData, int nReceived)
            {
                //Since all protocol packets are encapsulated in the IP datagram
                //so we start by parsing the IP header and see what protocol data
                //is being carried by it and filter source and destination address.
                IPHeader ipHeader = new(byteData, nReceived);
                if (ipHeader.SourceAddress.ToString().Equals(_ClientName) ||
                    ipHeader.DestinationAddress.ToString().Equals(_ClientName))
                {
                    //According to the protocol being carried by the IP datagram we parse 
                    //the data field of the datagram if it carries TCP protocol
                    if ((ipHeader.ProtocolType == Protocol.TCP) && (ipHeader.MessageLength > 0))
                    {
                        TCPHeader tcpHeader = new(ipHeader.Data, ipHeader.MessageLength);
                        //Filter traffic per port
                        if (tcpHeader.SourcePort.Equals(_ClientPort.ToString()) ||
                            tcpHeader.DestinationPort.Equals(_ClientPort.ToString()))
                        {
                            if (tcpHeader.MessageLength > 0)
                            {
                                try
                                {
                                    _AudysseyMultEQAvrTcpStream.Unpack(tcpHeader.Data, tcpHeader.MessageLength);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                    }
                }
            }

            private enum ReceiveAll
            {
                RCVALL_OFF = 0,
                RCVALL_ON = 1,
                RCVALL_SOCKETLEVELONLY = 2,
                RCVALL_IPLEVEL = 3,
            }
        }
    }
}