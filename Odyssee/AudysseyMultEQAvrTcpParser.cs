﻿using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;
using Audyssey.MultEQAvr;
using Audyssey.MultEQTcpClient;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows;

namespace Audyssey
{
    namespace MultEQTcp
    {
        public class AudysseyMultEQAvrTcp : INotifyPropertyChanged
        {
            private AudysseyMultEQAvr AudysseyMultEQAvr = null;
            private AudysseyMultEQAvrTcpClient AudysseyMultEQAvrTcpClient = null;

            private const string AUDYFINFLG = "{\"AudyFinFlg\":\"Fin\"}";

            private const string INPROGRESS = "INPROGRESS";
            private const string NACK = "NACK";
            private const string ACK = "ACK";
            private const byte EOT = 0x04;
            private const byte ESC = 0x1B;

            CmdAck cmdAck = new();

            private bool CmdSuccess = false;

            public AudysseyMultEQAvrTcp(ref AudysseyMultEQAvr audysseyMultEQAvr, string ClientAddress = "127.0.0.1", int ClientPort = 1256, int ClientTimeout = 5000)
            {
                AudysseyMultEQAvr = audysseyMultEQAvr;
                AudysseyMultEQAvrTcpClient = new(ClientAddress, ClientPort, ClientTimeout, Log, null, null, Populate);
            }

            ~AudysseyMultEQAvrTcp()
            {
            }

            public void Open()
            {
                AudysseyMultEQAvrTcpClient.Open();
            }

            public void Close()
            {
                AudysseyMultEQAvrTcpClient.Close();
            }

            public bool GetAvrInfo(int TimeoutMs = 1000)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "GET_AVRINF";
                    // build JSON and replace values with "?"
                    string AvrString = MakeQuery(JsonConvert.SerializeObject(AudysseyMultEQAvr, new JsonSerializerSettings {
                        ContractResolver = new InterfaceContractResolver(typeof(IInfo))
                    }));
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(TimeoutMs);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }
            
