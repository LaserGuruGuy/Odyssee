using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Audyssey
{
    namespace MultEQ.List
    {
        public class ChannelList
        {
            private static ObservableCollection<decimal> _ChLevelList = new()
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

            private static ObservableCollection<string> _CrossoverList = new()
            { "40", "60", "80", "90", "100", "110", "120", "150", "180", "200", "250", "F" };

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
            private static ObservableCollection<string> _SetupList = new()
            { "L", "N", "S", "E" };

            [JsonIgnore]
            public static ObservableCollection<string> SetupList
            {
                get
                {
                    return _SetupList;
                }
            }
        }

        public class MultEQList
        {
            private static ObservableCollection<string> _DispDataList = new()
            { "dispLargeData", "dispSmallData" };

            private static ObservableCollection<string> _SampleRateList = new()
            { "coefficient32kHz", "coefficient441kHz", "coefficient48kHz" };

            private static ObservableCollection<string> _AudyDynSetList = new()
            { "H", "M", "L" };

            private static ObservableCollection<string> _AudyEqSetList = new()
            { "Audy", "Flat" };

            private static ObservableCollection<int> _AudyEqRefList = new()
            { 0, 5, 10, 15 };

            private static ObservableCollection<int> _AudyLfcLevList = new()
            { 1, 2, 3, 4, 5, 6, 7 };

            private static ObservableCollection<string> _AudyFinFlgList = new()
            { "Fin", "NotFin" };

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