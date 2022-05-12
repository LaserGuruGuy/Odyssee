using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Audyssey.MultEQAvr;
using Audyssey.MultEQTcpClient;
using Audyssey.MultEQ.List;
using System.Threading.Tasks;

namespace Audyssey
{
    namespace MultEQTcp
    {
        public delegate void CmdAckCallBack(string Response);

        public class AudysseyMultEQAvrTcp : INotifyPropertyChanged
        {
            private AudysseyMultEQAvr AudysseyMultEQAvr = null;
            private AudysseyMultEQAvrTcpClient AudysseyMultEQAvrTcpClient = null;

            private const string INPROGRESS = "INPROGRESS";
            private const string NACK = "NACK";
            private const string ACK = "ACK";
            private const byte EOT = 0x04;
            private const byte ESC = 0x1B;

            private ResponseData _ResponseData = new();

            private CmdAck cmdAck = new();

            public AudysseyMultEQAvrTcp(ref AudysseyMultEQAvr audysseyMultEQAvr, string ClientAddress = "127.0.0.1", int ClientPort = 1256, int ClientTimeout = 5000)
            {
                AudysseyMultEQAvr = audysseyMultEQAvr;
                AudysseyMultEQAvrTcpClient = new(ClientAddress, ClientPort, ClientTimeout, AvrConnectCallback, AvrReceiveCallback);
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

            public bool Connected
            {
                get
                {
                    return AudysseyMultEQAvrTcpClient.Connected;
                }
            }

            public void AvrConnectCallback(bool IsConnected, string Result)
            {
                AudysseyMultEQAvr.AvrConnect_IsChecked = IsConnected;
                AudysseyMultEQAvr.StatusBar(Result);
            }

            public async void AvrReceiveCallback(char TransmitReceiveChar, string CmdString, byte[] DataByte, byte CurrentSegment, byte TotalSegments, bool TransferComplete)
            {
                if (AudysseyMultEQAvr != null)
                {
                    if (TotalSegments == 0)
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
                                JsonConvert.PopulateObject(DataString, Response, new JsonSerializerSettings
                                {
                                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                // echo the datastring received from the AVR on screen
                                AudysseyMultEQAvr.StatusBar(CmdString + DataString);
                                // parse the response
                                switch (CmdString)
                                {
                                    case "ENTER_AUDY":
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                AudysseyMultEQAvr.EnterAudysseyMode_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                AudysseyMultEQAvr.EnterAudysseyMode_IsChecked = false;
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                AudysseyMultEQAvr.EnterAudysseyMode_IsChecked = false;
                                                cmdAck.Nack();
                                            }
                                        }
                                        break;
                                    case "ABORT_OPRT":
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                AudysseyMultEQAvr.AvrLvlm_SW1_IsChecked = false;
                                                AudysseyMultEQAvr.AvrLvlm_SW2_IsChecked = false;
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
                                                AudysseyMultEQAvr.AvrLvlm_SW1_IsChecked = true;
                                                AudysseyMultEQAvr.AvrLvlm_SW2_IsChecked = true;
                                                cmdAck.Ack();
                                            }
                                            if (Response.Comm.Equals(INPROGRESS))
                                            {
                                                AudysseyMultEQAvr.AvrLvlm_SW1_IsChecked = false;
                                                AudysseyMultEQAvr.AvrLvlm_SW2_IsChecked = false;
                                                cmdAck.Progress();
                                            }
                                            if (Response.Comm.Equals(NACK))
                                            {
                                                AudysseyMultEQAvr.AvrLvlm_SW1_IsChecked = false;
                                                AudysseyMultEQAvr.AvrLvlm_SW2_IsChecked = false;
                                                cmdAck.Nack();
                                            }
                                        }
                                        break;
                                    case "GET_AVRINF":
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
                                        else if (TransmitReceiveChar == 'R')
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
                                        else if (TransmitReceiveChar == 'R')
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
                                            _ResponseData = JsonConvert.DeserializeObject<ResponseData>(DataString, new JsonSerializerSettings
                                            {
                                                NullValueHandling = NullValueHandling.Ignore,
                                                ObjectCreationHandling = ObjectCreationHandling.Replace,
                                            });
                                            foreach (var ch in AudysseyMultEQAvr.DetectedChannels)
                                            {
                                                if (ch.Channel.Equals(_ResponseData.ChData))
                                                {
                                                    AudysseyMultEQAvr.SelectedChannel = ch;
                                                    break;
                                                }
                                            }
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
                                        else if (TransmitReceiveChar == 'R')
                                        {
                                            if (Response.Comm.Equals(ACK))
                                            {
                                                if (AudysseyMultEQAvr.AudyFinFlg != null)
                                                {
                                                    AudysseyMultEQAvr.AudyFinFlag_IsChecked = AudysseyMultEQAvr.AudyFinFlg.Equals("Fin");
                                                }
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
                                        else if (TransmitReceiveChar == 'R')
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
                                    case "INIT_COEFS":
                                        if (TransmitReceiveChar == 'T')
                                        {
                                        }
                                        else if (TransmitReceiveChar == 'R')
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
                                    case "EXIT_AUDMD":
                                        if (TransmitReceiveChar == 'R')
                                        {
                                            if (AudysseyMultEQAvr.SnifferAttach_IsChecked == false)
                                            {
                                                if (Response.Comm.Equals(ACK))
                                                {
                                                    AudysseyMultEQAvr.ExitAudysseyMode_IsChecked = false;
                                                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(ESC);
                                                    cmdAck.Progress();
                                                }
                                                else if (Response.Comm.Equals(NACK) && cmdAck.Pending)
                                                {
                                                    AudysseyMultEQAvr.ExitAudysseyMode_IsChecked = true;
                                                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(EOT);
                                                    cmdAck.Ack();
                                                }
                                            }
                                            else
                                            {
                                                AudysseyMultEQAvr.ExitAudysseyMode_IsChecked = false;
                                            }
                                        }
                                        break;
                                    default:
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
                            else
                            {
                                // echo the datastring received from the AVR on screen
                                AudysseyMultEQAvr.StatusBar(CmdString);
                            }
                        }
                        else
                        {
                            // echo the datastring received from the AVR on screen
                            AudysseyMultEQAvr.StatusBar(CmdString);
                        }
                    }
                    else
                    {
                        switch (CmdString)
                        {
                            case "SET_COEFDT":
                                if (TransmitReceiveChar == 'T')
                                {
                                    AudysseyMultEQAvr.StatusBar(CmdString + " Segment " + CurrentSegment + " of " + TotalSegments);
                                    if (TransferComplete)
                                    {
                                        string Channel = string.Empty;
                                        string Curve = string.Empty;
                                        string SampleRate = string.Empty;
                                        try
                                        {
                                            AudysseyMultEQAvr.CoefData = DataByte;
                                        }
                                        catch (Exception ex)
                                        {
                                            AudysseyMultEQAvr.StatusBar(ex.Message);
                                        }
                                        try
                                        {
                                            Channel = AudysseyMultEQAvr.CoefChannelList.SmartReverseLookup(DataByte[2], "Unknown");
                                        }
                                        catch (Exception ex)
                                        {
                                            AudysseyMultEQAvr.StatusBar(ex.Message);
                                        }
                                        try
                                        {
                                            Curve = AudysseyMultEQAvr.CurveFilterList[DataByte[0]];
                                        }
                                        catch (Exception ex)
                                        {
                                            AudysseyMultEQAvr.StatusBar(ex.Message);
                                        }
                                        try
                                        {
                                            SampleRate = AudysseyMultEQAvr.SampleRateList[DataByte[1]];
                                        }
                                        catch (Exception ex)
                                        {
                                            AudysseyMultEQAvr.StatusBar(ex.Message);
                                        }
                                        AudysseyMultEQAvr.StatusBar(CmdString + " " + Channel + " (" + DataByte[2] + ") " + Curve + " (" + DataByte[0] + ") " + SampleRate + " (" + DataByte[1] + ") Spare (" + DataByte[3] + ") with " + (DataByte.Length / 4 - 1) + " FIR Coefficients");
                                    }
                                }
                                break;
                            case "GET_RESPON":
                                if (TransmitReceiveChar == 'R')
                                {
                                    if (TransferComplete)
                                    {
                                        _ResponseData.RspData = DataByte;
                                        AudysseyMultEQAvr.ResponseData = _ResponseData;
                                        AudysseyMultEQAvr.GetRespon_IsChecked = true;
                                        cmdAck.Ack();
                                    }
                                    else
                                    {
                                        AudysseyMultEQAvr.GetRespon_IsChecked = false;
                                        cmdAck.Progress();
                                        AudysseyMultEQAvr.StatusBar(CmdString + " Segment " + CurrentSegment + " of " + TotalSegments);
                                    }
                                }
                                break;
                            default:
                                AudysseyMultEQAvr.StatusBar(CmdString + " Segment " + (CurrentSegment + 1) + " of " + TotalSegments);
                                if (TransferComplete)
                                {
                                    cmdAck.Ack();
                                }
                                else
                                {
                                    cmdAck.Progress();
                                }
                                break;
                        }
                    }
                }
            }

