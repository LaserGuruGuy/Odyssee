using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        interface ICoefWaitTime
        {
            public decimal? Init { get; set; }
            public decimal? Final { get; set; }
        }

        public class CoefWaitTime : ICoefWaitTime, INotifyPropertyChanged
        {
            #region BackingField
            private decimal? _Init = null;
            private decimal? _Final = null;
            #endregion

            #region Properties
            public decimal? Init
            {
                get
                {
                    return _Init;
                }
                set
                {
                    _Init = value;
                    RaisePropertyChanged("Init");
                }
            }
            public decimal? Final
            {
                get
                {
                    return _Final;
                }
                set
                {
                    _Final = value;
                    RaisePropertyChanged("Final");
                }
            }
            #endregion

            #region Methods
            private void ResetInit() { _Init = null; }
            private void ResetFinal() { _Final = null; }
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