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
            float[] FilData { get; set; }
            [JsonProperty(Order = 4)]
            [JsonConverter(typeof(DecimalJsonConverter))]
            float[] DispData { get; set; }
        }
        public partial class AudysseyMultEQAvr :IDisFil, INotifyPropertyChanged
        {
            #region Properties
            [JsonIgnore]
            public string EqType { get; set; }
            [JsonIgnore]
            public string ChData { get; set; }
            [JsonIgnore]
            public float[] FilData
            {
                get
                {
                    float[] FilData = null;
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
                                            FilData = ch.AudyCurveFilter["dispLargeData"];
                                        }
                                        else if(EqType.Equals("Flat"))
                                        {
                                            FilData = ch.FlatCurveFilter["dispLargeData"];
                                        }
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
                                            ch.AudyCurveFilter.Remove("dispLargeData");
                                            ch.AudyCurveFilter.Add("dispLargeData", value);
                                            break;
                                        }
                                        else if (EqType.Equals("Flat"))
                                        {
                                            ch.FlatCurveFilter.Remove("dispLargeData");
                                            ch.FlatCurveFilter.Add("dispLargeData", value);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            [JsonIgnore]
            public float[] DispData
            {
                get
                {
                    float[] DispData = null;
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
                                            DispData = ch.AudyCurveFilter["dispSmallData"];
                                        }
                                        else if (EqType.Equals("Flat"))
                                        {
                                            DispData = ch.FlatCurveFilter["dispSmallData"];
                                        }
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
                                            ch.AudyCurveFilter.Remove("dispSmallData");
                                            ch.AudyCurveFilter.Add("dispSmallData", value);
                                            break;
                                        }
                                        else if (EqType.Equals("Flat"))
                                        {
                                            ch.FlatCurveFilter.Remove("dispSmallData");
                                            ch.FlatCurveFilter.Add("dispSmallData", value);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Methods
            public float[] ByteToFloatArray(sbyte[] Bytes)
            {
                if (Bytes.Length % 4 != 0) throw new ArgumentException();
                float[] Floats = new float[Bytes.Length / 4];
                Buffer.BlockCopy(Bytes, 0, Floats, 0, Bytes.Length);
                return Floats;
            }
            public sbyte[] FloatToByteArray(float[] Floats)
            {
                sbyte[] Bytes = new sbyte[Floats.Length * 4];
                Buffer.BlockCopy(Floats, 0, Bytes, 0, Bytes.Length);
                return Bytes;
            }
            public bool ShouldSerializeEqType() { return false; }
            public bool ShouldSerializeChData() { return false; }
            public bool ShouldSerializeFilData() { return false; }
            public bool ShouldSerializeDispData() { return false; }
            #endregion
        }
    }
}