using System;
using System.ComponentModel;
using Newtonsoft.Json;

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

        public class RunningAverage : INotifyPropertyChanged
        {
            #region BackingField
            private double _Average = 0;
            private double _Count = 0;
            #endregion

            #region Properties
            public double Average { get {return 10.0*Math.Log10(_Average); } }
            public UInt16? Value
            {
                set 
                {
                    double lin_value = Math.Pow(10.0, (double)value/1000.0);
                    _Count += 1;
                    _Average = (_Average * (_Count - 1) + lin_value) / _Count;
                }
            }
            #endregion

            #region Methods
            public void ResetAverage() { _Average = 7500; }
            public void ResetCount() { _Count = 0; }
            public void Reset()
            {
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(GetType()))
                {
                    if (prop.CanResetValue(this))
                    {
                        prop.ResetValue(this);
                        RaisePropertyChanged(prop.Name);
                    }
                }
            }
            private void RaisePropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged = delegate { };
            #endregion
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
            private UInt16? _SPLValue = null;
            private RunningAverage _SPLAvg = new();
            #endregion

            #region Properties
            [JsonIgnore]
            [JsonConverter(typeof(HexStringJsonConverter))]
            public UInt16? SPLValue
            {
                get
                {
                    return _SPLValue;
                }
                set
                {
                    if (value != 0xFE0C)
                    {
                        _SPLAvg.Value = _SPLValue = value;
                        RaisePropertyChanged("SPLValue");
                        RaisePropertyChanged("SPLValuedB");
                        RaisePropertyChanged("SPLAvgdB");
                    }
                }
            }
            [JsonIgnore]
            public bool IsValidSPLValue
            {
                get
                {
                    return _SPLValue != null;
                }
            }
            [JsonIgnore]
            public double SPLValuedB
            {
                get
                {
                    return _SPLValue == null ? 7500 : (double)_SPLValue / 100.0;
                }
            }
            [JsonIgnore]
            public double SPLAvgdB
            {
                get
                {
                    return _SPLAvg.Average;
                }
            }
            #endregion

            #region Methods
            private void ResetSPLValue()
            {
                _SPLValue = null;
            }
            private void ResetSPLAvg()
            {
                _SPLAvg.Reset();
            }
            #endregion
        }
    }
}