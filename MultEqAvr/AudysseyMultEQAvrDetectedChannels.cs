using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Audyssey.MultEQ.List;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public class DetectedChannel : ChannelList, INotifyPropertyChanged
        {
            #region BackingField
            private string _Channel;
            private string _Setup;
            private bool? _Skip;
            private ChannelReport _ChannelReport = new();
            private Dictionary<string, Int32[]> _ResponseData;
            private decimal? _ChLevel = 0m;
            private object? _Crossover = "F";
            #endregion

            #region Properties
            public string Channel
            {
                get
                {
                    return _Channel;
                }
                set
                {
                    _Channel = value;
                    RaisePropertyChanged("Channel");
                }
            }
            public string Setup
            {
                get
                {
                    return _Setup;
                }
                set
                {
                    _Setup = value;
                    RaisePropertyChanged("Setup");
                }
            }
            public bool? Skip
            {
                get
                {
                    return _Skip;
                }
                set
                {
                    _Skip = value;
                    RaisePropertyChanged("Sticky");
                }
            }
            public ChannelReport ChannelReport
            {
                get
                {
                    return _ChannelReport;
                }
                set
                {
                    _ChannelReport = ChannelReport;
                    RaisePropertyChanged("ChannelReport");
                }
            }
            public Dictionary<string, Int32[]> ResponseData
            {
                get
                {
                    return _ResponseData;
                }
                set
                {
                    _ResponseData = value;
                    RaisePropertyChanged("ResponseData");
                }
            }
            public decimal? ChLevel
            {
                get
                {
                    return _ChLevel;
                }
                set
                {
                    _ChLevel = value;
                    RaisePropertyChanged("ChLevel");
                }
            }
            public object? Crossover
            {
                get
                {
                    return _Crossover;
                }
                set
                {
                    _Crossover = value;
                    RaisePropertyChanged("Crossover");
                }
            }
            #endregion

            #region Methods
            private void ResetChannel() { _Channel = null; }
            private void ResetSetup() { _Setup = null; }
            private void ResetSkip() { _Skip = null; }
            private void ResetChannelReport() { _ChannelReport = new(); }
            private void ResetResponseData() { _ResponseData = null; }
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
            protected void RaisePropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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

            #region Events
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
        }
    }
}