            public void GetAvrInfo(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "GET_AVRINF";
                    // build JSON and replace values with "?"
                    string AvrString = MakeQuery(JsonConvert.SerializeObject(AudysseyMultEQAvr.AvrInfo, new JsonSerializerSettings {
                        ContractResolver = new InterfaceContractResolver(typeof(AvrInfo))
                    }));
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }
            
            public void GetAvrStatus(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "GET_AVRSTS";
                    // build JSON and replace values with "?"
                    string AvrString = MakeQuery(JsonConvert.SerializeObject(AudysseyMultEQAvr.AvrStatus, new JsonSerializerSettings {
                        ContractResolver = new InterfaceContractResolver(typeof(AvrStatus))
                    }));
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public void EnterAudysseyMode(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "ENTER_AUDY";
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString);
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public void Escape(CmdAckCallBack CallBack = null)
            {
                // transmit
                AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(ESC);
                // command ack request pending, clear ack request after timeout
                cmdAck.Rqst(CallBack);
            }

            public void ExitAudysseyMode(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "EXIT_AUDMD";
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString);
                    // transmit
                    AudysseyMultEQAvrTcpClient .TransmitTcpAvrStream(CmdString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public void StartLvLmSw1(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // abort operation
                    string CmdString = "START_LVLM";
                    // build JSON
                    string AvrString = "{\"SWNum\":\"SW1\"}";
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public void StartLvLmSw2(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // abort operation
                    string CmdString = "START_LVLM";
                    // build JSON
                    string AvrString = "{\"SWNum\":\"SW2\"}";
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public void AbortOprt(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // abort operation
                    string CmdString = "ABORT_OPRT";
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString);
                    // transmit
                    AudysseyMultEQAvrTcpClient .TransmitTcpAvrStream(CmdString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public void SetPosNum(CmdAckCallBack CallBack = null)
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
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public void StartChnl(CmdAckCallBack CallBack = null)
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
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public void GetRespon(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    _ResponseData = AudysseyMultEQAvr.ResponseData;
                    // get 128 * 512 bytes channel response
                    string CmdString = "GET_RESPON";
                    // build JSON {"ChData":"FL"}
                    string AvrString = JsonConvert.SerializeObject(_ResponseData, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IResponseData)),
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                    // transmit
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public void SetAmp(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    // clear finflag
                    AudysseyMultEQAvr.AudyFinFlg = "NotFin";
                    string CmdString = "SET_SETDAT";
                    // build JSON for class Dat on interface Iamp
                    string AvrString = JsonConvert.SerializeObject(AudysseyMultEQAvr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IAmp)),
                    });
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                    // transmit request
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public void SetAudy(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "SET_SETDAT";
                    // build JSON for class Dat on interface IAudy
                    string AvrString = JsonConvert.SerializeObject(AudysseyMultEQAvr, new JsonSerializerSettings
                    {
                        ContractResolver = new InterfaceContractResolver(typeof(IAudy))
                    });
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                    // transmit request
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
            }

            public async Task SetAvrSetDisFil(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    AudysseyMultEQAvr.SetDisFil_IsChecked = false;
                    int DisFilCount = AudysseyMultEQAvr.DisFilCount;
                    int DisFilIndex = 0;
                    foreach (var eq in AudysseyMultEQAvr.AudyEqSetList)
                    {
                        AudysseyMultEQAvr.EqType = eq;
                        foreach (var ch in AudysseyMultEQAvr.DetectedChannels)
                        {
                            AudysseyMultEQAvr.ChData = ch.Channel;
                            // transmit all the channels we have data for (even if skip is set)
                            if (AudysseyMultEQAvr.FilData != null && AudysseyMultEQAvr.DispData != null)
                            {
                                string CmdString = "SET_DISFIL";
                                // build JSON
                                string AvrString = JsonConvert.SerializeObject(AudysseyMultEQAvr, new JsonSerializerSettings
                                {
                                    ContractResolver = new InterfaceContractResolver(typeof(IDisFil))
                                });
                                // check if this is the final one
                                if (++DisFilIndex >= DisFilCount) AudysseyMultEQAvr.SetDisFil_IsChecked = true;
                                // statusbar
                                AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                                // transmit request
                                AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                                // command ack request pending, clear ack request after timeout
                                await Task.Run(() => cmdAck.Rqst(CallBack));
                                // wait for ACK or timeout
                                while (cmdAck.Pending);
                            }
                        }
                    }
                }
            }

            public void SetAvrInitCoefs(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    string CmdString = "INIT_COEFS";
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString);
                    // transmit request
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString);
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack, 4000);
                }
            }

