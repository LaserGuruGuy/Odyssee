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
            public void ResetIfver() { _Ifver = null; }
            public void ResetDType() { _DType = null; }
            public void ResetCoefWaitTime() { _CoefWaitTime = null; }
            public void ResetADC() { _ADC = null; }
            public void ResetSysDelay() { _SysDelay = null; }
            public void ResetEQType() { _EQType = null; }
            public void ResetSWLvlMatch() { _SWLvlMatch = null; }
            public void ResetLFC() { _LFC = null; }
            public void ResetAuro() { _Auro = null; }
            public void ResetUpgrade() {_Upgrade = null; }
            #endregion
        }
    }
}