            public bool GetAvrStatus(int TimeoutMs = 1000)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "GET_AVRSTS";
                    // build JSON and replace values with "?"
                    string AvrString = MakeQuery(JsonConvert.SerializeObject(AudysseyMultEQAvr, new JsonSerializerSettings {
                        ContractResolver = new InterfaceContractResolver(typeof(IStatus))
                    }));
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(TimeoutMs);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool EnterAudysseyMode(int TimeoutMs = 1000)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "ENTER_AUDY";
                    string AvrString = "";
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(TimeoutMs);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool ExitAudysseyMode(int TimeoutMs = 1000)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "EXIT_AUDMD";
                    string AvrString = "";
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(TimeoutMs);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool StartLvLm(int TimeoutMs = 1000)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // abort operation
                    string CmdString = "START_LVLM";
                    // build JSON
                    string AvrString = "{\"SWNum\":\"SW1\"}";
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(TimeoutMs);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool AbortOprt(int TimeoutMs = 1000)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // abort operation
                    string CmdString = "ABORT_OPRT";
                    // build JSON
                    string AvrString = string.Empty;
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(TimeoutMs);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool SetAudysseyFinishedFlag(int TimeoutMs = 1000)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "SET_SETDAT";
                    string AvrString = AUDYFINFLG;
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit request
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(TimeoutMs);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public void Populate(char TransmitReceiveChar, string CmdString, byte[] DataByte, byte CurrentPacket, byte TotalPackets, bool TransferComplete)
            {
                if (AudysseyMultEQAvr != null)
                {
                    if (TotalPackets == 0)
                    {
                        if (DataByte != null)
                        {
                            string DataString = Encoding.ASCII.GetString(DataByte);
                            if (!string.IsNullOrEmpty(DataString))
                            {
#if DEBUG
                                File.AppendAllText(System.Environment.CurrentDirectory + "\\" + CmdString + ".json", DataString + "\n");
#endif
                                AudysseyMultEQAvrComm Response = new();
                                // populate the datastring response so we can check for: NACK, ACK, ...
                                JsonConvert.PopulateObject(DataString, Response, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace, NullValueHandling = NullValueHandling.Ignore });
                                // echo the datastring received from the AVR on screen
                                AudysseyMultEQAvr.Serialized += CmdString + DataString + "\n";
                                // parse the response
                                switch (CmdString)
                                {
                                    case "ENTER_AUDY":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                cmdAck.Rqst(1000);
                                            }
                                        }
                                        break;
                                    case "ABORT_OPRT":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                cmdAck.Ack();
                                            }
                                        }
                                        break;
                                    case "START_LVLM":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                JsonConvert.PopulateObject(DataString, AudysseyMultEQAvr, new JsonSerializerSettings
                                                {
                                                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                                                    NullValueHandling = NullValueHandling.Ignore,
                                                    ContractResolver = new InterfaceContractResolver(typeof(ISPLValue)),
                                                });
                                                cmdAck.Ack();
                                            }
                                        }
                                        break;
                                    case "GET_AVRINF":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (!Response.Comm.Equals(NACK))
                                            {
                                                JsonConvert.PopulateObject(DataString, AudysseyMultEQAvr, new JsonSerializerSettings
                                                {
                                                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                                                    ContractResolver = new InterfaceContractResolver(typeof(IInfo)),
                                                    FloatParseHandling = FloatParseHandling.Decimal
                                                });
                                                cmdAck.Ack();
                                            }
                                        }
                                        break;
                                    case "GET_AVRSTS":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (!Response.Comm.Equals(NACK))
                                            {
                                                AudysseyMultEQAvr.ChSetup = null;
                                                JsonConvert.PopulateObject(DataString, AudysseyMultEQAvr, new JsonSerializerSettings
                                                {
                                                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                                                    ContractResolver = new InterfaceContractResolver(typeof(IStatus)),
                                                    FloatParseHandling = FloatParseHandling.Decimal,
                                                });
                                                cmdAck.Ack();
                                            }
                                        }
                                        break;
                                    case "SET_SETDAT":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK) && !Response.Comm.Equals(INPROGRESS))
                                            {
                                                CmdSuccess = true;
                                            }
                                        }
                                        break;
                                    case "SET_DISFIL":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK) && !Response.Comm.Equals(INPROGRESS))
                                            {
                                                CmdSuccess = true;
                                            }
                                        }
                                        break;
                                    case "INIT_COEFS":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK) && !Response.Comm.Equals(INPROGRESS))
                                            {
                                                CmdSuccess = true;
                                            }
                                        }
                                        break;
                                    case "SET_COEFDT":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK) && !Response.Comm.Equals(INPROGRESS))
                                            {
                                                CmdSuccess = true;
                                            }
                                        }
                                        break;
                                    case "EXIT_AUDMD":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                CmdSuccess = false;
                                                byte[] Data = new byte[] { ESC };
                                                AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(Data, null, null);
                                            }
                                            else if (Response.Comm.Equals(NACK) && !CmdSuccess)
                                            {
                                                byte[] Data = new byte[] { EOT };
                                                AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(Data, null, null);
                                                CmdSuccess = true;
                                            }
                                        }
                                        break;
                                    case "SET_POSNUM":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                CmdSuccess = true;
                                            }
                                        }
                                        break;
                                    case "START_CHNL":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(NACK))
                                            {
                                            }
                                            else if (Response.Comm.Equals(INPROGRESS))
                                            {
                                            }
                                            else if (Response.Comm.Equals(ACK))
                                            {
                                                if (Response.Comm.Equals(ACK))
                                                {
                                                    CmdSuccess = true;
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                CmdSuccess = true;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        switch (CmdString)
                        {
                            case "SET_COEFDT":
                                if (TransmitReceiveChar == 'T')
                                {
                                    if (TransferComplete)
                                    {
                                        AudysseyMultEQAvr.Serialized += "TODO\n";
                                        CmdSuccess = true;
                                    }
                                    else
                                    {
                                        AudysseyMultEQAvr.Serialized += CmdString + " segment " + CurrentPacket.ToString() + " of " + TotalPackets.ToString() + " with " + Encoding.ASCII.GetString(DataByte) + " bytes" + "\n";
                                    }
                                }
                                if (TransmitReceiveChar == 'R')
                                {
                                    if (TransferComplete)
                                    {
                                        AudysseyMultEQAvr.Serialized += "TODO\n";
                                        CmdSuccess = true;
                                    }
                                    else
                                    {
                                        AudysseyMultEQAvr.Serialized += CmdString + " segment " + CurrentPacket.ToString() + " of " + TotalPackets.ToString() + " with " + Encoding.ASCII.GetString(DataByte) + " bytes" + "\n";
                                    }
                                }
                                break;
                            case "GET_RESPON":
                                if (TransmitReceiveChar == 'T')
                                {
                                    if (TransferComplete)
                                    {
                                        AudysseyMultEQAvr.Serialized += "TODO\n";
                                        CmdSuccess = true;
                                    }
                                    else
                                    {
                                        AudysseyMultEQAvr.Serialized += CmdString + " segment " + CurrentPacket.ToString() + " of " + TotalPackets.ToString() + " with " + Encoding.ASCII.GetString(DataByte) + " bytes" + "\n";
                                    }
                                }
                                if (TransmitReceiveChar == 'R')
                                {
                                    if (TransferComplete)
                                    {
                                        AudysseyMultEQAvr.Serialized += "TODO\n";
                                        CmdSuccess = true;
                                    }
                                    else
                                    {
                                        AudysseyMultEQAvr.Serialized += CmdString + " segment " + CurrentPacket.ToString() + " of " + TotalPackets.ToString() + " with " + Encoding.ASCII.GetString(DataByte) + " bytes" + "\n";
                                    }
                                }
                                break;
                            default:
                                if (TransferComplete)
                                {
                                    AudysseyMultEQAvr.Serialized += "TODO\n";
                                    CmdSuccess = true;
                                }
                                else
                                {
                                    AudysseyMultEQAvr.Serialized += CmdString + " segment " + CurrentPacket.ToString() + " of " + TotalPackets.ToString() + " with " + Encoding.ASCII.GetString(DataByte) + " bytes" + "\n";
                                }
                                break;
                        }
                    }
                }
            }


            public void Log(string Result)
            {
                AudysseyMultEQAvr.Serialized += Result;
            }

            private string MakeQuery(string Serialized)
            {
                var SerializedJObject = JObject.Parse(Serialized);
                foreach (var prop in SerializedJObject.Properties()) { prop.Value = "?"; }
                return SerializedJObject.ToString(Formatting.None);
            }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };
            private void RaisePropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            [StructLayout(LayoutKind.Explicit, Size = 4)]
            struct FloatInt32
            {
                [FieldOffset(0)] private float Float;
                [FieldOffset(0)] private int Int32;

                private static FloatInt32 inst = new();
                public static int FloatToInt32(float value)
                {
                    inst.Float = value;
                    return inst.Int32;
                }
                public static float Int32ToFloat(int value)
                {
                    inst.Int32 = value;
                    return inst.Float;
                }
            }

            public Int32[] ByteToInt32Array(byte[] Bytes)
            {
                if (Bytes.Length % 4 != 0) throw new ArgumentException();

                Int32[] Int32s = new Int32[Bytes.Length / 4];
                for (int i = 0; i < Bytes.Length / 4; i++)
                {
                    Int32s[i] = BitConverter.ToInt32(Bytes, i * 4);
                }
                return Int32s;
            }
        }

        public class AudysseyMultEQAvrComm
        {
            public string Comm { get; set; } = string.Empty;
        }

        class CmdAck
        {
            private System.Timers.Timer Timer;

            public bool Pending { get; internal set; } = false;

            public CmdAck()
            {
                Timer = new System.Timers.Timer();
                Timer.Elapsed += TimerElapsed;
                Pending = false;
            }

            public void Rqst(int TimeoutMs = 1000)
            {
                Timer.Stop();
                Pending = true;
                Timer.Interval = TimeoutMs;
                Timer.Start();
            }

            public void Ack()
            {
                Timer.Stop();
                Pending = false;
            }

            private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                Timer.Stop();
                MessageBox.Show("Communication timeout.", "Timeout expired before ACK received.", MessageBoxButton.OK, MessageBoxImage.Error);
                Pending = false;
            }
        }
    }
}