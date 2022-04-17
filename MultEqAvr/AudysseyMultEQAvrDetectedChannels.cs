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
using Newtonsoft.Json;

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
            private Dictionary<string, float[]> _ResponseData;
            private decimal? _ChLevel = 0m;
            private object? _Crossover = "F";
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
            public Dictionary<string, float[]> ResponseData
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
            public decimal? ChLevel
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
            public object? Crossover
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
            private void ResetResponseData() { _ResponseData = null; }
            #endregion

            #region BackingField
            private KeyValuePair<string, float[]> _SelectedResponseData = new();
            private List<KeyValuePair<string, float[]>> _StickyResponseData = new();
            private KeyValuePair<string, string[]> _selectedReferenceCurveFilter = new();
            private List<KeyValuePair<string, string[]>> _stickyReferenceCurveFilter = new();
            private KeyValuePair<string, string[]> _selectedFlatCurveFilter = new();
            private List<KeyValuePair<string, string[]>> _stickyFlatCurveFilter = new();
            private static double[] _filterFrequencies =
            { 19.6862664, 22.09708691, 24.80314144, 27.84058494, 31.25, 35.07693901, 39.37253281, 44.19417382, 49.60628287, 55.68116988, 62.5, 70.15387802, 78.74506562, 88.38834765, 99.21256575, 111.3623398, 125, 140.307756, 157.4901312, 176.7766953, 198.4251315, 222.7246795, 250, 280.6155121, 314.9802625, 353.5533906, 396.850263, 445.4493591, 500, 561.2310242, 629.9605249, 707.1067812, 793.700526, 890.8987181, 1000, 1122.462048, 1259.92105, 1414.213562, 1587.401052, 1781.797436, 2000, 2244.924097, 2519.8421, 2828.427125, 3174.802104, 3563.594873, 4000, 4489.848193, 5039.6842, 5656.854249, 6349.604208, 7127.189745, 8000, 8979.696386, 10079.3684, 11313.7085, 12699.20842, 14254.37949, 16000, 17959.39277, 20158.7368};
            private static double[] _displayFrequencies =
             { 62.5, 125, 250, 500, 1000, 2000, 4000, 8000, 16000};
            #endregion

            #region Properties
            [JsonIgnore]
            public KeyValuePair<string, float[]> SelectedResponseData
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
            public List<KeyValuePair<string, float[]>> StickyResponseData
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
            public KeyValuePair<string, string[]> SelectedReferenceCurveFilter
            {
                get
                {
                    return _selectedReferenceCurveFilter;
                }
                set
                {
                    _selectedReferenceCurveFilter = value;
                }
            }
            [JsonIgnore]
            public List<KeyValuePair<string, string[]>> StickyReferenceCurveFilter
            {
                get
                {
                    return _stickyReferenceCurveFilter;
                }
                set
                {
                    _stickyReferenceCurveFilter = value;
                }
            }
            [JsonIgnore]
            public KeyValuePair<string, string[]> SelectedFlatCurveFilter
            {
                get
                {
                    return _selectedFlatCurveFilter;
                }
                set
                {
                    _selectedFlatCurveFilter = value;
                }
            }
            [JsonIgnore]
            public List<KeyValuePair<string, string[]>> StickyFlatCurveFilter
            {
                get
                {
                    return _stickyFlatCurveFilter;
                }
                set
                {
                    _stickyFlatCurveFilter = value;
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
            private void ResetStickyResponseData() { _StickyResponseData = new(); }
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