            public async Task SetAvrSetCoefDt(CmdAckCallBack CallBack = null)
            {
                if ((AudysseyMultEQAvrTcpClient != null) && (AudysseyMultEQAvr != null) && (cmdAck.Pending == false))
                {
                    AudysseyMultEQAvr.SetCoefDt_IsChecked = false;

                    int CoefDtCount = AudysseyMultEQAvr.CoefDtCount;
                    int CoefDtIndex = 0;

                    foreach (var sr in MultEQList.SampleRateList)
                    {
                        foreach (var cf in MultEQList.CurveFilterList)
                        {
                            foreach (var ch in AudysseyMultEQAvr.DetectedChannels)
                            {
                                if (cf.Equals(MultEQList.CurveFilterList[0]))
                                {
                                    foreach (var ac in ch.AudyCurveFilter)
                                    {
                                        if (ac.Key.Equals(sr))
                                        {
                                            AudysseyMultEQAvr.CoefChannel = MultEQList.CoefChannelList[ch.Channel];
                                            AudysseyMultEQAvr.CoefCurve = 0x00;
                                            AudysseyMultEQAvr.CoefSampleRate = (byte)MultEQList.SampleRateList.IndexOf(sr);

                                            CoefDtIndex++;

                                            await PumpAvrSetCoefDt(CallBack, CoefDtIndex, CoefDtCount);

                                            ch.SelectedAudyCurveFilter = new(sr, ch.AudyCurveFilter[sr]);
                                            AudysseyMultEQAvr.SelectedChannel = ch;
                                        }
                                    }
                                }
                                else if (cf.Equals(MultEQList.CurveFilterList[1]))
                                {
                                    foreach (var fc in ch.FlatCurveFilter)
                                    {
                                        if (fc.Key.Equals(sr))
                                        {
                                            AudysseyMultEQAvr.CoefChannel = MultEQList.CoefChannelList[ch.Channel];
                                            AudysseyMultEQAvr.CoefCurve = 0x01;
                                            AudysseyMultEQAvr.CoefSampleRate = (byte)MultEQList.SampleRateList.IndexOf(sr);

                                            CoefDtIndex++;

                                            await PumpAvrSetCoefDt(CallBack, CoefDtIndex, CoefDtCount);

                                            ch.SelectedFlatCurveFilter = new(sr, ch.FlatCurveFilter[sr]);
                                            AudysseyMultEQAvr.SelectedChannel = ch;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private async Task PumpAvrSetCoefDt(CmdAckCallBack CallBack = null, int CoefDtIndex = 0, int CoefDtCount = 0)
            {
                // get binary datablob
                byte[] Data = AudysseyMultEQAvr.CoefData;
                // get header
                string Channel;
                try
                {
                    Channel = AudysseyMultEQAvr.CoefChannelList.SmartReverseLookup(AudysseyMultEQAvr.CoefChannel, "Unknown");
                }
                catch (Exception ex)
                {
                    Channel = ex.Message;
                }
                string Curve;
                try
                {
                    Curve = AudysseyMultEQAvr.CurveFilterList[AudysseyMultEQAvr.CoefCurve];
                }
                catch (Exception ex)
                {
                    Curve = ex.Message;
                }
                string SampleRate;
                try
                {
                    SampleRate = AudysseyMultEQAvr.SampleRateList[AudysseyMultEQAvr.CoefSampleRate];
                }
                catch (Exception ex)
                {
                    SampleRate = ex.Message;
                }
                // write header and number of 32-bits coefficients
                AudysseyMultEQAvr.StatusBar(Channel + " (" + AudysseyMultEQAvr.CoefChannel + ") " + Curve + " (" + AudysseyMultEQAvr.CoefCurve + ") " + SampleRate + " (" +  AudysseyMultEQAvr.CoefSampleRate + ") Spare (" + AudysseyMultEQAvr.CoefSpare + ") with " + (AudysseyMultEQAvr.CoefData.Length / 4 - 1) + " FIR Coefficients");
                // transmit packets in chunks of 512 bytes
                int total_byte_packets = Data.Length / 512;
                // the last packet may have less than 512 bytes
                int last_packet_length = Data.Length - (total_byte_packets * 512);
                // count for all packets
                if (last_packet_length > 0) total_byte_packets++;
                // data ... this is a very dumb binary data pump...
                for (int current_packet = 0; current_packet < total_byte_packets; current_packet++)
                {
                    byte[] CopyData = current_packet < total_byte_packets - 1 ? new byte[512] : new byte[last_packet_length];
                    Array.Copy(Data, current_packet * 512, CopyData, 0, current_packet < total_byte_packets - 1 ? 512 : last_packet_length);
                    string CmdString = "SET_COEFDT";
                    AudysseyMultEQAvr.StatusBar(CmdString + " Segment " + current_packet + " of " + (total_byte_packets - 1) + " with " + CopyData.Length + " bytes");
                    if (CoefDtIndex >= CoefDtCount && current_packet + 1 >= total_byte_packets) AudysseyMultEQAvr.SetCoefDt_IsChecked = true;
                    AudysseyMultEQAvrTcpClient.TransmitTcpAvrStream(CmdString, CopyData, current_packet, total_byte_packets - 1);
                    // command ack request pending, clear ack request after timeout
                    await Task.Run(() => cmdAck.Rqst(CallBack));
                    // TODO: ASYNC wait...
                    while (cmdAck.Pending) System.Threading.Thread.Sleep(0);
                }
            }

            public void AudyFinFlag(CmdAckCallBack CallBack = null)
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
                    // statusbar
                    AudysseyMultEQAvr.StatusBar(CmdString + AvrString);
                    // transmit request
                    AudysseyMultEQAvrTcpClient .TransmitTcpAvrStream(CmdString, Encoding.ASCII.GetBytes(AvrString));
                    // command ack request pending, clear ack request after timeout
                    cmdAck.Rqst(CallBack);
                }
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

            //public Int32[] ByteToInt32Array(byte[] Bytes)
            //{
            //    if (Bytes.Length % 4 != 0) throw new ArgumentException();

            //    Int32[] Int32s = new Int32[Bytes.Length / 4];
            //    for (int i = 0; i < Bytes.Length / 4; i++)
            //    {
            //        Int32s[i] = BitConverter.ToInt32(Bytes, i * 4);
            //    }
            //    return Int32s;
            //}

            //public double[] ByteToDoubleArray(byte[] Bytes)
            //{
            //    if (Bytes.Length % 4 != 0) throw new ArgumentException();

            //    double[] result = new double[Bytes.Length / 4];
            //    for (int i = 0; i < Bytes.Length / 4; i++)
            //    {
            //        result[i] = (double)(BitConverter.ToInt32(Bytes, i * 4)) / (double)(Int32.MaxValue);
            //    }
            //    return result;
            //}

            //public float[] ByteToFloatArray(byte[] Bytes)
            //{
            //    if (Bytes.Length % 4 != 0) throw new ArgumentException();
            //    float[] Floats = new float[Bytes.Length / 4];
            //    Buffer.BlockCopy(Bytes, 0, Floats, 0, Bytes.Length);
            //    return Floats;
            //}

            //public byte[] FloatToByteArray(float[] Floats)
            //{
            //    byte[] Bytes = new byte[Floats.Length * 4];
            //    Buffer.BlockCopy(Floats, 0, Bytes, 0, Bytes.Length);
            //    return Bytes;
            //}
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
            
            public string Status { get; internal set; }

            public bool Pending { get; internal set; }

            public CmdAck()
            {
                Timer = new System.Timers.Timer() { AutoReset = false };
                Timer.AutoReset = false;
                Timer.Elapsed += TimerElapsed;
                Pending = false;
                Status = string.Empty;
                Callback = null;
            }

            /// <summary>
            /// Start the timer, set the command pending flag and load the callback function.
            /// </summary>
            /// <param name="CallBack">The timeout or acknowledge funtion (default null)</param>
            /// <param name="TimerInterval">The timeout period in ms (default 1000)</param>
            public void Rqst(CmdAckCallBack CallBack = null, int TimerInterval = 1000)
            {
                Timer.Stop();
                Pending = true;
                Status = "RQST";
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
                Status = "ACK";
                Callback?.Invoke(Status);
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
                    Status = "INPROGRESS";
                    Callback?.Invoke(Status);
                }
            }

            /// <summary>
            /// Stop the timer, clear the command pending flag and invoke the callback funtion(fail).
            /// </summary>
            public void Nack()
            {
                Timer.Stop();
                Pending = false;
                Status = "NACK";
                Callback?.Invoke(Status);
            }

            /// <summary>
            /// Stop the timer, clear the command pending flag and invoke the callback funtion(fail).
            /// </summary>
            private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                Pending = false;
                Status = "TIMEOUT";
                Callback?.Invoke(Status);
            }
        }
    }
}