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
            private void RaisePropertyChanged(string propertyName)
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