using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public class UniqueObservableCollection<T> : ObservableCollection<T>
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
                if (item.GetType() == typeof(Dictionary<string, string>))
                {
                    foreach (var Item in Items)
                    {
                        if ((Item as Dictionary<string, string>).ContainsKey((item as Dictionary<string, string>).ElementAt(0).Key))
                        {
                            index = Items.IndexOf(Item);
                            Items.Remove(Item);
                            break;
                        }
                    }
                    base.InsertItem(index, item);
                }
                else if (item.GetType() == typeof(Dictionary<string, int>))
                {
                    foreach (var Item in Items)
                    {
                        if ((Item as Dictionary<string, int>).ContainsKey((item as Dictionary<string, int>).ElementAt(0).Key))
                        {
                            index = Items.IndexOf(Item);
                            Items.Remove(Item);
                            break;
                        }
                    }
                    base.InsertItem(index, item);
                }
                else if (item.GetType() == typeof(Dictionary<string, object>))
                {
                    foreach (var Item in Items)
                    {
                        if ((Item as Dictionary<string, object>).ContainsKey((item as Dictionary<string, object>).ElementAt(0).Key))
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