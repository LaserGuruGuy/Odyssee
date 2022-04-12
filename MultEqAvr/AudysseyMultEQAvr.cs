using Audyssey.MultEQ;
using Audyssey.MultEQ.List;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public interface IPosNum
        {
            public int Position { get; set; }
            public ObservableCollection<string> ChSetup { get; set; }
        }

        public class MeasuredPositionChSetup : IPosNum
        {
            public int Position { get; set; }
            public ObservableCollection<string> ChSetup { get; set; } = new();
        }

        public partial class AudysseyMultEQAvr : MultEQList, INotifyPropertyChanged
        {
            #region TODO BackingField
            private UniqueObservableCollection<DetectedChannel> _DetectedChannels = new();
            private DetectedChannel _SelectedChannel;
            private int _NumPos = 1;
            #endregion

            #region TODO Properties
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
            public MeasuredPositionChSetup MeasuredPositionChSetup
            {
                get
                {
                    MeasuredPositionChSetup _MeasuredPositionChSetup = new();
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            // add al channels we do not want to skip to the Collection 
                            if (ch.Skip == false)
                            {
                                _MeasuredPositionChSetup.ChSetup.Add(ch.Channel);
                            }
                            // create the Dictionary if it does not exist
                            if (ch.ResponseData == null)
                            {
                                ch.ResponseData = new();
                            }
                            // the position number is the the number of key/value pairs contained in the Dictionary
                            _MeasuredPositionChSetup.Position = ch.ResponseData.Count + 1;
                        }
                    }
                    return _MeasuredPositionChSetup;
                }
                set
                {
                    if (DetectedChannels != null)
                    {
                        // flag all channels the sniffer captured to not skip
                        foreach (var ch in DetectedChannels)
                        {
                            if (value.ChSetup.Contains(ch.Channel))
                            {
                                ch.Skip = false;
                            }
                            else
                            {
                                ch.Skip = true;
                            }
                        }
                        RaisePropertyChanged("DetectedChannels");
                    }
                }
            }
            public DetectedChannel MeasuredChannel
            {
                get
                {
                    DetectedChannel _DetectedChannel = null;
                    if (DetectedChannels != null)
                    {
                        // find the channel without skip with the smallest responsedata Dictonary count
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Skip == false)
                            {
                                if (_DetectedChannel == null)
                                {
                                    _DetectedChannel = ch;
                                }
                                if (ch.SpConnect == null && ch.Polarity == null && ch.Distance == null && ch.ResponseCoef == null)
                                {
                                    _DetectedChannel = ch;
                                    break;
                                }
                                //else if (ch.ResponseData.Count < _DetectedChannel.ResponseData.Count)
                                //{
                                //    _DetectedChannel = ch;
                                //}
                            }
                        }
                    }
                    return _DetectedChannel;
                }
                set
                {
                    DetectedChannel _DetectedChannel = null;
                    if (DetectedChannels != null)
                    {
                        // find the channel without skip with the smallest responsedata Dictonary count
                        foreach (var ch in DetectedChannels)
                        {
                            if (value.Channel == null)
                            {
                                if (ch.Skip == false)
                                {
                                    if (_DetectedChannel == null)
                                    {
                                        _DetectedChannel = ch;
                                    }
                                    if (ch.SpConnect == null && ch.Polarity == null && ch.Distance == null && ch.ResponseCoef == null)
                                    {
                                        _DetectedChannel = ch;
                                        break;
                                    }
                                    //else if (ch.ResponseData.Count < _DetectedChannel.ResponseData.Count)
                                    //{
                                    //    _DetectedChannel = ch;
                                    //}
                                }
                                _DetectedChannel.SpConnect = value.SpConnect;
                                _DetectedChannel.Polarity = value.Polarity;
                                _DetectedChannel.Distance = value.Distance;
                                _DetectedChannel.ResponseCoef = value.ResponseCoef;
                            }
                            else if (value.Channel.Equals(ch.Channel))
                            {
                                _DetectedChannel = ch;
                                _DetectedChannel.SpConnect = null;
                                _DetectedChannel.Polarity = null;
                                _DetectedChannel.Distance = null;
                                _DetectedChannel.ResponseCoef = null;
                            }
                        }
                        RaisePropertyChanged("DetectedChannels");
                    }
                }
            }

            public int NumPos { get { return _NumPos; } set { _NumPos = value; RaisePropertyChanged("NumPos"); } }
            #endregion

            #region TODO Methods
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
            private void ResetSerialized() { _Serialized = string.Empty;  }
            private void ResetAvrConnect_IsChecked() { _AvrConnect_IsChecked = false;  }
            private void ResetSnifferAttach_IsChecked() { _SnifferAttach_IsChecked = false; }
            private void ResetAvrLvlm_IsChecked() { _AvrLvlm_IsChecked = false; }
            private void ResetAvrInfo_IsChecked() { _AvrInfo_IsChecked = false; RaisePropertyChanged("Inspector_IsChecked"); }
            private void ResetAvrStatus_IsChecked() { _AvrStatus_IsChecked = false; RaisePropertyChanged("Inspector_IsChecked"); }
            private void ResetAudysseyMode_IsChecked() { _AudysseyMode_IsChecked = false; }
            private void ResetAudyFinFlag_IsChecked() { _AudyFinFlag_IsChecked = false; }
            private void ResetSetPosNum_IsChecked() { _SetPosNum_IsChecked = false; }
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