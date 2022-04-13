using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Audyssey
{
    namespace MultEQ.List
    {
        public class ChannelList
        {
            static ObservableCollection<decimal> _ChLevelList = new()
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

            static ObservableCollection<string> _CrossoverList = new()
            { " ", "40", "60", "80", "90", "100", "110", "120", "150", "180", "200", "250", "F" };

            static ObservableCollection<string> _SetupList = new()
            { "L", "N", "S", "E" };

            static ObservableCollection<string> _SpConfigList = new()
            { " ", "S", "L" };

            [JsonIgnore]
            public ObservableCollection<decimal> ChLevelList
            {
                get
                {
                    return _ChLevelList;
                }
            }

            [JsonIgnore]
            public ObservableCollection<string> CrossoverList
            {
                get
                {
                    return _CrossoverList;
                }
            }

            [JsonIgnore]
            public ObservableCollection<string> SetupList
            {
                get
                {
                    return _SetupList;
                }
            }

            [JsonIgnore]
            public ObservableCollection<string> SpConfigList
            {
                get
                {
                    return _SpConfigList;
                }
            }
        }

        public class AvrStatusList
        {
            static ObservableCollection<string> _AmpAssignList = new()
            {
                "FrontA",
                "FrontB",
                "Type3",
                "Type4",
                "Type5",
                "Type6",
                "Type7",
                "Type8",
                "Type9",
                "Type10",
                "Type11",
                "Type12",
                "Type13",
                "Type14",
                "Type15",
                "Type16",
                "Type17",
                "Type18",
                "Type19",
                "Type20"
            };

            [JsonIgnore]
            public ObservableCollection<string> AmpAssignList
            {
                get
                {
                    return _AmpAssignList;
                }
            }
        }

        public class AvrInfoList
        {
            static ObservableCollection<string> _EQTypeList = new()
            {
                "MultEQ",
                "MultEQXT",
                "MultEQXT32"
            };

            [JsonIgnore]
            public ObservableCollection<string> EQTypeList
            {
                get
                {
                    return _EQTypeList;
                }
            }
        }

        public class MultEQList
        {

            static ObservableCollection<string> _AudyDynSetList = new()
            { "H", "M", "L" };

            static ObservableCollection<string> _AudyEqSetList = new()
            { "Audy", "Flat" };

            static ObservableCollection<int> _AudyEqRefList = new()
            { 0, 5, 10, 15 };

            static ObservableCollection<int> _AudyLfcLevList = new()
            { 1, 2, 3, 4, 5, 6, 7 };

            static ObservableCollection<string> _AudyFinFlgList = new()
            { "Fin", "NotFin" };

            [JsonIgnore]
            public ObservableCollection<string> AudyDynSetList
            {
                get
                {
                    return _AudyDynSetList;
                }
            }

            [JsonIgnore]
            public ObservableCollection<string> AudyEqSetList
            {
                get
                {
                    return _AudyEqSetList;
                }
            }

            [JsonIgnore]
            public ObservableCollection<int> AudyEqRefList
            {
                get
                {
                    return _AudyEqRefList;
                }
            }

            [JsonIgnore]
            public ObservableCollection<int> AudyLfcLevList
            {
                get
                {
                    return _AudyLfcLevList;
                }
            }

            [JsonIgnore]
            public ObservableCollection<string> AudyFinFlgList
            {
                get
                {
                    return _AudyFinFlgList;
                }
            }
        }
    }
}