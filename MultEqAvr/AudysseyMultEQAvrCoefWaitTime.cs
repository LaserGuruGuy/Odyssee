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

            #region ResetMethods
            public void ResetInit()
            {
                _Init = null;
                RaisePropertyChanged("Init");
            }
            public void ResetFinal()
            {
                _Final = null;
                RaisePropertyChanged("Final");
            }
            #endregion

            #region INotifyPropertyChanged implementation
            public event PropertyChangedEventHandler PropertyChanged = delegate { };
            #endregion

            #region methods
            private void RaisePropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion
        }
    }
}