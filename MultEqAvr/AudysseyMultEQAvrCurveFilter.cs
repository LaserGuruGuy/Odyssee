using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Audyssey
{
    namespace MultEQAvr
    {
        /// <summary>
        /// This class is not used. Do we want to write a class to dictionary converter?
        /// </summary>
        public class CurveFilter: INotifyPropertyChanged
        {
            #region BackingField
            private ObservableCollection<float> _Coefficient48kHz;  //[1024] or [704]
            private ObservableCollection<float> _Coefficient441kHz; //[1024] or [704]
            private ObservableCollection<float> _Coefficient32kHz;  //[1024] or [704]
            private ObservableCollection<float> _DispLargeData = new() { 0, 0, 0, -2, -2, 1, 2, 1, -8, -5, -8, 0, -2, -4, -2, 3, 3, 2, 0, -1, -6, -3, -5, -3, 0, -5, 0, 0, 0, -1, -2, -2, 1, 2, 0, -1, 0, 0, 1, 3, 2, 1, 2, 1, 0, -1, -2, -2, 0, -1, 1, 3, 1, 1, 2, 0, 1, 0, 3, 5, 5 };     //[61]
            private ObservableCollection<float> _DispSmallData = new() { -4, 0, -4, -2, 0, 2, -1, 1, 3 };     //[9]
            #endregion

            #region Properties
            public ObservableCollection<float> Coefficient48kHz
            {
                get
                {
                    return _Coefficient48kHz;
                }
                set
                {
                    _Coefficient48kHz = value;
                }
            }
            public ObservableCollection<float> Coefficient441kHz
            {
                get
                {
                    return _Coefficient441kHz;
                }
                set
                {
                    _Coefficient441kHz = value;
                }
            }
            public ObservableCollection<float> Coefficient32kHz
            {
                get
                {
                    return _Coefficient32kHz;
                }
                set
                {
                    _Coefficient32kHz = value;
                }
            }
            public ObservableCollection<float> DispSmallData
            {
                get
                {
                    return _DispSmallData;
                }
                set
                {
                    _DispSmallData = value;
                }
            }
            public ObservableCollection<float> DispLargeData
            {
                get
                {
                    return _DispLargeData;
                }
                set
                {
                    _DispLargeData = value;
                }
            }
            #endregion

            #region Methods
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
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
        }
    }
}