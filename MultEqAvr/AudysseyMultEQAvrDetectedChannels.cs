using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Audyssey.MultEQ;

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

        public interface IResponseData
        {
            // GET_RESPON {"ChData":"FL"}
            public string ChData { get; set; }
            // GET_RESPON 128 * 512 bytes or 128 float
            public Int32[] ResponseData { get; set; }
        }

        public class DetectedChannel : MultEQList, IChannel, IResponseData, INotifyPropertyChanged
        {
            #region BackingField
            private string _Channel;
            private string _Setup;
            private bool? _Skip;
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
            public string SpConnect { get; set; }
            public string Polarity { get; set; }
            public int? Distance { get; set; }
            public decimal? ResponseCoef { get; set; }
            public string ChData { get; set; }
            public Int32[] ResponseData { get; set; }
            public string[] ResponseDataString
            {
                get
                {
                    return ResponseData?.Select(x => ((float)x / (float)0x7fffffff).ToString(CultureInfo.InvariantCulture)).ToArray();
                }
            }
            #endregion

            #region Methods
            private void ResetChannel() { _Channel = null; }
            private void ResetSetup() { _Setup = null; }
            private void ResetSkip() { _Skip = false; }
            private void ResetSpConnect() { SpConnect = null; }
            private void ResetPolarity() { Polarity = null; }
            private void ResetDistance() { Distance = null; }
            private void ResetResponseCoef() { ResponseCoef = null; }
            public void ResetChata() { ChData = null; }
            public void ResetResponseData() { ResponseData = null; }
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