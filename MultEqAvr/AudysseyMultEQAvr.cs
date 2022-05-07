using Audyssey.MultEQ.List;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public interface IMeasuredPosition
        {
            public int Position { get; set; }
            public ObservableCollection<string> ChSetup { get; set; }
        }

        public class MeasuredPosition
        {
            public int Position { get; set; } = 0;
            public ObservableCollection<string> ChSetup { get; set; } = new();
        }

        public interface IMeasuredChannel
        {
            public string Channel { get; set; }
        }

        public class MeasuredChannel : ChannelReport, IMeasuredChannel, IChannelReport
        {
            public string Channel { get; set; }
        }

        public interface IResponseData
        {
            public string ChData { get; set; }
            public byte[] RspData { get; set; }
        }

        public class ResponseData : IResponseData
        {
            public string ChData { get; set; }
            public byte[] RspData { get; set; }
        }

        public class ReceiverDeviceInfo : INotifyPropertyChanged
        {
            public string Manufacturer { get; set; }
            public string FriendlyName { get; set; }
            public string ModelName { get; set; }
            public string ModelNumber { get; set; }
            public string SerialNumber { get; set; }
            public string IpAddress { get; set; }
            public int Port { get; set; }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };
        }

        public class ComputerDeviceInfo : INotifyPropertyChanged
        {
            public string Description { get; set; }
            public string IpAddress { get; set; }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };
        }

        public partial class AudysseyMultEQAvr : MultEQList, INotifyPropertyChanged
        {
            #region BackingField
            private UniqueObservableCollection<ComputerDeviceInfo> _ComputerDeviceInfo = new();
            private UniqueObservableCollection<ReceiverDeviceInfo> _ReceiverDeviceInfo = new();
            private AvrInfo _AvrInfo = new();
            private AvrStatus _AvrStatus = new();
            private UniqueObservableCollection<DetectedChannel> _DetectedChannels = new();
            private DetectedChannel _SelectedChannel;
            private int _NumPos = 8;
            private int _SmoothingFactor = 1;
            #endregion

            #region Properties
            public UniqueObservableCollection<ComputerDeviceInfo> ComputerDeviceInfo
            {
                get
                {
                    return _ComputerDeviceInfo;
                }
                set
                {
                    _ComputerDeviceInfo = value;
                    RaisePropertyChanged("ComputerDeviceInfo");
                }
            }
        
            public UniqueObservableCollection<ReceiverDeviceInfo> ReceiverDeviceInfo
            {
                get
                {
                    return _ReceiverDeviceInfo;
                }
                set
                {
                    _ReceiverDeviceInfo = value;
                    RaisePropertyChanged("ReceiverDeviceInfo");
                }
            }
            public AvrInfo AvrInfo
            {
                get
                {
                    return _AvrInfo;
                }
                set
                {
                    _AvrInfo = value;
                    RaisePropertyChanged("AvrInfo");
                }
            }
            public AvrStatus AvrStatus
            {
                get
                {
                    return _AvrStatus;
                }
                set
                {
                    _AvrStatus = value;
                    RaisePropertyChanged("AvrStatus");
                }
            }
            public UniqueObservableCollection<DetectedChannel> DetectedChannels
            {
                get
                {
                    return _DetectedChannels;
                }
                set
                {
                    if (AvrStatus != null)
                    {
                        if (AvrStatus.ChSetup != null)
                        {
                            _DetectedChannels = new();
                            foreach (var Element in AvrStatus.ChSetup)
                            {
                                foreach (var Item in Element)
                                {
                                    _DetectedChannels.Add(new() { Channel = Item.Key.Replace("MIX", ""), Setup = Item.Value, Skip = Item.Value == "N" ? true : false });
                                }
                            }
                        }
                        else
                        {
                            _DetectedChannels = value;
                        }
                    }
                    else
                    {
                        _DetectedChannels = value;
                    }
                    RaisePropertyChanged("DetectedChannels");
                }
            }
            [JsonIgnore]
            public DetectedChannel SelectedChannel
            {
                get
                {
                    return _SelectedChannel;
                }
                set
                {
                    _SelectedChannel = value;
                    RaisePropertyChanged("SelectedChannel");
                }
            }
            [JsonIgnore]
            public bool IsNextGetRespon
            {
                get
                {
                    if (DetectedChannels != null)
                    {
                        int? Count = null;
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Skip != null && ch.ResponseData != null)
                            {
                                if (ch.Skip == false)
                                {
                                    if (Count == null)
                                    { 
                                        Count = ch.ResponseData.Count;
                                        SelectedChannel = ch;
                                        if (Count > 0)
                                        {
                                            SelectedChannel.SelectedResponseData = new((ch.ResponseData.Count - 1).ToString(), ch.ResponseData[(ch.ResponseData.Count - 1).ToString()]);
                                        }
                                        else
                                        {
                                            SelectedChannel.ResetSelectedResponseData();
                                        }
                                    }
                                    else if (ch.ResponseData.Count < Count)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        SelectedChannel = ch;
                                        if (Count > 0)
                                        {
                                            SelectedChannel.SelectedResponseData = new((ch.ResponseData.Count - 1).ToString(), ch.ResponseData[(ch.ResponseData.Count - 1).ToString()]);
                                        }
                                        else
                                        {
                                            SelectedChannel.ResetSelectedResponseData();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return false;
                }
            }
            [JsonIgnore]
            public bool IsNextSetPosNum
            {
                get
                {
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Skip != null && ch.ResponseData != null)
                            {
                                if (ch.Skip == false)
                                {
                                    if (ch.ResponseData.Count < NumPos)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    return false;
                }
            }
            [JsonIgnore]
            public MeasuredPosition MeasuredPosition
            {
                // SET_POSNUM {"Position":1,"ChSetup":["FL","FR"]}
                get
                {
                    MeasuredPosition _MeasuredPosition = new();
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            // create the Dictionary if it does not exist but do not add any key/value pair ResponseData yet
                            if (ch.ResponseData == null)
                            {
                                DetectedChannels[DetectedChannels.IndexOf(ch)].ResponseData = new();
                            }
                            // the position number is the the number of key/value pairs aleady contained in the Dictionary of the first channel in the Collection
                            if (ch.Skip == false && _MeasuredPosition.Position == 0)
                            {
                                _MeasuredPosition.Position = ch.ResponseData.Count + 1;
                            }
                            // add channels we do not want to skip to the position Collection 
                            if (ch.Skip == false)
                            {
                                _MeasuredPosition.ChSetup.Add(ch.Channel);
                            }
                        }
                    }
                    return _MeasuredPosition;
                }
                set
                {
                    // flag all skip channels the sniffer captured
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (value.ChSetup.Contains(ch.Channel))
                            {
                                DetectedChannels[DetectedChannels.IndexOf(ch)].Skip = false;
                            }
                            else
                            {
                                DetectedChannels[DetectedChannels.IndexOf(ch)].Skip = true;
                            }
                        }
                        RaisePropertyChanged("DetectedChannels");
                    }
                }
            }
            [JsonIgnore]
            public MeasuredChannel MeasuredChannel
            {
                // START_CHNL {"SpConnect":"S","Polarity":"N","Distance":237,"ResponseCoef":1}
                get
                {
                    MeasuredChannel MeasuredChannel = null;
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Skip != null && ch.ChannelReport != null)
                            {
                                if (ch.Skip == false)
                                {
                                    if (string.IsNullOrEmpty(ch.ChannelReport.SpConnect))
                                    {
                                        MeasuredChannel = new() { Channel = ch.Channel };
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    return MeasuredChannel;
                }
                set
                {
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Skip != null && ch.Channel != null && ch.ChannelReport != null)
                            {
                                if (value.Channel == null)
                                {
                                    if (ch.Skip == false)
                                    {
                                        if (string.IsNullOrEmpty(ch.ChannelReport.SpConnect))
                                        {
                                            DetectedChannels[DetectedChannels.IndexOf(ch)].ChannelReport.SpConnect = value.SpConnect;
                                            DetectedChannels[DetectedChannels.IndexOf(ch)].ChannelReport.Polarity = value.Polarity;
                                            DetectedChannels[DetectedChannels.IndexOf(ch)].ChannelReport.Distance = value.Distance;
                                            DetectedChannels[DetectedChannels.IndexOf(ch)].ChannelReport.ResponseCoef = value.ResponseCoef;
                                            break;
                                        }
                                    }
                                }
                                else if (value.Channel.Equals(ch.Channel))
                                {
                                    DetectedChannels[DetectedChannels.IndexOf(ch)].ChannelReport = null;
                                    break;
                                }
                            }
                        }
                        RaisePropertyChanged("ChannelReport");
                    }
                }
            }
            [JsonIgnore]
            public ResponseData ResponseData
            {
                // GET_RESPON {"ChData":"FL"}
                get
                {
                    //find the first or default channel with the lowest responsedata count
                    if (DetectedChannels != null)
                    {
                        DetectedChannel _DetectedChannel = DetectedChannels[0];
                        foreach (var ch in _DetectedChannels)
                        {
                            if (ch.Skip != null)
                            {
                                if (ch.Skip == false)
                                {
                                    if (ch.ChannelReport != null)
                                    {
                                        if (ch.ChannelReport.SpConnect != null)
                                        {
                                            if (ch.ChannelReport.SpConnect.Equals("L") || ch.ChannelReport.SpConnect.Equals("S") || ch.ChannelReport.SpConnect.Equals("E"))
                                            {
                                                if (ch.ResponseData == null)
                                                {
                                                    return new() { ChData = ch.Channel };
                                                }
                                                else if (ch.ResponseData.Count == 0)
                                                {
                                                    return new() { ChData = ch.Channel };
                                                }
                                                else if (ch.ResponseData.Count < _DetectedChannel.ResponseData.Count)
                                                {
                                                    return new() { ChData = ch.Channel };
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return new() { ChData = string.Empty };
                }
                set
                {
                    if ((string.IsNullOrEmpty(value.ChData) == false) && value.RspData != null)
                    {
                        if (DetectedChannels != null)
                        {
                            foreach (var ch in DetectedChannels)
                            {
                                if (ch.Channel.Equals(value.ChData))
                                {
                                    if (ch.ResponseData == null)
                                    {
                                        ch.ResponseData = new();
                                    }
                                    if (ch.ChannelReport.ResponseCoef == null)
                                    {
                                        ch.ResponseData.Add(ch.ResponseData.Count.ToString(), ByteToDoubleArray(value.RspData));
                                    }
                                    else
                                    {
                                        ch.ResponseData.Add(ch.ResponseData.Count.ToString(), ByteToDoubleArray(value.RspData, (double)ch.ChannelReport.ResponseCoef));
                                    }
                                    RaisePropertyChanged("ResponseData");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            [JsonIgnore]
            public int NumPos
            {
                get
                {
                    return _NumPos;
                }
                set
                {
                    _NumPos = value;
                    RaisePropertyChanged("NumPos");
                }
            }
            [JsonIgnore]
            public int SmoothingFactor
            {
                get
                {
                    return _SmoothingFactor;
                }
                set
                {
                    _SmoothingFactor = value > 48 ? 48 : value < 0 ? 0 : value;
                    RaisePropertyChanged("SmoothingFactor");
                }
            }
            #endregion

            #region Methods
            private double[] ByteToDoubleArray(byte[] Bytes, double ResponseCoef = 1.0d)
            {
                if (Bytes.Length % 4 != 0) throw new ArgumentException();

                double[] result = new double[Bytes.Length / 4];
                for (int i = 0; i < Bytes.Length / 4; i++)
                {
                    result[i] = ResponseCoef * (double)BitConverter.ToInt32(Bytes, i * 4) / (double)Int32.MaxValue;
                }
                return result;
            }
            private void ResetAvrInfo() { /* reset class object */ AvrInfo.Reset(); }
            private void ResetAvrStatus() { /* reset class object */ AvrStatus.Reset(); }
            private void ResetDetectedChannels() { /* new collection of class objects */ _DetectedChannels = new(); }
            private void ResetSelectedChannel() { /* no object */ _SelectedChannel = null; }
            private void ResetMeasuredPosition() { /* property has no backingfield */ }
            private void ResetMeasuredChannel() { /* property has no backingfield */ }
            private void ResetNumPos() { _NumPos = 8; }
            private void ResetSmoothingFactor() { _SmoothingFactor = 1; }
            #endregion

            #region BackingField
            private string _Serialized;
            private bool _AvrConnect_IsChecked;
            private bool _SnifferAttach_IsChecked;
            private bool _AvrLvlm_SW1_IsChecked;
            private bool _AvrLvlm_SW2_IsChecked;
            private bool _AvrInfo_IsChecked;
            private bool _AvrStatus_IsChecked;
            private bool _AudysseyMode_IsChecked;
            private bool _AudyFinFlag_IsChecked;
            private bool _SetPosNum_IsChecked;
            private bool _StartChnl_IsChecked;
            private bool _GetRespon_IsChecked;
            private bool _SetAmp_IsChecked;
            private bool _SetAudy_IsChecked;
            private bool _SetDisFil_IsChecked;
            private bool _InitCoefs_IsChecked;
            private bool _SetCoefDt_IsChecked;
            #endregion

            #region Properties
            [JsonIgnore]
            public string Serialized
            {
                get
                {
                    return _Serialized;
                }
                set
                {
                    _Serialized = value;
                    RaisePropertyChanged("Serialized");
                }
            }
            [JsonIgnore]
            public bool AvrConnect_IsChecked { get { return _AvrConnect_IsChecked; } set { _AvrConnect_IsChecked = value; RaisePropertyChanged("AvrConnect_IsChecked"); } }
            [JsonIgnore]
            public bool SnifferAttach_IsChecked { get { return _SnifferAttach_IsChecked; } set { _SnifferAttach_IsChecked = value; RaisePropertyChanged("SnifferAttach_IsChecked"); } }
            [JsonIgnore]
            public bool AvrLvlm_SW1_IsChecked { get { return _AvrLvlm_SW1_IsChecked; } set { _AvrLvlm_SW1_IsChecked = value; RaisePropertyChanged("AvrLvlm_SW1_IsChecked"); } }
            [JsonIgnore]
            public bool AvrLvlm_SW2_IsChecked { get { return _AvrLvlm_SW2_IsChecked; } set { _AvrLvlm_SW2_IsChecked = value; RaisePropertyChanged("AvrLvlm_SW2_IsChecked"); } }
            [JsonIgnore]
            public bool AvrInfo_IsChecked { get { return _AvrInfo_IsChecked; } set { _AvrInfo_IsChecked = value; RaisePropertyChanged("AvrInfo_IsChecked"); RaisePropertyChanged("Inspector_IsChecked"); } }
            [JsonIgnore]
            public bool AvrStatus_IsChecked { get { return _AvrStatus_IsChecked; } set { _AvrStatus_IsChecked = value; RaisePropertyChanged("AvrStatus_IsChecked"); RaisePropertyChanged("Inspector_IsChecked"); } }
            [JsonIgnore]
            public bool Inspector_IsChecked { get { return _AvrInfo_IsChecked & _AvrStatus_IsChecked; } }
            [JsonIgnore]
            public bool AudysseyMode_IsChecked { get { return _AudysseyMode_IsChecked; } set { _AudysseyMode_IsChecked = value; RaisePropertyChanged("AudysseyMode_IsChecked"); } }
            [JsonIgnore]
            public bool AudyFinFlag_IsChecked { get { return _AudyFinFlag_IsChecked; } set { _AudyFinFlag_IsChecked = value; RaisePropertyChanged("AudyFinFlag_IsChecked"); } }
            [JsonIgnore]
            public bool SetPosNum_IsChecked { get { return _SetPosNum_IsChecked; } set { _SetPosNum_IsChecked = value; RaisePropertyChanged("SetPosNum_IsChecked"); } }
            [JsonIgnore]
            public bool StartChnl_IsChecked { get { return _StartChnl_IsChecked; } set { _StartChnl_IsChecked = value; RaisePropertyChanged("StartChnl_IsChecked"); } }

            [JsonIgnore]
            public bool GetRespon_IsChecked { get { return _GetRespon_IsChecked; } set { _GetRespon_IsChecked = value; RaisePropertyChanged("GetRespon_IsChecked"); } }
            [JsonIgnore]
            public bool SetAmp_IsChecked { get { return _SetAmp_IsChecked; } set { _SetAmp_IsChecked = value; RaisePropertyChanged("SetAmp_IsChecked"); } }
            [JsonIgnore]
            public bool SetAudy_IsChecked { get { return _SetAudy_IsChecked; } set { _SetAudy_IsChecked = value; RaisePropertyChanged("SetAudy_IsChecked"); } }
            [JsonIgnore]
            public bool SetDisFil_IsChecked { get { return _SetDisFil_IsChecked; } set { _SetDisFil_IsChecked = value; RaisePropertyChanged("SetDisFil_IsChecked"); } }
            [JsonIgnore]
            public bool InitCoefs_IsChecked { get { return _InitCoefs_IsChecked; } set { _InitCoefs_IsChecked = value; RaisePropertyChanged("InitCoefs_IsChecked"); } }
            [JsonIgnore]
            public bool SetCoefDt_IsChecked { get { return _SetCoefDt_IsChecked; } set { _SetCoefDt_IsChecked = value; RaisePropertyChanged("SetCoefDt_IsChecked"); } }
            #endregion

            #region Methods
            private void ResetSerialized() { _Serialized = string.Empty; }
            private void ResetAvrConnect_IsChecked() { /*_AvrConnect_IsChecked = false;*/  }
            private void ResetSnifferAttach_IsChecked() { /*_SnifferAttach_IsChecked = false;*/ }
            private void ResetAvrLvlm_SW1_IsChecked() { _AvrLvlm_SW1_IsChecked = false; }
            private void ResetAvrLvlm_SW2_IsChecked() { _AvrLvlm_SW2_IsChecked = false; }
            private void ResetAvrInfo_IsChecked() { _AvrInfo_IsChecked = false; }
            private void ResetAvrStatus_IsChecked() { _AvrStatus_IsChecked = false; }
            private void ResetAudysseyMode_IsChecked() { _AudysseyMode_IsChecked = false; }
            private void ResetAudyFinFlag_IsChecked() { _AudyFinFlag_IsChecked = false; }
            private void ResetSetPosNum_IsChecked() { _SetPosNum_IsChecked = false; }
            private void ResetStartChnl_IsChecked() { _StartChnl_IsChecked = false; }
            private void ResetSetAmp_IsChecked() { _SetAmp_IsChecked = false; }
            private void ResetSetAudy_IsChecked() { _SetAudy_IsChecked = false; }
            private void ResetSetDisFil_IsChecked() { _SetDisFil_IsChecked = false; }
            private void ResetInitCoefs_IsChecked() { _InitCoefs_IsChecked = false; }
            private void ResetSetCoefDt_IsChecked() { _SetCoefDt_IsChecked = false; }
            private void RaisePropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion

            #region Methods
            public void Reset()
            {
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(GetType()))
                {
                    if (prop.CanResetValue(this))
                    {
                        prop.ResetValue(this);
                        RaisePropertyChanged(prop.Name);
                    }
                }
                RaisePropertyChanged("Inspector_IsChecked");
            }
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged = delegate { };
            #endregion
        }
    }
}