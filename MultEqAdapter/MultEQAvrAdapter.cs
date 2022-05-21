using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MultEQAvrAdapter.AdapterList;
using Audyssey.MultEQAvr;
using Audyssey.MultEQ.List;
using System;
using System.Globalization;

namespace MultEQAvrAdapter
{
    namespace MultEQApp
    {
        public class AudysseyMultEQAvrAdapter
        {
            private ObservableCollection<DetectedChannel> _DetectedChannels;

            #region Poperties
            private AudysseyMultEQAvr _AudysseyMultEQAvr;

            public AudysseyMultEQAvrAdapter(ref AudysseyMultEQAvr AudysseyMultEQAvr)
            {
                _AudysseyMultEQAvr = AudysseyMultEQAvr;
                AudysseyMultEQAvr.PropertyChanged += PropertyChanged;
            }

            ~AudysseyMultEQAvrAdapter()
            {
                _AudysseyMultEQAvr.PropertyChanged -= PropertyChanged;
            }

            public string Title { get; set; }

            public string TargetModelName { get; set; }

            public string InterfaceVersion
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.Ifver;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.Ifver = value;
                    RaisePropertyChanged("Ifver");
                }
            }
            
            public bool? DynamicEq
            {
                get
                {
                    return _AudysseyMultEQAvr.AudyDynEq;
                }
                set
                {
                    _AudysseyMultEQAvr.AudyDynEq = value;
                    RaisePropertyChanged("AudyDynEq");
                }
            }
            
            public bool? DynamicVolume
            {
                get
                {
                    return _AudysseyMultEQAvr.AudyDynVol;
                }
                set
                {
                    _AudysseyMultEQAvr.AudyDynVol = value;
                    RaisePropertyChanged("AudyDynVol");
                }
            }
            
            public bool? LfcSupport
            {
                get
                {
                    return _AudysseyMultEQAvr.AudyLfc;
                }
                set
                {
                    _AudysseyMultEQAvr.AudyLfc = value;
                    RaisePropertyChanged("AudyLfc");
                }
            }
            
