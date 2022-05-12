using Audyssey.MultEQ.List;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public partial class AudysseyMultEQAvr : MultEQList, INotifyPropertyChanged
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
                                if (CoefChannelList[ch.Channel] == CoefChannel)
                                {
                                    double[] Coef = null;
                                    if (CoefCurve == 0x00)
                                    {
                                        if (ch.AudyCurveFilter != null)
                                        {
                                            Coef = ch.AudyCurveFilter[SampleRateList[CoefSampleRate]];
                                        }
                                    }
                                    else if (CoefCurve == 0x01)
                                    {
                                        if (ch.FlatCurveFilter != null)
                                        {
                                            Coef = ch.FlatCurveFilter[SampleRateList[CoefSampleRate]];
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
                                if (CoefChannelList[ch.Channel] == CoefChannel)
                                {
                                    if (CoefCurve == CurveFilterList.IndexOf("referenceCurveFilter"))
                                    {
                                        try
                                        {
                                            if (ch.AudyCurveFilter == null)
                                            {
                                                ch.AudyCurveFilter = new();
                                            }
                                            else
                                            {
                                                ch.AudyCurveFilter.Remove(SampleRateList[CoefSampleRate]);
                                            }
                                            ch.AudyCurveFilter.Add(SampleRateList[CoefSampleRate], Coef);
                                            ch.SelectedAudyCurveFilter = new(SampleRateList[CoefSampleRate], Coef);
                                            ch.SelectedFlatCurveFilter = new();
                                            SelectedChannel = ch;
                                        }
                                        catch (Exception ex)
                                        {
                                            StatusBar(ex.Message);
                                        }
                                    }
                                    else if (CoefCurve == CurveFilterList.IndexOf("flatCurveFilter"))
                                    {
                                        try
                                        {
                                            if (ch.FlatCurveFilter == null)
                                            {
                                                ch.FlatCurveFilter = new();
                                            }
                                            else
                                            {
                                                ch.FlatCurveFilter.Remove(SampleRateList[CoefSampleRate]);
                                            }
                                            ch.FlatCurveFilter.Add(SampleRateList[CoefSampleRate], Coef);
                                            ch.SelectedAudyCurveFilter = new();
                                            ch.SelectedFlatCurveFilter = new(SampleRateList[CoefSampleRate], Coef);
                                            SelectedChannel = ch;
                                        }
                                        catch (Exception ex)
                                        {
                                            StatusBar(ex.Message);
                                        }
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