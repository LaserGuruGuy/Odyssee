using System.Text;

namespace MultEQAvrAdapter
{
    namespace MultEQApp
    {
        public class ChannelReport
        {
            #region Properties
            public int? EnSpeakerConnect { get; set; }

            public int? CustomEnSpeakerConnect { get; set; }

            public bool? IsReversePolarity { get; set; }

            public decimal? Distance { get; set; }
            #endregion

            #region Methods
            public bool ShouldSerializeCustomEnSpeakerConnect()
            {
                if (CustomEnSpeakerConnect != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                foreach (var property in this.GetType().GetProperties())
                {
                    sb.Append(property + "=" + property.GetValue(this, null) + "\r\n");
                }

                return sb.ToString();
            }
            #endregion
        }
    }
}