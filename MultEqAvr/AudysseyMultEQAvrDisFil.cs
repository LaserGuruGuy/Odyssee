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
        public partial class AudysseyMultEQAvr :IDisFil, INotifyPropertyChanged
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
                            if (ch.Channel != null && ch.Skip != null && ch.AudyCurveFilter != null && ch.FlatCurveFilter != null)
                            {
                                if (ch.Skip == false)
                                {
                                    if (ch.Channel.Equals(ChData))
                                    {
                                        if (EqType.Equals("Audy"))
                                        {
                                            if (ch.AudyCurveFilter.Count > 0)
                                            {
                                                FilData = ch.AudyCurveFilter["dispLargeData"];
                                            }
                                        }
                                        else if(EqType.Equals("Flat"))
                                        {
                                            if (ch.FlatCurveFilter.Count > 0)
                                            {
                                                FilData = ch.FlatCurveFilter["dispLargeData"];
                                            }
                                        }
                                        break;
                                    }
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
                            if (ch.Channel != null && ch.Skip != null)
                            {
                                if (ch.Skip == false)
                                {
                                    if (ch.Channel.Equals(ChData))
                                    {
                                        if (EqType.Equals("Audy"))
                                        {
                                            if (ch.AudyCurveFilter == null)
                                            {
                                                ch.AudyCurveFilter = new();
                                            }
                                            ch.AudyCurveFilter.Remove("dispLargeData");
                                            ch.AudyCurveFilter.Add("dispLargeData", value);
                                        }
                                        else if (EqType.Equals("Flat"))
                                        {
                                            if (ch.FlatCurveFilter == null)
                                            {
                                                ch.FlatCurveFilter = new();
                                            }
                                            ch.FlatCurveFilter.Remove("dispLargeData");
                                            ch.FlatCurveFilter.Add("dispLargeData", value);
                                        }
                                        break;
                                    }
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
                            if (ch.Channel != null && ch.Skip != null && ch.AudyCurveFilter != null && ch.FlatCurveFilter != null)
                            {
                                if (ch.Skip == false)
                                {
                                    if (ch.Channel.Equals(ChData))
                                    {
                                        if (EqType.Equals("Audy"))
                                        {
                                            if (ch.AudyCurveFilter.Count > 0)
                                            {
                                                DispData = ch.AudyCurveFilter["dispSmallData"];
                                            }
                                        }
                                        else if (EqType.Equals("Flat"))
                                        {
                                            if (ch.FlatCurveFilter.Count > 0)
                                            {
                                                DispData = ch.FlatCurveFilter["dispSmallData"];
                                            }
                                        }
                                        break;
                                    }
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
                            if (ch.Channel != null && ch.Skip != null)
                            {
                                if (ch.Skip == false)
                                {
                                    if (ch.Channel.Equals(ChData))
                                    {
                                        if (EqType.Equals("Audy"))
                                        {
                                            if (ch.AudyCurveFilter == null)
                                            {
                                                ch.AudyCurveFilter = new();
                                            }
                                            ch.AudyCurveFilter.Remove("dispSmallData");
                                            ch.AudyCurveFilter.Add("dispSmallData", value);
                                        }
                                        else if (EqType.Equals("Flat"))
                                        {
                                            if (ch.FlatCurveFilter == null)
                                            {
                                                ch.FlatCurveFilter = new();
                                            }
                                            ch.FlatCurveFilter.Remove("dispSmallData");
                                            ch.FlatCurveFilter.Add("dispSmallData", value);
                                        }
                                        break;
                                    }
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