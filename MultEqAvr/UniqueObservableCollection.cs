using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Audyssey
{
    namespace MultEQAvr
    {
        static class Extensions
        {
            static readonly StringCollection SpeakerOrder = new() { "FL", "C", "FR", "SRA", "SWMIX1", "SLA", "SWMIX2" };
            public static void Sort<T>(this UniqueObservableCollection<T> source)
            {
                if (source.GetType() == typeof(UniqueObservableCollection<Dictionary<string, string>>))
                {
                    UniqueObservableCollection<Dictionary<string, string>> collection = source as UniqueObservableCollection<Dictionary<string, string>>;
                    for (var i = source.Count() - 1; i > 0; i--)
                    {
                        for (var j = 1; j <= i; j++)
                        {
                            Dictionary<string, string> o1 = collection[j - 1];
                            Dictionary<string, string> o2 = collection[j];

                            var t1 = (o1 as Dictionary<string, string>).ElementAt(0);
                            var t2 = (o2 as Dictionary<string, string>).ElementAt(0);

                            if (SpeakerOrder.IndexOf(t1.Key) > SpeakerOrder.IndexOf(t2.Key))
                            {
                                collection.Remove(o1);
                                collection.Insert(j, o1);
                            }
                        }
                    }
                }
            }
        }

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
                    return;
                }
                else if (item.GetType() == typeof(Dictionary<string, string>))
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
                }
                base.InsertItem(index, item);
            }
        }
    }
}