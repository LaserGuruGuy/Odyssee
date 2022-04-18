using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Audyssey.MultEQAvr;
using Audyssey.MultEQTcpClient;

namespace Audyssey
{
    namespace MultEQTcp
    {
        public delegate void CmdAckCallBack(bool IsAck);

        public class AudysseyMultEQAvrTcp : INotifyPropertyChanged
        {
            private AudysseyMultEQAvr AudysseyMultEQAvr = null;
            private AudysseyMultEQAvrTcpClient AudysseyMultEQAvrTcpClient = null;

            private const string INPROGRESS = "INPROGRESS";
            private const string NACK = "NACK";
            private const string ACK = "ACK";
            private const byte EOT = 0x04;
            private const byte ESC = 0x1B;

            CmdAck cmdAck = new();

            public AudysseyMultEQAvrTcp(ref AudysseyMultEQAvr audysseyMultEQAvr, string ClientAddress = "127.0.0.1", int ClientPort = 1256, int ClientTimeout = 5000)
            {
                AudysseyMultEQAvr = audysseyMultEQAvr;
                AudysseyMultEQAvrTcpClient = new(ClientAddress, ClientPort, ClientTimeout, AvrConnectCallback, null, null, Populate);
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

            public bool GetAvrInfo(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "GET_AVRINF";
                    // build JSON and replace values with "?"
                    string AvrString = MakeQuery(JsonConvert.SerializeObject(AudysseyMultEQAvr.AvrInfo, new JsonSerializerSettings {
                        ContractResolver = new InterfaceContractResolver(typeof(AvrInfo))
                    }));
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }
            
            public bool GetAvrStatus(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "GET_AVRSTS";
                    // build JSON and replace values with "?"
                    string AvrString = MakeQuery(JsonConvert.SerializeObject(AudysseyMultEQAvr.AvrStatus, new JsonSerializerSettings {
                        ContractResolver = new InterfaceContractResolver(typeof(AvrStatus))
                    }));
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool EnterAudysseyMode(CmdAckCallBack CallBack = null)
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
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool ExitAudysseyMode(CmdAckCallBack CallBack = null)
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
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool StartLvLm(CmdAckCallBack CallBack = null)
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
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool AbortOprt(CmdAckCallBack CallBack = null)
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
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool SetPosNum(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // set the first position number
                    string CmdString = "SET_POSNUM";
                    // build JSON {"Position":1,"ChSetup":["FL","FR"]};
                    string AvrString = JsonConvert.SerializeObject(AudysseyMultEQAvr.MeasuredPosition, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IMeasuredPosition))
                    });
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool StartChnl(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // start the channel (chirp)
                    string CmdString = "START_CHNL";
                    // build JSON {"Channel":"FL"}
                    string AvrString = JsonConvert.SerializeObject(AudysseyMultEQAvr.MeasuredChannel, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IMeasuredChannel)),
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool GetRespon(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // get 128 * 512 bytes channel response
                    string CmdString = "GET_RESPON";
                    // build JSON {"ChData":"FL"}
                    string AvrString = JsonConvert.SerializeObject(AudysseyMultEQAvr.ResponseData, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IResponseData)),
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool SetAmp(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // clear finflag
                    AudysseyMultEQAvr.AudyFinFlg = "NotFin";  //TODO what does this flag do?
                    string CmdString = "SET_SETDAT";
                    // build JSON for class Dat on interface Iamp
                    string AvrString = JsonConvert.SerializeObject(AudysseyMultEQAvr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IAmp))
                    });
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit request
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // callback
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool SetAudy(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "SET_SETDAT";
                    // build JSON for class Dat on interface IAudy
                    string AvrString = JsonConvert.SerializeObject(AudysseyMultEQAvr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IAudy))
                    });
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit request
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // callback
                    cmdAck.Rqst(CallBack);
                    // return command was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool SetAvrSetDisFil(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    foreach (var eq in AudysseyMultEQAvr.AudyEqSetList)
                    {
                        AudysseyMultEQAvr.EqType = eq;
                        foreach (var ch in AudysseyMultEQAvr.DetectedChannels)
                        {
                            AudysseyMultEQAvr.ChData = ch.Channel;
                            if (ch.Skip == false)
                            {
                                string CmdString = "SET_DISFIL";
                                // build JSON
                                string AvrString = JsonConvert.SerializeObject(AudysseyMultEQAvr, new JsonSerializerSettings { ContractResolver = new InterfaceContractResolver(typeof(IDisFil)) });
                                // toolbar
                                AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                                // transmit request
                                AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                                // callback
                                cmdAck.Rqst(CallBack);
                                // wait
                                while (cmdAck.Pending);
                            }
                        }
                    }
                    // return command sequence was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool SetAvrInitCoefs(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "INIT_COEFS";
                    string AvrString = "";
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit request
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // callback
                    cmdAck.Rqst(CallBack);
                    // response may take some processing time for the receiver
                    // return command sequence was issued
                    return true;
                }
                else
                {
                    // return command was not issued
                    return false;
                }
            }

            public bool AudyFinFlag(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // set finflag
                    AudysseyMultEQAvr.AudyFinFlg = "Fin";
                    string CmdString = "SET_SETDAT";
                    // build JSON for class Dat on interface Iamp
                    string AvrString = JsonConvert.SerializeObject(AudysseyMultEQAvr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IFin))
                    });
                    // toolbar
                    AudysseyMultEQAvr.Serialized += CmdString + AvrString + "\n";
                    // transmit request
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, AvrString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
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
                                                AudysseyMultEQAvr.AudysseyMode_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                AudysseyMultEQAvr.AudysseyMode_IsChecked = false;
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                AudysseyMultEQAvr.AudysseyMode_IsChecked = false;
                                                cmdAck.Nack();
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
                                                AudysseyMultEQAvr.AvrLvlm_IsChecked = false;
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                cmdAck.Nack();
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
                                                AudysseyMultEQAvr.AvrLvlm_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                AudysseyMultEQAvr.AvrLvlm_IsChecked = false;
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                AudysseyMultEQAvr.AvrLvlm_IsChecked = false;
                                                cmdAck.Nack();
                                            }
                                        }
                                        break;
                                    case "GET_AVRINF":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(string.Empty))
                                            {
                                                JsonConvert.PopulateObject(DataString, AudysseyMultEQAvr.AvrInfo, new JsonSerializerSettings
                                                {
                                                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                                                    ContractResolver = new InterfaceContractResolver(typeof(AvrInfo)),
                                                    FloatParseHandling = FloatParseHandling.Decimal
                                                });
                                                AudysseyMultEQAvr.AvrInfo_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            else
                                            {
                                                AudysseyMultEQAvr.AvrInfo_IsChecked = false;
                                            }
                                        }
                                        break;
                                    case "GET_AVRSTS":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(string.Empty))
                                            {
                                                JsonConvert.PopulateObject(DataString, AudysseyMultEQAvr.AvrStatus, new JsonSerializerSettings
                                                {
                                                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                                                    ContractResolver = new InterfaceContractResolver(typeof(AvrStatus)),
                                                    FloatParseHandling = FloatParseHandling.Decimal,
                                                });
                                                AudysseyMultEQAvr.AvrStatus_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            else
                                            {
                                                AudysseyMultEQAvr.AvrStatus_IsChecked = false;
                                            }
                                        }
                                        break;
                                    case "SET_POSNUM":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                            /*
                                            { "Position":1,"ChSetup":["FL","FR"]}
                                            { "Position":2,"ChSetup":["FL","FR"]}
                                            { "Position":3,"ChSetup":["FL","FR"]}
                                            */
                                            AudysseyMultEQAvr.MeasuredPosition = JsonConvert.DeserializeObject<MeasuredPosition>(DataString, new JsonSerializerSettings
                                            {
                                                NullValueHandling = NullValueHandling.Ignore,
                                                ObjectCreationHandling = ObjectCreationHandling.Replace,
                                            });
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            /*
                                            { "Comm":"ACK"}
                                            */
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                AudysseyMultEQAvr.SetPosNum_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                AudysseyMultEQAvr.SetPosNum_IsChecked = false;
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                AudysseyMultEQAvr.SetPosNum_IsChecked = false;
                                                cmdAck.Nack();
                                            }
                                        }
                                        break;
                                    case "START_CHNL":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                            /*
                                            {"Channel":"FL"}
                                            */
                                            // populate the channel object
                                            AudysseyMultEQAvr.MeasuredChannel = JsonConvert.DeserializeObject<MeasuredChannel>(DataString, new JsonSerializerSettings
                                            {
                                                NullValueHandling = NullValueHandling.Ignore,
                                                ObjectCreationHandling = ObjectCreationHandling.Replace,
                                            });
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            /*
                                            {"Comm":"INPROGRESS"}
                                            {"Comm":"ACK","SpConnect":"S","Polarity":"N","Distance":192,"ResponseCoef":2}                                         
                                             */
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                // populate the channel object
                                                AudysseyMultEQAvr.MeasuredChannel = JsonConvert.DeserializeObject<MeasuredChannel>(DataString, new JsonSerializerSettings
                                                {
                                                    NullValueHandling = NullValueHandling.Ignore,
                                                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                                                });
                                                AudysseyMultEQAvr.StartChnl_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                AudysseyMultEQAvr.StartChnl_IsChecked = false;
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                AudysseyMultEQAvr.StartChnl_IsChecked = true;
                                                cmdAck.Nack();
                                            }
                                        }
                                        break;
                                    case "GET_RESPON":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                            AudysseyMultEQAvr.ResponseData = JsonConvert.DeserializeObject<ResponseData>(DataString, new JsonSerializerSettings
                                            {
                                                NullValueHandling = NullValueHandling.Ignore,
                                                ObjectCreationHandling = ObjectCreationHandling.Replace,
                                            });
                                            AudysseyMultEQAvr.Serialized += "\n";
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            // We did not expect to ever get here...
                                        }
                                            break;
                                    case "SET_SETDAT":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                            JsonConvert.PopulateObject(DataString, AudysseyMultEQAvr, new JsonSerializerSettings
                                            {
                                                ObjectCreationHandling = ObjectCreationHandling.Replace,
                                                ContractResolver = new InterfaceContractResolver(typeof(IAmp)),
                                                FloatParseHandling = FloatParseHandling.Decimal,
                                            });
                                            JsonConvert.PopulateObject(DataString, AudysseyMultEQAvr, new JsonSerializerSettings
                                            {
                                                ObjectCreationHandling = ObjectCreationHandling.Replace,
                                                ContractResolver = new InterfaceContractResolver(typeof(IAudy)),
                                                FloatParseHandling = FloatParseHandling.Decimal,
                                            });
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                AudysseyMultEQAvr.AudyFinFlag_IsChecked = AudysseyMultEQAvr.AudyFinFlg.Equals("Fin");
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                cmdAck.Nack();
                                            }
                                        }
                                        break;
                                    case "SET_DISFIL":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                            JsonConvert.PopulateObject(DataString, AudysseyMultEQAvr, new JsonSerializerSettings
                                            {
                                                ObjectCreationHandling = ObjectCreationHandling.Replace,
                                                ContractResolver = new InterfaceContractResolver(typeof(IDisFil))
                                            });
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                AudysseyMultEQAvr.SetDisFil_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                AudysseyMultEQAvr.SetDisFil_IsChecked = false;
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                AudysseyMultEQAvr.SetDisFil_IsChecked = false;
                                                cmdAck.Nack();
                                            }
                                        }
                                        break;
                                    case "INIT_COEFS":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                AudysseyMultEQAvr.InitCoefs_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                AudysseyMultEQAvr.InitCoefs_IsChecked = false;
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                AudysseyMultEQAvr.InitCoefs_IsChecked = false;
                                                cmdAck.Nack();
                                            }
                                        }
                                        break;
                                    case "SET_COEFDT":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                AudysseyMultEQAvr.SetCoefDt_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                AudysseyMultEQAvr.SetCoefDt_IsChecked = false;
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                AudysseyMultEQAvr.SetCoefDt_IsChecked = false;
                                                cmdAck.Nack();
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
                                                byte[] Data = new byte[] { ESC };
                                                AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(Data, null, null);
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK) && cmdAck.Pending)
                                            {
                                                byte[] Data = new byte[] { EOT };
                                                AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(Data, null, null);
                                                cmdAck.Ack();
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
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                cmdAck.Nack();
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
                                        cmdAck.Ack();
                                    }
                                    else
                                    {
                                        AudysseyMultEQAvr.Serialized += CmdString + " segment " + CurrentPacket.ToString() + " of " + TotalPackets.ToString() + " with " + Encoding.ASCII.GetString(DataByte) + " bytes" + "\n";
                                        cmdAck.Progress();
                                    }
                                }
                                if (TransmitReceiveChar == 'R')
                                {
                                    if (TransferComplete)
                                    {
                                        AudysseyMultEQAvr.Serialized += "TODO\n";
                                        cmdAck.Ack();
                                    }
                                    else
                                    {
                                        AudysseyMultEQAvr.Serialized += CmdString + " segment " + CurrentPacket.ToString() + " of " + TotalPackets.ToString() + " with " + Encoding.ASCII.GetString(DataByte) + " bytes" + "\n";
                                        cmdAck.Progress();
                                    }
                                }
                                break;
                            case "GET_RESPON":
                                if (TransmitReceiveChar == 'R')
                                {
                                    if (TransferComplete)
                                    {
                                        AudysseyMultEQAvr.ResponseData = new() { RspData = ByteToFloatArray(DataByte) };
                                        AudysseyMultEQAvr.Serialized += "Success\n";
                                        cmdAck.Ack();
                                    }
                                    else
                                    {
                                        AudysseyMultEQAvr.Serialized += CmdString + " segment " + CurrentPacket.ToString() + " of " + TotalPackets.ToString() + " with " + Encoding.ASCII.GetString(DataByte) + " bytes" + "\n";
                                        cmdAck.Progress();
                                    }
                                }
                                break;
                            default:
                                if (TransferComplete)
                                {
                                    AudysseyMultEQAvr.Serialized += "TODO\n";
                                    cmdAck.Ack();
                                }
                                else
                                {
                                    AudysseyMultEQAvr.Serialized += CmdString + " segment " + CurrentPacket.ToString() + " of " + TotalPackets.ToString() + " with " + Encoding.ASCII.GetString(DataByte) + " bytes" + "\n";
                                    cmdAck.Progress();
                                }
                                break;
                        }
                    }
                }
            }

            public void AvrConnectCallback(bool IsConnected, string Result)
            {
                AudysseyMultEQAvr.AvrConnect_IsChecked = IsConnected;
                AudysseyMultEQAvr.Serialized += Result;
            }

            public void AvrSnifferCallback(bool IsConnected, string Result)
            {
                AudysseyMultEQAvr.SnifferAttach_IsChecked = IsConnected;
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

            public float[] ByteToFloatArray(byte[] Bytes)
            {
                if (Bytes.Length % 4 != 0) throw new ArgumentException();

                float[] result = new float[Bytes.Length / 4];
                for (int i = 0; i < Bytes.Length / 4; i++)
                {
                    result[i] = (float)(BitConverter.ToInt32(Bytes, i * 4)) / (float)(Int32.MaxValue);
                }
                return result;
            }
        }

        class AudysseyMultEQAvrComm
        {
            public string Comm { get; set; } = string.Empty;
        }

        /// <summary>
        /// Provide a wait-for-ack-with-timeout and callback-on-success-or-fail mechanism
        /// </summary>
        class CmdAck
        {
            private System.Timers.Timer Timer;
            private CmdAckCallBack Callback;

            public bool Pending { get; internal set; }

            public CmdAck()
            {
                Timer = new System.Timers.Timer();
                Timer.AutoReset = false;
                Timer.Elapsed += TimerElapsed;
                Pending = false;
                Callback = null;
            }

            /// <summary>
            /// Start the timer, set the command pending flag and load the callback function.
            /// </summary>
            /// <param name="CallBack">The timeout or acknowledge funtion (default null)</param>
            /// <param name="TimerInterval">The timeout period in ms (default 2000)</param>
            public void Rqst(CmdAckCallBack CallBack = null, int TimerInterval = 2000)
            {
                Timer.Stop();
                Pending = true;
                Callback = CallBack;
                Timer.Interval = TimerInterval;
                Timer.Start();
            }

            /// <summary>
            /// Stop the timer, clear the command pending flag and invoke the callback funtion(success).
            /// </summary>
            public void Ack()
            {
                Timer.Stop();
                Pending = false;
                Callback?.Invoke(true);
            }

            /// <summary>
            /// Restart the timer.
            /// </summary>
            /// <param name="TimerInterval">The timeout period in ms</param>
            public void Progress()
            {
                if (Timer.Enabled)
                {
                    Timer.Stop();
                    Timer.Start();
                }
            }

            /// <summary>
            /// Stop the timer, clear the command pending flag and invoke the callback funtion(fail).
            /// </summary>
            public void Nack()
            {
                Timer.Stop();
                Pending = false;
                Callback?.Invoke(false);
            }

            /// <summary>
            /// Stop the timer, clear the command pending flag and invoke the callback funtion(fail).
            /// </summary>
            private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                Pending = false;
                Callback?.Invoke(false);
            }
        }
    }
}