using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Audyssey.MultEQ.List;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public class DetectedChannel : ChannelList, INotifyPropertyChanged
        {
            #region BackingField
            private string _Channel;
            private string _Setup;
            private bool? _Skip;
            private bool? _Stick = false;
            private ChannelReport _ChannelReport = new();
            private Dictionary<string, double[]> _ResponseData = new();
            private Dictionary<string, double[]> _AudyCurveFilter = new(); //{ { "dispSmallData", new float[9] }, { "dispLargeData", new float[61] }, { "coefficient48kHz", new float[704] }, { "coefficient441kHz", new float[704] }, { "coefficient32kHz", new float[704] } };
            private Dictionary<string, double[]> _FlatCurveFilter = new(); //{ { "dispSmallData", new float[9] }, { "dispLargeData", new float[61] }, { "coefficient48kHz", new float[704] }, { "coefficient441kHz", new float[704] }, { "coefficient32kHz", new float[704] } };
            private decimal _ChLevel;// = 0m;
            private object _Crossover;// = "F";
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
                    RaisePropertyChanged("Skip");
                }
            }
            public bool? Stick
            {
                get
                {
                    return _Stick;
                }
                set
                {
                    _Stick = value;
                    RaisePropertyChanged("Stick");
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
            public Dictionary<string, double[]> ResponseData
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
            public Dictionary<string, double[]> AudyCurveFilter
            {
                get
                {
                    return _AudyCurveFilter;
                }
                set
                {
                    _AudyCurveFilter = value;
                    RaisePropertyChanged("AudyCurveFilter");
                }
            }
            public Dictionary<string, double[]> FlatCurveFilter
            {
                get
                {
                    return _FlatCurveFilter;
                }
                set
                {
                    _FlatCurveFilter = value;
                    RaisePropertyChanged("FlatCurveFilter");
                }
            }
            public decimal ChLevel
            {
                get
                {
                    return _ChLevel;
                }
                set
                {
                    _ChLevel = value;
                    RaisePropertyChanged("ChLevel");
                }
            }
            public object Crossover
            {
                get
                {
                    return _Crossover;
                }
                set
                {
                    _Crossover = value;
                    RaisePropertyChanged("Crossover");
                }
            }
            #endregion

            #region Methods
            private void ResetChannel() { _Channel = null; }
            private void ResetSetup() { _Setup = null; }
            private void ResetSkip() { _Skip = null; }
            private void ResetStick() { _Stick = false; }
            private void ResetChannelReport() { _ChannelReport = new(); }
            private void ResetResponseData() { _ResponseData = new(); }
            private void ResetAudyCurveFilter() { _AudyCurveFilter = new(); }
            private void ResetFlatCurveFilter() { _FlatCurveFilter = new(); }
            #endregion

            #region BackingField
            private KeyValuePair<string, double[]> _SelectedResponseData = new();
            private List<KeyValuePair<string, double[]>> _StickyResponseData = new();
            private KeyValuePair<string, double[]> _SelectedAudyCurveFilter = new();
            private List<KeyValuePair<string, double[]>> _StickyAudyCurveFilter = new();
            private KeyValuePair<string, double[]> _SelectedFlatCurveFilter = new();
            private List<KeyValuePair<string, double[]>> _StickyFlatCurveFilter = new();
            private static double[] _filterFrequencies = { 19.6862664, 22.09708691, 24.80314144, 27.84058494, 31.25, 35.07693901, 39.37253281, 44.19417382, 49.60628287, 55.68116988, 62.5, 70.15387802, 78.74506562, 88.38834765, 99.21256575, 111.3623398, 125, 140.307756, 157.4901312, 176.7766953, 198.4251315, 222.7246795, 250, 280.6155121, 314.9802625, 353.5533906, 396.850263, 445.4493591, 500, 561.2310242, 629.9605249, 707.1067812, 793.700526, 890.8987181, 1000, 1122.462048, 1259.92105, 1414.213562, 1587.401052, 1781.797436, 2000, 2244.924097, 2519.8421, 2828.427125, 3174.802104, 3563.594873, 4000, 4489.848193, 5039.6842, 5656.854249, 6349.604208, 7127.189745, 8000, 8979.696386, 10079.3684, 11313.7085, 12699.20842, 14254.37949, 16000, 17959.39277, 20158.7368};
            private static double[] _displayFrequencies = { 62.5, 125, 250, 500, 1000, 2000, 4000, 8000, 16000};
            #endregion

            #region Properties
            [JsonIgnore]
            public KeyValuePair<string, double[]> SelectedResponseData
            {
                get
                {
                    return _SelectedResponseData;
                }
                set
                {
                    _SelectedResponseData = value;
                }
            }
            [JsonIgnore]
            public List<KeyValuePair<string, double[]>> StickyResponseData
            {
                get
                {
                    return _StickyResponseData;
                }
                set
                {
                    _StickyResponseData = value;
                }
            }
            [JsonIgnore]
            public KeyValuePair<string, double[]> SelectedAudyCurveFilter
            {
                get
                {
                    return _SelectedAudyCurveFilter;
                }
                set
                {
                    _SelectedAudyCurveFilter = value;
                }
            }
            [JsonIgnore]
            public List<KeyValuePair<string, double[]>> StickyAudyCurveFilter
            {
                get
                {
                    return _StickyAudyCurveFilter;
                }
                set
                {
                    _StickyAudyCurveFilter = value;
                }
            }
            [JsonIgnore]
            public KeyValuePair<string, double[]> SelectedFlatCurveFilter
            {
                get
                {
                    return _SelectedFlatCurveFilter;
                }
                set
                {
                    _SelectedFlatCurveFilter = value;
                }
            }
            [JsonIgnore]
            public List<KeyValuePair<string, double[]>> StickyFlatCurveFilter
            {
                get
                {
                    return _StickyFlatCurveFilter;
                }
                set
                {
                    _StickyFlatCurveFilter = value;
                }
            }
            [JsonIgnore]
            public double[] FilterFrequencies
            {
                get
                {
                    return _filterFrequencies;
                }
            }
            [JsonIgnore]
            public double[] DisplayFrequencies
            {
                get
                {
                    return _displayFrequencies;
                }
            }
            #endregion

            #region Methods
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
            public void ResetSelectedResponseData() { _SelectedResponseData = new();  }
            private void ResetStickyResponseData() { _StickyResponseData = new(); }
            private void ResetSelectedAudyCurveFilter() { _SelectedAudyCurveFilter = new(); }
            private void ResetStickyAudyCurveFilter() { _StickyAudyCurveFilter = new(); }
            private void ResetSelectedFlatCurveFilter() { _SelectedFlatCurveFilter = new(); }
            private void ResetStickyFlatCurveFilter() { _StickyFlatCurveFilter = new(); }
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