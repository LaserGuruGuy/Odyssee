using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MultEQAvrAdapter.AdapterList;
using Audyssey.MultEQAvr;
using Audyssey.MultEQ.List;

namespace MultEQAvrAdapter
{
    namespace MultEQApp
    {
        public class AudysseyMultEQAvrAdapter
        {
            #region Poperties
            private AudysseyMultEQAvr _AudysseyMultEQAvr;

            public AudysseyMultEQAvrAdapter(ref AudysseyMultEQAvr AudysseyMultEQAvr)
            {
                _AudysseyMultEQAvr = AudysseyMultEQAvr;
                AudysseyMultEQAvr.PropertyChanged += PropertyChanged;
            }

            ~AudysseyMultEQAvrAdapter()
            {
                _AudysseyMultEQAvr.PropertyChanged -= PropertyChanged;
            }

            public string Title { get; set; }

            public string TargetModelName { get; set; }

            public string InterfaceVersion
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.Ifver;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.Ifver = value;
                    RaisePropertyChanged("Ifver");
                }
            }
            
            public bool? DynamicEq
            {
                get
                {
                    return _AudysseyMultEQAvr.AudyDynEq;
                }
                set
                {
                    _AudysseyMultEQAvr.AudyDynEq = value;
                    RaisePropertyChanged("AudyDynEq");
                }
            }
            
            public bool? DynamicVolume
            {
                get
                {
                    return _AudysseyMultEQAvr.AudyDynVol;
                }
                set
                {
                    _AudysseyMultEQAvr.AudyDynVol = value;
                    RaisePropertyChanged("AudyDynVol");
                }
            }
            
            public bool? LfcSupport
            {
                get
                {
                    return _AudysseyMultEQAvr.AudyLfc;
                }
                set
                {
                    _AudysseyMultEQAvr.AudyLfc = value;
                    RaisePropertyChanged("AudyLfc");
                }
            }
            
            public bool? Lfc
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.LFC;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.LFC = value;
                    RaisePropertyChanged("LFC");
                }
            }
            
            public int? SystemDelay
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.SysDelay;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.SysDelay = value;
                    RaisePropertyChanged("SysDelay");
                }
            }
            
            public decimal? AdcLineup
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.ADC;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.ADC = value;
                    RaisePropertyChanged("ADC");
                }
            }
            
            public int? EnTargetCurveType
            {
                get
                {

                    return MultEQList.AudyEqSetList.IndexOf(_AudysseyMultEQAvr.AudyEqSet);
                }
                set
                {
                    _AudysseyMultEQAvr.AudyEqSet = MultEQList.AudyEqSetList.ElementAt((int) value);
                    RaisePropertyChanged("AudyEqSetList");
                }
            }

            public int? EnAmpAssignType
            {
                get
                {
                    return AdapterList.StatusList.AmpAssignTypeList.IndexOf(_AudysseyMultEQAvr.AvrStatus.AmpAssign);
                }
                set
                {
                    _AudysseyMultEQAvr.AvrStatus.AmpAssign = AdapterList.StatusList.AmpAssignTypeList.ElementAt((int)value);
                    RaisePropertyChanged("AmpAssign");
                }
            }
            
            public int? EnMultEQType
            {
                get
                {
                    return InfoList.MultEQTypeList.IndexOf(_AudysseyMultEQAvr.AvrInfo.EQType);
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.EQType = InfoList.MultEQTypeList.ElementAt((int)value);
                    RaisePropertyChanged("EQType");
                }
            }
            
            public string AmpAssignInfo
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrStatus.AssignBin;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrStatus.AssignBin = value;
                    RaisePropertyChanged("AssignBin");
                }
            }
            
            public bool? Auro
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.Auro;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.Auro = value;
                    RaisePropertyChanged("Auro");
                }
            }
            
            public string UpgradeInfo
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.Upgrade;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.Upgrade = value;
                    RaisePropertyChanged("UpgradeInfo");
                }
            }

            public Collection<DetectedChannel> DetectedChannels { get; set; }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };
            #endregion

            #region Methods
            protected void RaisePropertyChanged(string propertyName)
            {
                this?.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}