            public bool? Lfc
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.LFC;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.LFC = value;
                    RaisePropertyChanged("LFC");
                }
            }
            
            public int? SystemDelay
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.SysDelay;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.SysDelay = value;
                    RaisePropertyChanged("SysDelay");
                }
            }
            
            public decimal? AdcLineup
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.ADC;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.ADC = value;
                    RaisePropertyChanged("ADC");
                }
            }
            
            public int? EnTargetCurveType
            {
                get
                {

                    return MultEQList.AudyEqSetList.IndexOf(_AudysseyMultEQAvr.AudyEqSet) + 1;
                }
                set
                {
                    _AudysseyMultEQAvr.AudyEqSet = MultEQList.AudyEqSetList.ElementAt((int) value - 1);
                    RaisePropertyChanged("AudyEqSetList");
                }
            }

            public int? EnAmpAssignType
            {
                get
                {
                    return AdapterList.StatusList.AmpAssignTypeList.IndexOf(_AudysseyMultEQAvr.AvrStatus.AmpAssign);
                }
                set
                {
                    _AudysseyMultEQAvr.AvrStatus.AmpAssign = AdapterList.StatusList.AmpAssignTypeList.ElementAt((int)value);
                    RaisePropertyChanged("AmpAssign");
                }
            }
            
            public int? EnMultEQType
            {
                get
                {
                    return InfoList.MultEQTypeList.IndexOf(_AudysseyMultEQAvr.AvrInfo.EQType);
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.EQType = InfoList.MultEQTypeList.ElementAt((int)value);
                    RaisePropertyChanged("EQType");
                }
            }
            
            public string AmpAssignInfo
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrStatus.AssignBin;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrStatus.AssignBin = value;
                    RaisePropertyChanged("AssignBin");
                }
            }
            
            public bool? Auro
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.Auro;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.Auro = value;
                    RaisePropertyChanged("Auro");
                }
            }
            
            public string UpgradeInfo
            {
                get
                {
                    return _AudysseyMultEQAvr.AvrInfo.Upgrade;
                }
                set
                {
                    _AudysseyMultEQAvr.AvrInfo.Upgrade = value;
                    RaisePropertyChanged("UpgradeInfo");
                }
            }

            public ObservableCollection<DetectedChannel> DetectedChannels
            {
                get
                {
                    //  TODO convertback
                    return _DetectedChannels;
                }
                set
                {
                    // stash properties only relevant for App
                    _DetectedChannels = value;
                    // import compatible properties
                    UniqueObservableCollection<Audyssey.MultEQAvr.DetectedChannel> channels = _AudysseyMultEQAvr.DetectedChannels;
                    _AudysseyMultEQAvr.DetectedChannels = new();
                    Audyssey.MultEQAvr.DetectedChannel channel = null;
                    foreach (var ch in value)
                    {
                        for (var i = 0; i <= channels.Count; i++)
                        {
                            if (i == channels.Count)
                            {
                                channel = new Audyssey.MultEQAvr.DetectedChannel();
                                channel.Channel = ch.CommandId;
                                channel.Skip = ch.IsSkipMeasurement;
                                channel.Crossover = ch.CustomCrossover == null ? "F" : 10L * Convert.ToInt64(ch.CustomCrossover, CultureInfo.InvariantCulture);
                                channel.ChLevel = Convert.ToDecimal(ch.CustomLevel, CultureInfo.InvariantCulture );
                                channel.ChannelReport.SpConnect = AdapterList.ChannelList.SpeakerTypeList[(Int32)ch.ChannelReport.EnSpeakerConnect];
                                channel.ChannelReport.Distance = Convert.ToInt32(100m * ch.ChannelReport.Distance);
                                channel.ChannelReport.Polarity = ChannelReportList.PolarityList[Convert.ToInt32((bool)ch.ChannelReport.IsReversePolarity)];
                                channel.Setup = ch.CustomSpeakerType;
                                if (ch.ReferenceCurveFilter != null)
                                {
                                    foreach (var Filter in ch.ReferenceCurveFilter)
                                    {
                                        if (Filter.Key.Equals(MultEQList.DispDataList[0]))
                                        {
                                            channel.AudyCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDoubleNormalised)));
                                        }
                                        else
                                        {
                                            channel.AudyCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDouble)));
                                        }
                                    }
                                }
                                if (ch.FlatCurveFilter != null)
                                {
                                    foreach (var Filter in ch.FlatCurveFilter)
                                    {
                                        if (Filter.Key.Equals(MultEQList.DispDataList[0]))
                                        {
                                            channel.FlatCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDoubleNormalised)));
                                        }
                                        else
                                        {
                                            channel.FlatCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDouble)));
                                        }
                                    }
                                }
                                if (ch.ResponseData != null)
                                {
                                    foreach (var Filter in ch.ResponseData)
                                    {
                                        channel.ResponseData.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDouble)));
                                    }
                                }
                                channels.Add(channel);
                                break;
                            }
                            else if (channels[i].Channel.Equals(ch.CommandId))
                            {
                                channel = channels[i];
                                channel.Skip = ch.IsSkipMeasurement;
                                channel.Crossover = ch.CustomCrossover;
                                channel.ChLevel = Convert.ToDecimal(ch.CustomLevel);
                                channel.ChannelReport.SpConnect = ChannelReportList.SetupList[(Int32)ch.ChannelReport.EnSpeakerConnect];
                                channel.ChannelReport.Distance = Convert.ToInt32(100m * ch.ChannelReport.Distance);
                                channel.ChannelReport.Polarity = ChannelReportList.PolarityList[Convert.ToInt32((bool)ch.ChannelReport.IsReversePolarity)];
                                channel.Setup = ch.CustomSpeakerType;
                                foreach (var Filter in ch.ReferenceCurveFilter)
                                {
                                    for (var k = 0; k <= channel.AudyCurveFilter.Count; k++)
                                    {
                                        if (k == channel.AudyCurveFilter.Count)
                                        {
                                            if(Filter.Key.Equals(MultEQList.DispDataList[0]))
                                            {
                                                channel.AudyCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDoubleNormalised)));
                                            }
                                            else
                                            {
                                                channel.AudyCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDouble)));
                                            }
                                            break;
                                        }
                                        else if (channel.AudyCurveFilter.ContainsKey(Filter.Key))
                                        {
                                            if (channel.AudyCurveFilter.Remove(Filter.Key))
                                            {
                                                if (Filter.Key.Equals(MultEQList.DispDataList[0]))
                                                {
                                                    channel.AudyCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDoubleNormalised)));
                                                }
                                                else
                                                {
                                                    channel.AudyCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDouble)));
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                foreach (var Filter in ch.FlatCurveFilter)
                                {
                                    for (var k = 0; k <= channel.FlatCurveFilter.Count; k++)
                                    {
                                        if (k == channel.FlatCurveFilter.Count)
                                        {
                                            if (Filter.Key.Equals(MultEQList.DispDataList[0]))
                                            {
                                                channel.FlatCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDoubleNormalised)));
                                            }
                                            else
                                            {
                                                channel.FlatCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDouble)));
                                            }
                                            break;
                                        }
                                        else if (channel.FlatCurveFilter.ContainsKey(Filter.Key))
                                        {
                                            if (channel.FlatCurveFilter.Remove(Filter.Key))
                                            {
                                                if (Filter.Key.Equals(MultEQList.DispDataList[0]))
                                                {
                                                    channel.FlatCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDoubleNormalised)));
                                                }
                                                else
                                                {
                                                    channel.FlatCurveFilter.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDouble)));
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                foreach (var Filter in ch.ResponseData)
                                {
                                    for (var k = 0; k <= channel.ResponseData.Count; k++)
                                    {
                                        if (k == channel.ResponseData.Count)
                                        {
                                            channel.ResponseData.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDouble)));
                                            break;
                                        }
                                        else if (channel.ResponseData.ContainsKey(Filter.Key))
                                        {
                                            if (channel.ResponseData.Remove(Filter.Key))
                                            {
                                                channel.ResponseData.Add(Filter.Key, Array.ConvertAll(Filter.Value, new Converter<string, double>(ParseStringToDouble)));
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    _AudysseyMultEQAvr.DetectedChannels = channels; 
                }
            }

            public static double ParseStringToDouble(string value)
            {
                if (Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var dbl))
                {
                    return dbl;
                }
                throw new ApplicationException("Error parsing value " + value);
            }

            public static double ParseStringToDoubleNormalised(string value)
            {
                if (Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var dbl))
                {
                    return Math.Round(dbl);
                }
                throw new ApplicationException("Error parsing value " + value);
            }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };
            #endregion

            #region Methods
            private void RaisePropertyChanged(string propertyName = null)
            {
                this?.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}
