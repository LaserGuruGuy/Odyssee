using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace MultEQAvrAdapter
{
    namespace AdapterList
    {
        public class StatusList
        {
            [JsonIgnore]
            public static ObservableCollection<string> AmpAssignTypeList { get; } = new()
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
        }
        public class InfoList
        {
            [JsonIgnore]
            public static ObservableCollection<string> MultEQTypeList { get; } = new()
            { "MultEQ", "MultEQXT", "MultEQXT32" };
        }
    }
}