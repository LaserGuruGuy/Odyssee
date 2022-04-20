using Audyssey.MultEQ.List;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public partial class AudysseyMultEQAvr : INotifyPropertyChanged
        {
            #region Properties
            [JsonIgnore]
            public byte CoefSpare { get; set; }
            [JsonIgnore]
            public byte CoefChannel { get; set; }
            [JsonIgnore]
            public byte CoefCurve { get; set; }
            [JsonIgnore]
            public byte CoefSampleRate { get; set; }
            [JsonIgnore]
            public byte[] CoefData
            {
                get
                {
                    byte[] _CoefData = null;
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Channel != null)
                            {
                                if ((ch.Channel.Equals("FL") && (CoefChannel == 0x00)) ||
                                    (ch.Channel.Equals("C") && (CoefChannel == 0x01)) ||
                                    (ch.Channel.Equals("FR") && (CoefChannel == 0x02)) ||
                                    (ch.Channel.Equals("SLA") && (CoefChannel == 0x03)) ||
                                    (ch.Channel.Equals("SRA") && (CoefChannel == 0x0C)) ||
                                    (ch.Channel.Equals("SW1") && (CoefChannel == 0x0D)))
                                {
                                    double[] Coef = null;
                                    if (CoefCurve == 0x00)
                                    {
                                        if (ch.AudyCurveFilter != null)
                                        {
                                            Coef = ch.AudyCurveFilter[CoefSampleRate.ToString()];
                                        }
                                    }
                                    else if (CoefCurve == 0x01)
                                    {
                                        if (ch.FlatCurveFilter != null)
                                        {
                                            Coef = ch.FlatCurveFilter[CoefSampleRate.ToString()];
                                        }
                                    }
                                    _CoefData = new byte[4 * (Coef.Length + 1)];
                                    _CoefData[0] = CoefCurve;
                                    _CoefData[1] = CoefSampleRate;
                                    _CoefData[2] = CoefChannel;
                                    _CoefData[3] = CoefSpare;
                                    for (int i = 0; i < Coef.Length; i++)
                                    {
                                        byte[] ByteCoef = BitConverter.GetBytes((Int32)Math.Round(Coef[i] * (double)Int32.MaxValue));
                                        for (int j = 0; j < 4; j++)
                                        {
                                            _CoefData[4 * (i + 1) + j] = ByteCoef[j];
                                        }
                                    }
                                    SelectedChannel = ch;
                                    break;
                                }
                            }
                        }
                    }
                    return _CoefData;
                }
                set
                {
                    if (value.Length % 4 != 0)
                    {
                        throw new ArgumentException();
                    }
                    if (DetectedChannels != null)
                    {
                        // Header
                        CoefCurve = value[0];
                        CoefSampleRate = value[1];
                        CoefChannel = value[2];
                        CoefSpare = value[3];
                        // Payload
                        double[] Coef = new double[(value.Length - 1) / 4];
                        for (int i = 0; i < Coef.Length - 1; i++)
                        {
                            Coef[i] = BitConverter.ToInt32(value, (i + 1) * 4) / (double)(Int32.MaxValue);
                        }
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Channel != null)
                            {
                                if ((ch.Channel.Equals("FL") && (CoefChannel == 0x00)) ||
                                    (ch.Channel.Equals("C") && (CoefChannel == 0x01)) ||
                                    (ch.Channel.Equals("FR") && (CoefChannel == 0x02)) ||
                                    (ch.Channel.Equals("SLA") && (CoefChannel == 0x03)) ||
                                    (ch.Channel.Equals("SRA") && (CoefChannel == 0x0C)) ||
                                    (ch.Channel.Equals("SW1") && (CoefChannel == 0x0D)))
                                {
                                    if (CoefCurve == 0x00)
                                    {
                                        if (ch.AudyCurveFilter == null)
                                        {
                                            ch.AudyCurveFilter = new();
                                        }
                                        ch.AudyCurveFilter.Add(CoefSampleRate.ToString(), Coef);
                                        RaisePropertyChanged("AudyCurveFilter");
                                        break;
                                    }
                                    else if (CoefCurve == 0x01)
                                    {
                                        if (ch.FlatCurveFilter == null)
                                        {
                                            ch.FlatCurveFilter = new();
                                        }
                                        ch.FlatCurveFilter.Add(CoefSampleRate.ToString(), Coef);
                                        RaisePropertyChanged("FlatCurveFilter");
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