using Newtonsoft.Json;
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
            private bool? _HPPlug;
            private bool? _Mic;
            private string _AmpAssign;
            private string _AssignBin;
            private UniqueObservableCollection<Dictionary<string, string>> _ChSetup;
            private bool? _BTTXStatus;
            private bool? _SpPreset;
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

            #region Methods
            private void ResetHPPlug() { _HPPlug = null; }
            private void ResetMic() { _Mic = null; }
            private void ResetAmpAssign() { _AmpAssign = null; }
            private void ResetAssignBin() { _AssignBin = null; }
            private void ResetChSetup() { _ChSetup = null; }
            private void ResetBTTXStatus() { _BTTXStatus = null; }
            private void ResetSpPreset() { _SpPreset = null; }
            #endregion
        }
    }
}