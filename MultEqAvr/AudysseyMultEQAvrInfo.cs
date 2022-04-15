using Audyssey.MultEQ.List;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public class AvrInfo : INotifyPropertyChanged
        {
            #region BackingField
            private string _Ifver;
            private string _DType;
            private CoefWaitTime _CoefWaitTime;
            private decimal? _ADC;
            private int? _SysDelay;
            private string _EQType;
            private bool? _SWLvlMatch;
            private bool? _LFC;
            private bool? _Auro;
            private string _Upgrade;
            #endregion

            #region Properties
            public string Ifver
            {
                get
                {
                    return _Ifver;
                }
                set
                {
                    _Ifver = value;
                    RaisePropertyChanged("Ifver");
                }
            }
            public string DType
            {
                get
                {
                    return _DType;
                }
                set
                {
                    _DType = value;
                    RaisePropertyChanged("DType");
                }
            }
            public CoefWaitTime CoefWaitTime
            {
                get
                {
                    return _CoefWaitTime;
                }
                set
                {
                    _CoefWaitTime = value;
                    RaisePropertyChanged("CoefWaitTime");
                }
            }
            public decimal? ADC
            {
                get
                {
                    return _ADC;
                }
                set
                {
                    _ADC = value;
                    RaisePropertyChanged("ADC");
                }
            }
            public int? SysDelay
            {
                get
                {
                    return _SysDelay;
                }
                set
                {
                    _SysDelay = value;
                    RaisePropertyChanged("SysDelay");
                }
            }
            public string EQType
            {
                get
                {
                    return _EQType;
                }
                set
                {
                    _EQType = value;
                    RaisePropertyChanged("EQType");
                }
            }
            public bool? SWLvlMatch
            {
                get
                {
                    return _SWLvlMatch;
                }
                set
                {
                    _SWLvlMatch = value;
                    RaisePropertyChanged("SWLvlMatch");
                }
            }
            public bool? LFC
            {
                get
                {
                    return _LFC;
                }
                set
                {
                    _LFC = value;
                    RaisePropertyChanged("LFC");
                }
            }
            public bool? Auro
            {
                get
                {
                    return _Auro;
                }
                set
                {
                    _Auro = value;
                    RaisePropertyChanged("Auro");
                }
            }
            public string Upgrade
            {
                get
                {
                    return _Upgrade;
                }
                set
                {
                    _Upgrade = value;
                    RaisePropertyChanged("Upgrade");
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
            }
            private void ResetIfver() { _Ifver = null; }
            private void ResetDType() { _DType = null; }
            private void ResetCoefWaitTime() { _CoefWaitTime = null; }
            private void ResetADC() { _ADC = null; }
            private void ResetSysDelay() { _SysDelay = null; }
            private void ResetEQType() { _EQType = null; }
            private void ResetSWLvlMatch() { _SWLvlMatch = null; }
            private void ResetLFC() { _LFC = null; }
            private void ResetAuro() { _Auro = null; }
            private void ResetUpgrade() { _Upgrade = null; }
            private void RaisePropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
        }
    }
}