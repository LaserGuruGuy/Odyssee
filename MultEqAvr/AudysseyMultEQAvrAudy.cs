using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        /// <summary>
        /// Interface for SET_SETDAT partial class IAudy
        /// </summary>
        public interface IAudy
        {
            bool? AudyDynVol { get; set; }
            string AudyDynSet { get; set; }
            bool? AudyMultEq { get; set; }
            string AudyEqSet { get; set; }
            bool? AudyLfc { get; set; }
            int? AudyLfcLev { get; set; }
        }

        /// <summary>
        /// Partial class IAudy
        /// </summary>
        public partial class AudysseyMultEQAvr : IAudy, INotifyPropertyChanged
        {
            #region BackingField
            private bool? _AudyDynVol;
            private string _AudyDynSet;
            private bool? _AudyMultEq;
            private string _AudyEqSet;
            private bool? _AudyLfc;
            private int? _AudyLfcLev;
            #endregion

            #region Properties
            public bool? AudyDynVol
            {
                get
                {
                    return _AudyDynVol;
                }
                set
                {
                    _AudyDynVol = value;
                    RaisePropertyChanged("AudyDynVol");
                }
            }
            public string AudyDynSet
            {
                get
                {
                    return _AudyDynSet;
                }
                set
                {
                    _AudyDynSet = value;
                    RaisePropertyChanged("AudyDynSet");
                }
            }
            public bool? AudyMultEq
            {
                get
                {
                    return _AudyMultEq;
                }
                set
                {
                    _AudyMultEq = value;
                    RaisePropertyChanged("AudyMultEq");
                }
            }
            public string AudyEqSet
            {
                get
                {
                    return _AudyEqSet;
                }
                set
                {
                    _AudyEqSet = value;
                    RaisePropertyChanged("AudyEqSet");
                }
            }
            public bool? AudyLfc
            {
                get
                {
                    return _AudyLfc;
                }
                set
                {
                    _AudyLfc = value;
                    RaisePropertyChanged("AudyLfc");
                }
            }
            public int? AudyLfcLev
            {
                get
                {
                    return _AudyLfcLev;
                }
                set
                {
                    _AudyLfcLev = value;
                    RaisePropertyChanged("AudyLfcLev");
                }
            }
            #endregion

            #region ResetMethods
            public void ResetAudyDynVol()
            {
                _AudyDynVol = null;
            }
            public void ResetAudyDynSet()
            {
                _AudyDynSet = string.Empty;
            }
            public void ResetAudyMultEq()
            {
                _AudyMultEq = null;
            }
            public void ResetAudyEqSet()
            {
                _AudyEqSet = string.Empty;
            }
            public void ResetAudyLfc()
            {
                _AudyLfc = null;
            }
            public void ResetAudyLfcLev()
            {
                _AudyLfcLev = null;
            }
            #endregion
        }
    }
}