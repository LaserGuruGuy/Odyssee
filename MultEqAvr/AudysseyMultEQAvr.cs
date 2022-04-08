using Newtonsoft.Json;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public partial class AudysseyMultEQAvr : INotifyPropertyChanged
        {
            #region BackingField
            private string _Serialized;
            private bool _AvrConnect_IsChecked = false;
            private bool _SnifferAttach_IsChecked = false;
            private bool _AvrLvlm_IsChecked = false;
            private bool _AvrInfo_IsChecked = false;
            private bool _AvrStatus_IsChecked = false;
            private bool _AudysseyMode_IsChecked = false;
            private bool _AudyFinFlag_IsChecked = false;
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
            #endregion

            #region Methods
            public AudysseyMultEQAvr()
            {
            }
            public void ResetSerialized() { _Serialized = string.Empty;  }
            public void ResetAvrConnect_IsChecked() { _AvrConnect_IsChecked = false;  }
            public void ResetSnifferAttach_IsChecked() { _SnifferAttach_IsChecked = false; }
            public void ResetAvrLvlm_IsChecked() { _AvrLvlm_IsChecked = false; }
            public void ResetAvrInfo_IsChecked() { _AvrInfo_IsChecked = false; }
            public void ResetAvrStatus_IsChecked() { _AvrStatus_IsChecked = false; }
            public void ResetAudysseyMode_IsChecked() { _AudysseyMode_IsChecked = false; }
            public void ResetAudyFinFlag_IsChecked() { _AudyFinFlag_IsChecked = false; }
            public void Reset()
            {
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(GetType()))
                {
                    if (prop.CanResetValue(this))
                    {
                        prop.ResetValue(this);
                    }
                }
                RaisePropertyChanged("");
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