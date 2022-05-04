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
            [JsonIgnore]
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
            [JsonIgnore]
            public static Dictionary<string, string> ChannelNameList { get; } = new()
            {
                { "FL", "Front Left" },
                { "C", "Center" },
                { "FR", "Front Right" },
                { "SRA", "Surround Right" },
                { "SBR", "Surround Back Right" },
                { "SBL", "Surround Back Left" },
                { "SLA", "Surround Left" },
                { "FHR", "Front Height Right" },
                { "TFR", "Top Front Right" },
                { "TMR", "Top Middle Right" },
                { "TRR", "Top Rear Right" },
                { "RHR", "Rear Height Right" },
                { "RHL", "Rear Height Left" },
                { "TRL", "Top Rear Left" },
                { "TML", "Top Middle Left" },
                { "TFL", "Top Front Left" },
                { "FHL", "Front Height Left" },
                { "SW1", "Subwoofer Mix 1" },
                { "SW2", "Subwoofer Mix 2" }
            };

            [JsonIgnore]
            public static ObservableCollection<decimal> ChLevelList { get; } = new()
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

            [JsonIgnore]
            public static ObservableCollection<object> CrossoverList { get; } = new()
            { (long)40, (long)60, (long)80, (long)90, (long)100, (long)110, (long)120, (long)150, (long)180, (long)200, (long)250, "F" };
        }

        public class ChannelReportList
        {
            [JsonIgnore]
            public static ObservableCollection<string> SetupList { get; } = new()
            { "L", "N", "S", "E" };

            [JsonIgnore]
            public static ObservableCollection<string> PolarityList { get; } = new()
            { "N", "R" };
        }

        public class MultEQList
        {
            [JsonIgnore]
            public static Dictionary<string, byte> CoefChannelList { get; } = new()
            {
                { "FL", 0x00 },
                { "C", 0x01 },
                { "FR", 0x02 },
                { "SLA", 0x03 },
                { "SRA", 0x0C },
                { "SW1", 0x0D }
            };

            [JsonIgnore]
            public static ObservableCollection<string> DispDataList { get; } = new()
            { "dispLargeData", "dispSmallData" };

            [JsonIgnore]
            public static ObservableCollection<string> SampleRateList { get; } = new()
            { "coefficient32kHz", "coefficient441kHz", "coefficient48kHz" };

            [JsonIgnore]
            public static ObservableCollection<int> SampleFrequencyList { get; } = new()
            { 32000, 44100, 48000 };

            [JsonIgnore]
            public static ObservableCollection<int> SampleCountList { get; } = new()
            { 1024, 704 };

            [JsonIgnore]
            public static ObservableCollection<string> CurveFilterList { get; } = new()
            { "referenceCurveFilter", "flatCurveFilter" };

            [JsonIgnore]
            public static ObservableCollection<string> AudyDynSetList { get; } = new()
            { "H", "M", "L" };

            [JsonIgnore]
            public static ObservableCollection<string> AudyEqSetList { get; } = new()
            { "Audy", "Flat" };

            [JsonIgnore]
            public static ObservableCollection<int> AudyEqRefList { get; } = new()
            { 0, 5, 10, 15 };

            [JsonIgnore]
            public static ObservableCollection<int> AudyLfcLevList { get; } = new()
            { 1, 2, 3, 4, 5, 6, 7 };

            [JsonIgnore]
            public static ObservableCollection<string> AudyFinFlgList { get; } = new()
            { "Fin", "NotFin" };
        }
    }
}