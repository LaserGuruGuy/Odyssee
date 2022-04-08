using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public interface IInfo
        {
            #region Properties
            public string Ifver { get; set; }
            public string DType { get; set; }
            public CoefWaitTime CoefWaitTime { get; set; }
            public decimal? ADC { get; set; }
            public int? SysDelay { get; set; }
            public string EQType { get; set; }
            public bool? SWLvlMatch { get; set; }
            public bool? LFC { get; set; }
            public bool? Auro { get; set; }
            public string Upgrade { get; set; }
            #endregion
        }

        public partial class AudysseyMultEQAvr : IInfo, INotifyPropertyChanged
        {
            #region BackingField
            private string _Ifver = null;
            private string _DType = null;
            private CoefWaitTime _CoefWaitTime = null;
            private decimal? _ADC = null;
            private int? _SysDelay = null;
            private string _EQType = null;
            private bool? _SWLvlMatch = null;
            private bool? _LFC = null;
            private bool? _Auro = null;
            private string _Upgrade = null;
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
            private void ResetIfver() { _Ifver = null; }
            private void ResetDType() { _DType = null; }
            private void ResetCoefWaitTime() { _CoefWaitTime = null; }
            private void ResetADC() { _ADC = null; }
            private void ResetSysDelay() { _SysDelay = null; }
            private void ResetEQType() { _EQType = null; }
            private void ResetSWLvlMatch() { _SWLvlMatch = null; }
            private void ResetLFC() { _LFC = null; }
            private void ResetAuro() { _Auro = null; }
            private void ResetUpgrade() {_Upgrade = null; }
            #endregion
        }
    }
}