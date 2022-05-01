using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public interface IFin
        {
            public string AudyFinFlg { get; set; }
        }

        /// <summary>
        /// Interface for SET_SETDAT partial class IAmp
        /// </summary>
        public interface IAmp
        {
            public UniqueObservableCollection<Dictionary<string, int>> ChLevel { get; set; }
            public UniqueObservableCollection<Dictionary<string, object>> Crossover { get; set; }
            public UniqueObservableCollection<Dictionary<string, string>> SpConfig { get; set; }
            public UniqueObservableCollection<Dictionary<string, int>> Distance { get; set; }
            public string AudyFinFlg { get; set; }
            public bool? AudyDynEq { get; set; }
            public int? AudyEqRef { get; set; }
        }

        /// <summary>
        /// Partial class IAmp
        /// </summary>
        public partial class AudysseyMultEQAvr : IFin, IAmp, INotifyPropertyChanged
        {
            #region BackingField
            private string _AudyFinFlg;// = "NotFin";
            private bool? _AudyDynEq;// = true;
            private int? _AudyEqRef;// = 0;
            #endregion

            #region Properties
            [JsonIgnore]
            public UniqueObservableCollection<Dictionary<string, int>> ChLevel
            {
                get
                {
                    UniqueObservableCollection<Dictionary<string, int>> ChLevel = new();
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Channel != null && ch.ChLevel != null && ch.Skip != null)
                            {
                                if (ch.Skip == false)
                                {
                                    ChLevel.Add(new Dictionary<string, int>() { { (string)ch.Channel, (int)((decimal)ch.ChLevel * 10m) } });
                                }
                            }
                        }
                    }
                    return ChLevel;
                }
                set
                {
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (string.IsNullOrEmpty(ch.Channel) == false)
                            {
                                foreach (var sp in value)
                                {
                                    foreach (var el in sp)
                                    {
                                        if (ch.Channel.Equals(el.Key))
                                        {
                                            try
                                            {
                                                ch.ChLevel = (decimal)el.Value / 10m;
                                            }
                                            catch (Exception ex)
                                            {
                                                Serialized += ex.Message;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            [JsonIgnore]
            public UniqueObservableCollection<Dictionary<string, object>> Crossover
            {
                get
                {
                    UniqueObservableCollection<Dictionary<string, object>> Crossover = new();
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Channel != null && ch.Crossover != null && ch.Skip != null)
                            {
                                if (ch.Skip == false)
                                {
                                    Crossover.Add(new Dictionary<string, object>() { { ch.Channel, ch.Crossover.GetType() == typeof(string) ? ch.Crossover : (int.Parse(ch.Crossover.ToString()) / 10) } });
                                }
                            }
                        }
                    }
                    return Crossover;
                }
                set
                {
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (string.IsNullOrEmpty(ch.Channel) == false)
                            {
                                foreach (var sp in value)
                                {
                                    foreach (var el in sp)
                                    {
                                        if (ch.Channel.Equals(el.Key))
                                        {
                                            try
                                            {
                                                ch.Crossover = el.Value;
                                            }
                                            catch (Exception ex)
                                            {
                                                Serialized += ex.Message;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            [JsonIgnore]
            public UniqueObservableCollection<Dictionary<string, string>> SpConfig
            {
                get
                {
                    UniqueObservableCollection<Dictionary<string, string>> SpConfig = new();
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.ChannelReport != null && ch.Skip != null)
                            {
                                if (ch.ChannelReport.SpConnect != null)
                                {
                                    if (ch.Skip == false)
                                    {
                                        if (ch.Skip == false)
                                        {
                                            SpConfig.Add(new Dictionary<string, string>() { { ch.Channel, ch.ChannelReport.SpConnect } });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return SpConfig;
                }
                set
                {
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (string.IsNullOrEmpty(ch.Channel) == false && ch.ChannelReport != null)
                            {
                                foreach (var sp in value)
                                {
                                    foreach (var el in sp)
                                    {
                                        if (ch.Channel.Equals(el.Key))
                                        {
                                            try
                                            {
                                                ch.ChannelReport.SpConnect = el.Value;
                                            }
                                            catch (Exception ex)
                                            {
                                                Serialized += ex.Message;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            [JsonIgnore]
            public UniqueObservableCollection<Dictionary<string, int>> Distance
            {
                get
                {
                    UniqueObservableCollection<Dictionary<string, int>> Distance = new();
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.ChannelReport != null && ch.Skip != null)
                            {
                                if (ch.ChannelReport.Distance != null)
                                {
                                    if (ch.Skip == false)
                                    {
                                        Distance.Add(new Dictionary<string, int>() { { ch.Channel, (int)ch.ChannelReport.Distance } });
                                    }
                                }
                            }
                        }
                    }
                    return Distance;
                }
                set
                {
                    if (DetectedChannels != null)
                    {
                        foreach (var ch in DetectedChannels)
                        {
                            if (ch.Channel != null && ch.ChannelReport != null)
                            {
                                foreach (var sp in value)
                                {
                                    foreach (var el in sp)
                                    {
                                        if (ch.Channel.Equals(el.Key))
                                        {
                                            try
                                            {
                                                ch.ChannelReport.Distance = el.Value;
                                            }
                                            catch (Exception ex)
                                            {
                                                Serialized += ex.Message;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            [JsonIgnore]
            public string AudyFinFlg
            {
                get
                {
                    return _AudyFinFlg;
                }
                set
                {
                    _AudyFinFlg = value;
                    RaisePropertyChanged("AudyFinFlg");
                }
            }
            public bool? AudyDynEq
            {
                get
                {
                    return _AudyDynEq;
                }
                set
                {
                    _AudyDynEq = value;
                    RaisePropertyChanged("AudyDynEq");
                }
            }
            public int? AudyEqRef
            {
                get
                {
                    return _AudyEqRef;
                }
                set
                {
                    _AudyEqRef = value;
                    RaisePropertyChanged("AudyEqRef");
                }
            }
            #endregion

            #region ResetMethods
            public void ResetAudyFinFlg()
            {
                _AudyFinFlg = null;
            }
            public void ResetAudyDynEq()
            {
                _AudyDynEq = null;
            }
            public void ResetAudyEqRef()
            {
                _AudyEqRef = null;
            }
            #endregion
        }
    }
}