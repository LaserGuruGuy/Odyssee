using System.Text;
using System.Collections.Generic;

namespace MultEQAvrAdapter
{
    namespace MultEQApp
    {
        public class DetectedChannel
        {
            #region Properties
            public string CustomCrossover { get; set; }

            public int? EnChannelType { get; set; }

            public bool? IsSkipMeasurement { get; set; }

            public string CustomLevel { get; set; }

            public decimal? FrequencyRangeRolloff { get; set; }

            public decimal? CustomDistance { get; set; }

            public string[] CustomTargetCurvePoints { get; set; }

            public string CommandId { get; set; }

            public string CustomSpeakerType { get; set; }

            public string DelayAdjustment { get; set; }

            public Dictionary<string, string[]> ReferenceCurveFilter { get; set; }

            public ChannelReport ChannelReport { get; set; }

            public Dictionary<string, string[]> ResponseData { get; set; }

            public Dictionary<string, string[]> FlatCurveFilter { get; set; }

            public string TrimAdjustment { get; set; }

            public bool? MidrangeCompensation { get; set; }
            #endregion

            #region Methods
            public bool ShouldSerializeCustomCrossover()
            {
                return (!string.IsNullOrEmpty(CustomCrossover));
            }

            public bool ShouldSerializeCustomLevel()
            {
                return (!string.IsNullOrEmpty(CustomLevel));
            }

            public bool ShouldSerializeCustomTargetCurvePoints()
            {
                return (!string.IsNullOrEmpty(CustomCrossover));
            }

            public bool ShouldSerializeCustomSpeakerType()
            {
                return (!string.IsNullOrEmpty(CustomSpeakerType));
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                foreach (var property in this.GetType().GetProperties())
                {
                    sb.Append(property + "=" + property.GetValue(this, null) + "\r\n");
                }

                if (ChannelReport != null) sb.Append(ChannelReport.ToString());
                if (ResponseData != null) sb.Append(ResponseData.ToString());

                return sb.ToString();
            }
            #endregion
        }
    }
}