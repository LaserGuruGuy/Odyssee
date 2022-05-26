using System;
using System.ComponentModel;
using Audyssey.MultEQ.List;
using Newtonsoft.Json;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public interface IChannelReport
        {
            public string SpConnect { get; set; }
            public string Polarity { get; set; }
            public int? Distance { get; set; }
            public double? ResponseCoef { get; set; }
        }

        public class ChannelReport : ChannelReportList, INotifyPropertyChanged
        {
            #region BackingField
            private string _SpConnect = "N";
            private string _Polarity = "N";
            private int? _Distance = 0;
            [JsonIgnore]
            private double? _ResponseCoef;
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
                    RaisePropertyChanged("Delay");
                }
            }

            [JsonIgnore]
            public decimal? Delay
            {
                get
                {
                    try
                    {
                        return (decimal)Math.Round((double)_Distance * 100d / 343d) / 10m;
                    }
                    catch
                    {
                        return 0;
                    }
                }
                set
                {
                    Distance = (int)(value * 34.30m);
                }
            }

            public double? ResponseCoef
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
            private void ResetSpConnect() { _SpConnect = "N"; }
            private void ResetPolarity() { _Polarity = "N"; }
            private void ResetDistance() { _Distance = 0; }
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
            private void RaisePropertyChanged(string propertyName)
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
    }
}