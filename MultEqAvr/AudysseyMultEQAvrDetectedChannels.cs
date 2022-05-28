using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Audyssey.MultEQ.List;
using System;
using Remez.Filtering;

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
            private Dictionary<string, double[]> _AudyCurveFilter = new();
            private Dictionary<string, double[]> _FlatCurveFilter = new();
            private decimal _ChLevel;
            private object _Crossover;
            #endregion

            public double[] GetOctave(int Band)
            {
                List<double> Result = new List<double>();
                double fr = 1000;

                // Modulus takes a long time to calcualte so just do it once
                bool IsEven = (Band % 2 == 0);

                for (int i = -(10 * (Band)); i < 5 * (Band); i++)
                {
                    double OctavePower = 0;
                    if (!IsEven)
                    {
                        OctavePower = (3 * (double)i / (10 * (double)Band));
                    }
                    else
                    {
                        OctavePower = (3 * (2 * (double)i + 1) / (20 * (double)Band));
                    }
                    double fm = fr * System.Math.Pow(10, OctavePower);

                    double fH = fm * System.Math.Pow(10, 3 / (20 + (double)Band));
                    double fL = fm * System.Math.Pow(10, -3 / (20 * (double)Band));

                    if ((fL >= 20 && fL <= 20000))
                    {
                        Result.Add(fm);
                    }
                }
                return Result.ToArray();
            }

            private static System.Threading.Mutex mutex = new();

            #region Properties
            [JsonIgnore]
            public string Name
            {
                get
                {
                    try
                    {
                        return ChannelNameList[_Channel];
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
            }
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
            [JsonIgnore]
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
                    _ChannelReport = value;
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
            private List<KeyValuePair<string, double[]>> _AverageResponseData = new();
            private KeyValuePair<string, double[]> _SelectedAudyCurveFilter = new();
            private KeyValuePair<string, double[]> _SelectedFlatCurveFilter = new();
            private double? _FirFilterGain;
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
                    RaisePropertyChanged("SelectedResponseData");
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
                    RaisePropertyChanged("StickyResponseData");
                }
            }
            [JsonIgnore]
            public List<KeyValuePair<string, double[]>> AverageResponseData
            {
                get
                {
                    return _AverageResponseData;
                }
                set
                {
                    _AverageResponseData = value;
                    RaisePropertyChanged("AverageResponseData");
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
                    RaisePropertyChanged("SelectedAudyCurveFilter");
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
                    RaisePropertyChanged("SelectedFlatCurveFilter");
                }
            }
            [JsonIgnore]
            public double? FirFilterGain
            {
                get
                {
                    return _FirFilterGain;
                }
                set
                {
                    _FirFilterGain = value;
                    RaisePropertyChanged("FirFilterGain");
                }
            }
            #endregion

            #region Methods
            private System.Windows.Input.ICommand _ClearAudyCurveFilter;
            private System.Windows.Input.ICommand _ClearFlatCurveFilter;
            [JsonIgnore]
            public System.Windows.Input.ICommand ClearAudyCurveFilter => _ClearAudyCurveFilter ?? (_ClearAudyCurveFilter = new CommunityToolkit.Mvvm.Input.RelayCommand<object>(ClearAudyCurveFilterCommand));
            [JsonIgnore]
            public System.Windows.Input.ICommand ClearFlatCurveFilter => _ClearFlatCurveFilter ?? (_ClearFlatCurveFilter = new CommunityToolkit.Mvvm.Input.RelayCommand<object>(ClearFlatCurveFilterCommand));
            public void ClearAudyCurveFilterCommand(object Object)
            {
                KeyValuePair<string, double[]> CurveFilter = (KeyValuePair<string, double[]>)Object;
                if (CurveFilter.Key != null && CurveFilter.Value != null)
                {
                    Array.Clear(CurveFilter.Value, 0, CurveFilter.Value.Length);
                    if (CurveFilter.Key.Contains("coefficient"))
                    {
                        CurveFilter.Value[0] = 1.0d /3.0d;
                    }
                    RaisePropertyChanged("SelectedAudyCurveFilter");
                }
            }
            public void ClearFlatCurveFilterCommand(object Object)
            {
                KeyValuePair<string, double[]> CurveFilter = (KeyValuePair<string, double[]>)Object;
                if (CurveFilter.Key != null && CurveFilter.Value != null)
                {
                    Array.Clear(CurveFilter.Value, 0, CurveFilter.Value.Length);
                    if (CurveFilter.Key.Contains("coefficient"))
                    {
                        CurveFilter.Value[0] = 1.0d / 3.0d;
                    }
                    RaisePropertyChanged("SelectedFlatCurveFilter");
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
            private void ResetChLevel() { _ChLevel = 0m; }
            private void ResetCrossover() { _Crossover = "F"; }
            private void ResetSelectedFlatCurveFilter() { _SelectedFlatCurveFilter = new(); }
            private void RaisePropertyChanged(string propertyName)
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