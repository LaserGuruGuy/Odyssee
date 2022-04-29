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
            private string _SpConnect;
            private string _Polarity;
            private int? _Distance;
            [JsonIgnore]
            private double? _ResponseCoef;
            #endregion

            #region Properties
            public string? SpConnect
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
            public string? Polarity
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
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
        }
    }
}