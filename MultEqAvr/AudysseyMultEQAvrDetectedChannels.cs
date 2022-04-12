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
        public interface IChannelReport
        {
            // START_CHNL {"SpConnect":"S","Polarity":"N","Distance":237,"ResponseCoef":1}
            public string SpConnect { get; set; }
            public string Polarity { get; set; }
            public int Distance { get; set; }
            public decimal ResponseCoef { get; set; } // TODO: int but not for SW?
        }

        public class ChannelReport : IChannelReport, INotifyPropertyChanged
        {
            #region BackingField
            private string _SpConnect = string.Empty;
            private string _Polarity = string.Empty;
            private int _Distance = 0;
            private decimal _ResponseCoef = 0;
            #endregion

            #region Properties
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
            public int Distance
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
            public decimal ResponseCoef
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
            #endregion

            #region Methods
            private void ResetSpConnect() { _SpConnect = string.Empty; }
            private void ResetPolarity() { _Polarity = string.Empty; }
            private void ResetDistance() { _Distance = 0; }
            private void ResetResponseCoef() { _ResponseCoef = 0; }
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
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
        }

        public class DetectedChannel : ChannelList, INotifyPropertyChanged
        {
            #region BackingField
            // SET_POSNUM
            private string _Channel;
            private string _Setup;
            private bool? _Skip;
            private ChannelReport _ChannelReport = new();
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
            public ChannelReport ChannelReport
            {
                get
                {
                    return _ChannelReport;
                }
                set
                {
                    _ChannelReport = ChannelReport;
                    RaisePropertyChanged("ChannelReport");
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