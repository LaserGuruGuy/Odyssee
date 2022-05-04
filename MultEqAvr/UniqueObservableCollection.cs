using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Audyssey
{
    namespace MultEQAvr
    {
        public static class Extensions
        {
            public static void Sort<T, K>(this UniqueObservableCollection<T> source, K Order)
            {
                if (source.GetType() == typeof(UniqueObservableCollection<Dictionary<string, string>>) &&
                    Order.GetType() == typeof(Collection<string>))
                {
                    UniqueObservableCollection<Dictionary<string, string>> collection = source as UniqueObservableCollection<Dictionary<string, string>>;
                    Collection<string> SpeakerOrder = Order as Collection<string>;

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

            public static void Sort<TKey, TValue , TOrder>(this Dictionary<TKey, TValue> source, TOrder Order)
            {
                Dictionary<string, double[]> collection = source as Dictionary<string, double[]>;
                ObservableCollection<string> SampleRateOrder = Order as ObservableCollection<string>;

                for (var i = source.Count() - 1; i > 0; i--)
                {
                    for (var j = 1; j <= i; j++)
                    {
                        KeyValuePair<string, double[]> o1 = collection.ElementAt(j - 1);
                        KeyValuePair<string, double[]> o2 = collection.ElementAt(j);

                        if (SampleRateOrder.IndexOf(o1.Key) > SampleRateOrder.IndexOf(o2.Key))
                        {
                            collection.Remove(o1.Key);
                            collection.Add(o1.Key, o1.Value);
                        }
                    }
                }
            }

            public static TKey SmartReverseLookup<TKey, TValue>(this Dictionary<TKey, TValue> me, TValue value, TKey DefaultIfEmpty)
            {
                try
                {
                    if (me.ContainsValue(value))
                        return me.First(a => a.Value.Equals(value)).Key;

                    return DefaultIfEmpty;
                }
                catch
                {
                    return DefaultIfEmpty;
                }
            }

            public static TKey[] ReverseLookup<TKey, TValue>(this Dictionary<TKey, TValue> me, TValue value)
            {
                try
                {
                    if (me.ContainsValue(value))
                        return me.Where(a => a.Value.Equals(value)).Select(b => b.Key).ToArray();
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        //Dictionary<string, int> numsList = new Dictionary<string, int>()
        //{
        //    {"Test One", 1}
        //    {"Test Two", 2}
        //    {"Test Three", 3}
        //    {"Test Four", 3}
        //}
        //
        //string[] foundKeys;
        //foundKeys = numsList.ReverseLookup(3);
        //
        //Console.WriteLine(numsList.SmartReverseLookup(2, "NOT FOUND"));

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