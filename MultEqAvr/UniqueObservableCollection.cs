using System.ComponentModel;
using System.Linq;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public class UniqueObservableCollection<T> : AnnotationObservableCollection<T> where T : INotifyPropertyChanged
        {
            protected override void InsertItem(int index, T item)
            {
                if (item.GetType() == typeof(DetectedChannel))
                {
                    foreach (var Item in Items)
                    {
                        if ((Item as DetectedChannel).Channel.Equals((item as DetectedChannel).Channel))
                        {
                            return;
                        }
                    }
                    Items.Add(item);
                }
                if (item.GetType() == typeof(ObservableDictionary))
                {
                    foreach (var Item in Items)
                    {
                        if ((Item as ObservableDictionary).ContainsKey((item as ObservableDictionary).ElementAt(0).Key))
                        {
                            index = Items.IndexOf(Item);
                            Items.Remove(Item);
                            break;
                        }
                    }
                    base.InsertItem(index, item);
                }
            }
        }
    }
}