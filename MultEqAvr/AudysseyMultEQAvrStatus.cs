using System.Collections.Generic;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public interface IStatus
        {
            #region Properties
            public bool? HPPlug { get; set; }
            public bool? Mic { get; set; }
            public string AmpAssign { get; set; }
            public string AssignBin { get; set; }
            public UniqueObservableCollection<Dictionary<string, string>> ChSetup { get; set; }
            public bool? BTTXStatus { get; set; }
            public bool? SpPreset { get; set; }
            #endregion
        }

        public partial class AudysseyMultEQAvr : IStatus, INotifyPropertyChanged
        {
            #region BackingField
            private bool? _HPPlug = null;
            private bool? _Mic = null;
            private string _AmpAssign = null;
            private string _AssignBin = null;
            private UniqueObservableCollection<Dictionary<string, string>> _ChSetup = null;
            private bool? _BTTXStatus = null;
            private bool? _SpPreset = null;
            #endregion

            #region Properties
            public bool? HPPlug
            { 
                get
                {
                    return _HPPlug;
                }
                set
                {
                    _HPPlug = value;
                    RaisePropertyChanged("HPPlug");
                }
            }
            public bool? Mic
            {
                get
                {
                    return _Mic;
                }
                set
                {
                    _Mic = value;
                    RaisePropertyChanged("Mic");
                }
            }
            public string AmpAssign
            {
                get
                {
                    return _AmpAssign;
                }
                set
                {
                    _AmpAssign = value;
                    RaisePropertyChanged("AmpAssign");
                }
            }
            public string AssignBin
            {
                get
                {
                    return _AssignBin;
                }
                set
                {
                    _AssignBin = value;
                    RaisePropertyChanged("AssignBin");
                }
            }
            public UniqueObservableCollection<Dictionary<string, string>> ChSetup
            {
                get
                {
                    return _ChSetup;
                }
                set
                {
                    _ChSetup = value;
                    RaisePropertyChanged("ChSetup");
                }
            }
            public bool? BTTXStatus
            {
                get
                {
                    return _BTTXStatus;
                }
                set
                {
                    _BTTXStatus = value;
                    RaisePropertyChanged("BTTXStatus");
                }
            }
            public bool? SpPreset
            {
                get
                {
                    return _SpPreset;
                }
                set
                {
                    _SpPreset = value;
                    RaisePropertyChanged("SpPreset");
                }
            }
            #endregion

            #region ResetMethods
            public void ResetHPPlug()
            {
                _HPPlug = null;
                RaisePropertyChanged("HPPlug");
            }
            public void ResetMic()
            {
                _Mic = null;
                RaisePropertyChanged("Mic");
            }
            public void ResetAmpAssign()
            {
                _AmpAssign = null;
                RaisePropertyChanged("AmpAssign");
            }
            public void ResetAssignBin()
            {
                _AssignBin = null;
                RaisePropertyChanged("AssignBin");
             }
            public void ResetChSetup()
            {
                _ChSetup = null;
                RaisePropertyChanged("ChSetup");
                RaisePropertyChanged("SelectedChSetup");
            }
            public void ResetBTTXStatus()
            {
                _BTTXStatus = null;
                RaisePropertyChanged("BTTXStatus");
            }
            public void ResetSpPreset()
            {
                _SpPreset = null;
                RaisePropertyChanged("SpPreset");
            }
            #endregion
        }
    }
}