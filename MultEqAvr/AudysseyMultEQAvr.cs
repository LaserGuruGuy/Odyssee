using Audyssey.MultEQ.List;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public class MeasuredPosition
        {
            public int Position { get; set; } = 0;
            public ObservableCollection<string> ChSetup { get; set; } = new();
        }

        public class MeasuredChannel : ChannelReport
        {
            public string Channel { get; set; }
        }

        public partial class AudysseyMultEQAvr : MultEQList, INotifyPropertyChanged
        {
            #region BackingField
            private AvrInfo _AvrInfo;
            private AvrStatus _AvrStatus;
            private UniqueObservableCollection<DetectedChannel> _DetectedChannels;
            private DetectedChannel _SelectedChannel;
            private int _NumPos;
            #endregion

            #region Properties
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
                    _DetectedChannels = value;
                    RaisePropertyChanged("DetectedChannels");
                }
            }
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
            public MeasuredChannel MeasuredChannel
            {
                // START_CHNL {"SpConnect":"S","Polarity":"N","Distance":237,"ResponseCoef":1}
                get
                {
                    MeasuredChannel _MeasuredChannel = null;
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Skip == false)
                            {
                                if (string.IsNullOrEmpty(ch.ChannelReport.SpConnect))
                                {
                                    _MeasuredChannel = new() { Channel = ch.Channel };
                                    break;
                                }
                            }
                        }
                    }
                    return _MeasuredChannel;
                }
                set
                {
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
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
                        RaisePropertyChanged("ChannelReport");
                    }
                }
            }
            public int NumPos { get { return _NumPos; } set { _NumPos = value; RaisePropertyChanged("NumPos"); } }
            #endregion

            #region Methods
            public AudysseyMultEQAvr()
            {
                _AvrInfo = new();
                _AvrStatus = new(this);
                _NumPos = 1;
            }
            private void ResetDetectedChannels() { _DetectedChannels = null; }
            private void ResetSelectedChannel() { _SelectedChannel = null; }
            #endregion

            #region BackingField
            private string _Serialized;
            private bool _AvrConnect_IsChecked;
            private bool _SnifferAttach_IsChecked;
            private bool _AvrLvlm_IsChecked;
            private bool _AvrInfo_IsChecked;
            private bool _AvrStatus_IsChecked;
            private bool _AudysseyMode_IsChecked;
            private bool _AudyFinFlag_IsChecked;
            private bool _SetPosNum_IsChecked;
            private bool _StartChnl_IsChecked;
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
            public bool AvrLvlm_IsChecked { get { return _AvrLvlm_IsChecked; } set { _AvrLvlm_IsChecked = value; RaisePropertyChanged("AvrLvlm_IsChecked"); } }
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
            }
            private void ResetSerialized() { _Serialized = string.Empty;  }
            private void ResetAvrConnect_IsChecked() { _AvrConnect_IsChecked = false;  }
            private void ResetSnifferAttach_IsChecked() { _SnifferAttach_IsChecked = false; }
            private void ResetAvrLvlm_IsChecked() { _AvrLvlm_IsChecked = false; }
            private void ResetAvrInfo_IsChecked() { _AvrInfo_IsChecked = false; RaisePropertyChanged("Inspector_IsChecked"); }
            private void ResetAvrStatus_IsChecked() { _AvrStatus_IsChecked = false; RaisePropertyChanged("Inspector_IsChecked"); }
            private void ResetAudysseyMode_IsChecked() { _AudysseyMode_IsChecked = false; }
            private void ResetAudyFinFlag_IsChecked() { _AudyFinFlag_IsChecked = false; }
            private void ResetSetPosNum_IsChecked() { _SetPosNum_IsChecked = false; }
            private void RaisePropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged = delegate { };
            #endregion
        }
    }
}