using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace Audyssey
{
    namespace MultEQ.List
    {
        public class StatusList
        {
            public static readonly StringCollection ChSetupList = new()
            { 
                "FL",
                "C",
                "FR",
                "SRA",
                "SBR",
                "SBL",
                "SLA",
                "FHR",
                "TFR",
                "TMR",
                "RHR",
                "TRR",
                "TRL", 
                "RHL",
                "TML",
                "TFL",
                "FHL",
                "SWMIX1",
                "SWMIX2"
            };
        }
        public class ChannelList
        {
            private static readonly Dictionary<string, string> _ChannelNameList = new()
            {
                { "FL", "Front Left" },
                { "C", "Center" },
                { "FR", "Front Right" },
                { "SRA", "Surround Right" },
                { "SBR", "Surround Back Right" },
                { "SBL", "Surround Back Left" },
                { "SLA", "Surround Left" },
                { "FHR", "Front High Right" },
                { "TFR", "Top Front Right" },
                { "TMR", "Top Middle Right" },
                { "TRR", "Top Rear Right" },
                { "RHR", "Rear High Right" },
                { "RHL", "Rear High Left" },
                { "TRL", "Top Rear Left" },
                { "TML", "Top Middle Left" },
                { "TFL", "Top Front Left" },
                { "FHL", "Front High Left" },
                { "SW1", "Subwoofer Mix 1" },
                { "SW2", "Subwoofer Mix 2" }
            };

            private static readonly ObservableCollection<decimal> _ChLevelList = new()
            {
                -12m,
                -11.5m,
                -11m,
                -10.5m,
                -10m,
                -9.5m,
                -9m,
                -8.5m,
                -8m,
                -7.5m,
                -7m,
                -6.5m,
                -6m,
                -5.5m,
                -5m,
                -4.5m,
                -4m,
                -3.5m,
                -3m,
                -2.5m,
                -2m,
                -1.5m,
                -1m,
                -0.5m,
                0m,
                0.5m,
                1.0m,
                1.5m,
                2.0m,
                2.5m,
                3m,
                3.5m,
                4m,
                4.5m,
                5m,
                5.5m,
                6m,
                6.5m,
                7m,
                7.5m,
                9m,
                8.5m,
                9m,
                9.5m,
                10m,
                10.5m,
                11m,
                11.5m,
                12m
            };

            private static readonly ObservableCollection<string> _CrossoverList = new()
            { "40", "60", "80", "90", "100", "110", "120", "150", "180", "200", "250", "F" };

            [JsonIgnore]
            public static Dictionary<string, string> ChannelNameList
            {
                get
                {
                    return _ChannelNameList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<decimal> ChLevelList
            {
                get
                {
                    return _ChLevelList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<string> CrossoverList
            {
                get
                {
                    return _CrossoverList;
                }
            }
        }

        public class ChannelReportList
        {
            private static readonly ObservableCollection<string> _SetupList = new()
            { "L", "N", "S", "E" };

            private static readonly ObservableCollection<string> _PolarityList = new()
            { "N", "R" };

            [JsonIgnore]
            public static ObservableCollection<string> SetupList
            {
                get
                {
                    return _SetupList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<string> PolarityList
            {
                get
                {
                    return _PolarityList;
                }
            }
        }

        public class MultEQList
        {
            private static readonly Dictionary<string, byte> _CoefChannelList = new()
            {
                { "FL", 0x00 },
                { "C", 0x01 },
                { "FR", 0x02 },
                { "SLA", 0x03 },
                { "SRA", 0x0C },
                { "SW1", 0x0D }
            };
            
            private static readonly ObservableCollection<string> _DispDataList = new()
            { "dispLargeData", "dispSmallData" };

            private static readonly ObservableCollection<string> _SampleRateList = new()
            { "coefficient32kHz", "coefficient441kHz", "coefficient48kHz" };

            private static readonly ObservableCollection<int> _SampleFrequencyList = new()
            { 32000, 44100, 48000 };

            private static readonly ObservableCollection<string> _CurveFilterList = new()
            { "referenceCurveFilter", "flatCurveFilter" };

            private static readonly ObservableCollection<string> _AudyDynSetList = new()
            { "H", "M", "L" };

            private static readonly ObservableCollection<string> _AudyEqSetList = new()
            { "Audy", "Flat" };

            private static readonly ObservableCollection<int> _AudyEqRefList = new()
            { 0, 5, 10, 15 };

            private static readonly ObservableCollection<int> _AudyLfcLevList = new()
            { 1, 2, 3, 4, 5, 6, 7 };

            private static readonly ObservableCollection<string> _AudyFinFlgList = new()
            { "Fin", "NotFin" };

            [JsonIgnore]
            public static Dictionary<string, byte> CoefChannelList
            {
                get
                {
                    return _CoefChannelList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<string> DispDataList
            {
                get
                {
                    return _DispDataList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<string> SampleRateList
            {
                get
                {
                    return _SampleRateList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<int> SampleFrequencyList
            {
                get
                {
                    return _SampleFrequencyList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<string> CurveFilterList
            {
                get
                {
                    return _CurveFilterList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<string> AudyDynSetList
            {
                get
                {
                    return _AudyDynSetList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<string> AudyEqSetList
            {
                get
                {
                    return _AudyEqSetList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<int> AudyEqRefList
            {
                get
                {
                    return _AudyEqRefList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<int> AudyLfcLevList
            {
                get
                {
                    return _AudyLfcLevList;
                }
            }

            [JsonIgnore]
            public static ObservableCollection<string> AudyFinFlgList
            {
                get
                {
                    return _AudyFinFlgList;
                }
            }
        }
    }
}