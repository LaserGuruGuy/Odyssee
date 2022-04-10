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

        public class MeasuredChannels : IPosNum
        {
            public int Position { get; set; }
            public ObservableCollection<string> ChSetup { get; set; } = new();
        }

        public partial class AudysseyMultEQAvr : INotifyPropertyChanged
        {
            #region TODO BackingField
            private UniqueObservableCollection<DetectedChannel> _DetectedChannels;
            private DetectedChannel _SelectedChannel;
            private int _NumPos = 2;//"2";
            private MeasuredChannels _EnabledChannels;
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
            public MeasuredChannels MeasuredChannels
            {
                get
                {
                    if (DetectedChannels != null)
                    {
                        _EnabledChannels = new();
                        _EnabledChannels.Position = _Position;
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Skip == false)
                            {
                                _EnabledChannels.ChSetup.Add(ch.Channel);
                            }
                        }
                    }
                    return _EnabledChannels;
                }
                set
                {
                    _EnabledChannels = value;
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (_EnabledChannels.ChSetup.Contains(ch.Channel))
                            {
                                ch.Skip = false;
                            }
                            else
                            {
                                ch.Skip = true;
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
            #endregion

            #region Methods
            public AudysseyMultEQAvr()
            {
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

            public void PopulateDetectedChannels()
            {
                if (DetectedChannels == null) DetectedChannels = new();
                foreach (var Element in ChSetup)
                {
                    foreach (var Item in Element)
                    {
                        DetectedChannels.Add(new() { Channel = Item.Key.Replace("MIX", ""), Setup = Item.Value, Skip = Item.Value == "N" ? true : false });
                    }
                }
                foreach (var SelectedChannel in DetectedChannels)
                {
                    // TODO add response coeff

                }
                RaisePropertyChanged("DetectedChannels");
            }
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged = delegate { };
            #endregion
        }
    }
}