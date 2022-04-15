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
            private bool? _AudyDynVol = false;
            private string _AudyDynSet = "H";
            private bool? _AudyMultEq = true;
            private string _AudyEqSet = "Flat";
            private bool? _AudyLfc = false;
            private int? _AudyLfcLev = 7;
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
                _AudyDynVol = false;
            }
            public void ResetAudyDynSet()
            {
                _AudyDynSet = "H";
            }
            public void ResetAudyMultEq()
            {
                _AudyMultEq = true;
            }
            public void ResetAudyEqSet()
            {
                _AudyEqSet = "Flat";
            }
            public void ResetAudyLfc()
            {
                _AudyLfc = false;
            }
            public void ResetAudyLfcLev()
            {
                _AudyLfcLev = 7;
            }
            #endregion
        }
    }
}