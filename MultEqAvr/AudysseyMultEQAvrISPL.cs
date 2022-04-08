using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public sealed class HexStringJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(UInt16).Equals(objectType);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue($"{value:x}");
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.ValueType.FullName == typeof(string).FullName)
                {
                    string str = (string)reader.Value;
                    if (str == null)
                    {
                        throw new JsonSerializationException();
                    }
                    return Convert.ToUInt16(str, 16);
                }
                else
                {
                    throw new JsonSerializationException();
                }
            }
        }

        public interface ISPLValue
        {
            #region Properties
            [JsonConverter(typeof(HexStringJsonConverter))]
            public UInt16? SPLValue { get; set; }
            #endregion
        }

        public partial class AudysseyMultEQAvr : ISPLValue, INotifyPropertyChanged
        {
            #region BackingField
            public UInt16? _SPLValue = null;
            #endregion

            #region Properties
            [JsonConverter(typeof(HexStringJsonConverter))]
            public UInt16? SPLValue
            {
                get
                {
                    return _SPLValue;
                }
                set
                {
                    _SPLValue = value;
                    RaisePropertyChanged("SPLValue");
                    if (value != 0xFE0C)
                    {
                        RaisePropertyChanged("SPLValuedB");
                    }
                }
            }
            public double SPLValuedB
            {
                get
                {
                    return 20.0*Math.Log10((double)_SPLValue);
                }
            }
            #endregion

            #region ResetMethods
            public void ResetSPLValue()
            {
                _SPLValue = null;
                RaisePropertyChanged("SPLValue");
            }
            #endregion
        }
    }
}