using Audyssey.MultEQ.List;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public interface IDisFil
        {
            [JsonProperty(Order = 1)]
            string EqType { get; set; }
            [JsonProperty(Order = 2)]
            string ChData { get; set; }
            [JsonProperty(Order = 3)]
            [JsonConverter(typeof(DecimalJsonConverter))]
            double[] FilData { get; set; }
            [JsonProperty(Order = 4)]
            [JsonConverter(typeof(DecimalJsonConverter))]
            double[] DispData { get; set; }
        }
        public partial class AudysseyMultEQAvr : MultEQList, IDisFil, INotifyPropertyChanged
        {
            #region Properties
            [JsonIgnore]
            public string EqType { get; set; }
            [JsonIgnore]
            public string ChData { get; set; }
            [JsonIgnore]
            public double[] FilData
            {
                get
                {
                    double[] FilData = null;
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Channel != null && ch.AudyCurveFilter != null && ch.FlatCurveFilter != null)
                            {
                                if (ch.Channel.Equals(ChData))
                                {
                                    if (EqType.Equals(AudyEqSetList[0]))
                                    {
                                        if (ch.AudyCurveFilter.Count > 0)
                                        {
                                            FilData = ch.AudyCurveFilter[DispDataList[0]];
                                        }
                                    }
                                    else if (EqType.Equals(AudyEqSetList[1]))
                                    {
                                        if (ch.FlatCurveFilter.Count > 0)
                                        {
                                            FilData = ch.FlatCurveFilter[DispDataList[0]];
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    return FilData;
                }
                set
                {
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Channel != null)
                            {
                                if (ch.Channel.Equals(ChData))
                                {
                                    if (EqType.Equals(AudyEqSetList[0]))
                                    {
                                        if (ch.AudyCurveFilter == null)
                                        {
                                            ch.AudyCurveFilter = new();
                                        }
                                        ch.AudyCurveFilter.Remove(DispDataList[0]);
                                        ch.AudyCurveFilter.Add(DispDataList[0], value);
                                    }
                                    else if (EqType.Equals(AudyEqSetList[1]))
                                    {
                                        if (ch.FlatCurveFilter == null)
                                        {
                                            ch.FlatCurveFilter = new();
                                        }
                                        ch.FlatCurveFilter.Remove(DispDataList[0]);
                                        ch.FlatCurveFilter.Add(DispDataList[0], value);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            [JsonIgnore]
            public double[] DispData
            {
                get
                {
                    double[] DispData = null;
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Channel != null && ch.AudyCurveFilter != null && ch.FlatCurveFilter != null)
                            {
                                if (ch.Channel.Equals(ChData))
                                {
                                    if (EqType.Equals(AudyEqSetList[0]))
                                    {
                                        if (ch.AudyCurveFilter.Count > 0)
                                        {
                                            DispData = ch.AudyCurveFilter[DispDataList[1]];
                                        }
                                    }
                                    else if (EqType.Equals(AudyEqSetList[1]))
                                    {
                                        if (ch.FlatCurveFilter.Count > 0)
                                        {
                                            DispData = ch.FlatCurveFilter[DispDataList[1]];
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    return DispData;
                }
                set
                {
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Channel != null)
                            {
                                if (ch.Channel.Equals(ChData))
                                {
                                    if (EqType.Equals(AudyEqSetList[0]))
                                    {
                                        if (ch.AudyCurveFilter == null)
                                        {
                                            ch.AudyCurveFilter = new();
                                        }
                                        ch.AudyCurveFilter.Remove(DispDataList[1]);
                                        ch.AudyCurveFilter.Add(DispDataList[1], value);
                                    }
                                    else if (EqType.Equals(AudyEqSetList[1]))
                                    {
                                        if (ch.FlatCurveFilter == null)
                                        {
                                            ch.FlatCurveFilter = new();
                                        }
                                        ch.FlatCurveFilter.Remove(DispDataList[1]);
                                        ch.FlatCurveFilter.Add(DispDataList[1], value);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }
    }
}