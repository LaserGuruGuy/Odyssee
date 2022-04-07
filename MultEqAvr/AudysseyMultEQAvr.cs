using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public partial class AudysseyMultEQAvr : INotifyPropertyChanged
        {
            private bool _AvrLvlm_IsChecked = false;
            public bool AvrLvlm_IsChecked { get {return _AvrLvlm_IsChecked; } set { _AvrLvlm_IsChecked = value; RaisePropertyChanged("AvrLvlm_IsChecked"); } }

            bool _AvrInfo_IsChecked = false;
            public bool AvrInfo_IsChecked { get { return _AvrInfo_IsChecked; } set { _AvrInfo_IsChecked = value; RaisePropertyChanged("AvrInfo_IsChecked"); } }

            bool _AvrStatus_IsChecked = false;
            public bool AvrStatus_IsChecked { get { return _AvrStatus_IsChecked; } set { _AvrStatus_IsChecked = value; RaisePropertyChanged("AvrStatus_IsChecked"); } }

            bool _ConnectAudyssey_IsChecked = false;
            public bool AudysseyMode_IsChecked { get { return _ConnectAudyssey_IsChecked; } set { _ConnectAudyssey_IsChecked = value; RaisePropertyChanged("ConnectAudyssey_IsChecked"); } }

            public bool _AudyFinFlag_IsChecked = false;
            public bool AudyFinFlag_IsChecked { get { return _AudyFinFlag_IsChecked; } set { _AudyFinFlag_IsChecked = value; RaisePropertyChanged("AudyFinFlag_IsChecked"); } }

            #region BackingField
            private string _Serialized;
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
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged = delegate { };
            #endregion

            #region Methods
            public AudysseyMultEQAvr()
            {
            }
            private void RaisePropertyChanged(string propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            public void Reset()
            {
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(GetType()))
                {
                    if (prop.CanResetValue(this))
                    {
                        prop.ResetValue(this);
                    }
                }
            }
            #endregion
        }
    }
}