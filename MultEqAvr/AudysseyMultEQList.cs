using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Audyssey
{
    namespace MultEQ.List
    {
        public class StatusList
        {
            [JsonIgnore]
            public static Collection<string> ChSetupList { get;  } = new()
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
            public static Collection<decimal> ChLevelList { get; } = new()
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
            public static Collection<object> CrossoverList { get; } = new()
            { (long)40, (long)60, (long)80, (long)90, (long)100, (long)110, (long)120, (long)150, (long)180, (long)200, (long)250, "F" };
            [JsonIgnore]
            public static Collection<double> FilterFrequencies { get; } = new() { 19.6862664, 22.09708691, 24.80314144, 27.84058494, 31.25, 35.07693901, 39.37253281, 44.19417382, 49.60628287, 55.68116988, 62.5, 70.15387802, 78.74506562, 88.38834765, 99.21256575, 111.3623398, 125, 140.307756, 157.4901312, 176.7766953, 198.4251315, 222.7246795, 250, 280.6155121, 314.9802625, 353.5533906, 396.850263, 445.4493591, 500, 561.2310242, 629.9605249, 707.1067812, 793.700526, 890.8987181, 1000, 1122.462048, 1259.92105, 1414.213562, 1587.401052, 1781.797436, 2000, 2244.924097, 2519.8421, 2828.427125, 3174.802104, 3563.594873, 4000, 4489.848193, 5039.6842, 5656.854249, 6349.604208, 7127.189745, 8000, 8979.696386, 10079.3684, 11313.7085, 12699.20842, 14254.37949, 16000, 17959.39277, 20158.7368 };
            [JsonIgnore]
            public static Collection<double> DisplayFrequencies { get; } = new() { 63, 125, 250, 500, 1000, 2000, 4000, 8000, 16000 };

        }

        public class ChannelReportList
        {
            [JsonIgnore]
            public static Collection<string> SetupList { get; } = new()
            { "L", "N", "S", "E" };

            [JsonIgnore]
            public static Collection<string> PolarityList { get; } = new()
            { "N", "R" };
        }

        public class MultEQList
        {
            [JsonIgnore]
            public static Dictionary<string, byte> CoefChannelList { get; } = new()
            {
                { "FL",  0 },
                { "C",   1 },
                { "FR",  2 },
                { "SRA", 12 },
                { "SBR", 7 },
                { "SBL", 8 },
                { "SLA", 3 },
                { "FHR", 0xFF }, /* TODO */
                { "TFR", 0xFF }, /* TODO */
                { "TMR", 0xFF }, /* TODO */
                { "RHR", 0xFF }, /* TODO */
                { "TRR", 0xFF }, /* TODO */
                { "TRL", 0xFF }, /* TODO */
                { "RHL", 0xFF }, /* TODO */
                { "TML", 0xFF }, /* TODO */
                { "TFL", 0xFF }, /* TODO */
                { "FHL", 0xFF }, /* TODO */
                { "SW1", 13 },
                { "SW2", 14 }
            };

            [JsonIgnore]
            public static Collection<string> DispDataList { get; } = new()
            { "dispLargeData", "dispSmallData" };

            [JsonIgnore]
            public static Collection<string> SampleRateList { get; } = new()
            { "coefficient32kHz", "coefficient441kHz", "coefficient48kHz" };

            [JsonIgnore]
            public static Collection<int> SampleFrequencyList { get; } = new()
            { 32000, 44100, 48000 };

            [JsonIgnore]
            public static Collection<int> SampleCountList { get; } = new()
            { 1024, 704 };

            [JsonIgnore]
            public static Collection<string> CurveFilterList { get; } = new()
            { "referenceCurveFilter", "flatCurveFilter" };

            [JsonIgnore]
            public static Collection<string> AudyDynSetList { get; } = new()
            { "H", "M", "L" };

            [JsonIgnore]
            public static Collection<string> AudyEqSetList { get; } = new()
            { "Audy", "Flat" };

            [JsonIgnore]
            public static Collection<int> AudyEqRefList { get; } = new()
            { 0, 5, 10, 15 };

            [JsonIgnore]
            public static Collection<int> AudyLfcLevList { get; } = new()
            { 1, 2, 3, 4, 5, 6, 7 };

            [JsonIgnore]
            public static Collection<string> AudyFinFlgList { get; } = new()
            { "Fin", "NotFin" };
        }
    }
}