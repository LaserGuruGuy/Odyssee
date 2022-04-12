using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Audyssey.MultEQ.List;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public interface IChannel
        {
            // START_CHNL {"Channel":"FL"}
            public string Channel { get; set; }
            // START_CHNL {"SpConnect":"S","Polarity":"N","Distance":237,"ResponseCoef":1}
            public string SpConnect { get; set; }
            public string Polarity { get; set; }
            public int? Distance { get; set; }
            public decimal? ResponseCoef { get; set; } // TODO: int but not for SW?
        }

        public class DetectedChannel : ChannelList, IChannel, INotifyPropertyChanged
        {
            #region BackingField
            // SET_POSNUM
            private string _Channel;
            private string _Setup;
            private bool? _Skip;

            // START_CHNL
            private string _SpConnect;
            private string _Polarity;
            private int? _Distance;
            private decimal? _ResponseCoef;
          
            //  
            private Dictionary<string, Int32[]> _ResponseData;
            #endregion

            #region Properties
            public string Channel
            {
                get
                {
                    return _Channel;
                }
                set
                {
                    _Channel = value;
                    RaisePropertyChanged("Channel");
                }
            }
            public string Setup
            {
                get
                {
                    return _Setup;
                }
                set
                {
                    _Setup = value;
                    RaisePropertyChanged("Setup");
                }
            }
            public bool? Skip
            {
                get
                {
                    return _Skip;
                }
                set
                {
                    _Skip = value;
                    RaisePropertyChanged("Sticky");
                }
            }

            /*"START_CHNL"*/
            public string SpConnect
            {
                get
                {
                    return _SpConnect;
                }
                set
                {
                    _SpConnect = value;
                    RaisePropertyChanged("SpConnect");
                }
            }
            public string Polarity
            {
                get
                {
                    return _Polarity;
                }
                set
                {
                    _Polarity = value;
                    RaisePropertyChanged("Polarity");
                }
            }
            public int? Distance
            {
                get
                {
                    return _Distance;
                }
                set
                {
                    _Distance = value;
                    RaisePropertyChanged("Distance");
                }
            }
            public decimal? ResponseCoef
            {
                get
                {
                    return _ResponseCoef;
                }
                set
                {
                    _ResponseCoef = value;
                    RaisePropertyChanged("ResponseCoef");
                }
            }

            public Dictionary<string, Int32[]> ResponseData
            {
                get
                {
                    return _ResponseData;
                }
                set
                {
                    _ResponseData = value;
                    RaisePropertyChanged("ResponseData");
                }
            }
            #endregion

            #region Methods
            private void ResetChannel() { _Channel = null; }
            private void ResetSetup() { _Setup = null; }
            private void ResetSkip() { _Skip = null; }

            private void ResetSpConnect() { _SpConnect = null; }
            private void ResetPolarity() { _Polarity = null; }
            private void ResetDistance() { _Distance = null; }
            private void ResetResponseCoef() { _ResponseCoef = null; }

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
            protected void RaisePropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var property in this.GetType().GetProperties())
                {
                    sb.Append(property + "=" + property.GetValue(this, null) + "\r\n");
                }
                return sb.ToString();
            }
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
        }
    }
}