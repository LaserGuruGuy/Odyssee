using System.ComponentModel;
using System.Text;
using Audyssey.MultEQ;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public class DetectedChannel : MultEQList, INotifyPropertyChanged
        {
            #region BackingField
            private string _Channel = null;
            private string _Setup = null;
            private bool? _Audy = null;
            #endregion

            #region Properties
            public string Channel
            {
                get
                {
                    return _Channel;
                }
                set
                {
                    _Channel = value;
                    RaisePropertyChanged("Channel");
                }
            }
            public string Setup
            {
                get
                {
                    return _Setup;
                }
                set
                {
                    _Setup = value;
                    RaisePropertyChanged("Setup");
                }
            }
            public bool? Audy
            {
                get
                {
                    return _Audy;
                }
                set
                {
                    _Audy = value;
                    RaisePropertyChanged("Sticky");
                }
            }
            #endregion

            #region Methods
            private void ResetChannel() { _Channel = null; }
            private void ResetSetup() { _Setup = null; }
            private void ResetSticky() { _Audy = false; }
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
            protected void RaisePropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var property in this.GetType().GetProperties())
                {
                    sb.Append(property + "=" + property.GetValue(this, null) + "\r\n");
                }
                return sb.ToString();
            }
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
        }
    }
}