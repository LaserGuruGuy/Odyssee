using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Audyssey
{
    namespace MultEQAvr
    {
        /// <summary>
        /// </summary>
        public class AnnotationObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
        {
            //------------------------------------------------------
            //
            //  Constructors
            //
            //------------------------------------------------------

            #region Constructors
            /// <summary>
            /// Initializes a new instance of AnnotationObservableCollection that is empty and has default initial capacity.
            /// </summary>
            public AnnotationObservableCollection() : base()
            {
                _listener = new PropertyChangedEventHandler(OnItemPropertyChanged);
            }

            /// <summary>
            /// Initializes a new instance of the AnnotationObservableCollection class
            /// that contains elements copied from the specified list
            /// </summary>
            /// <param name="list">The list whose elements are copied to the new list.</param>
            /// <remarks>
            /// The elements are copied onto the AnnotationObservableCollection in the
            /// same order they are read by the enumerator of the list.
            /// </remarks>
            /// <exception cref="ArgumentNullException"> list is a null reference </exception>
            public AnnotationObservableCollection(List<T> list) : base(list)
            {
                _listener = new PropertyChangedEventHandler(OnItemPropertyChanged);
            }

            #endregion Constructors

            //------------------------------------------------------
            //
            //  Public Events
            //
            //------------------------------------------------------

            //------------------------------------------------------
            //
            //  Protected Methods
            //
            //------------------------------------------------------

            #region Protected Methods

            /// <summary>
            /// called by base class Collection&lt;T&gt; when the list is being cleared;
            /// raises a CollectionChanged event to any listeners
            /// </summary>
            protected override void ClearItems()
            {
                foreach (INotifyPropertyChanged item in this)
                {
                    SetOwned(item, false);
                }

                ProtectedClearItems();
            }

            /// <summary>
            /// called by base class Collection&lt;T&gt; when an item is removed from list;
            /// raises a CollectionChanged event to any listeners
            /// </summary>
            protected override void RemoveItem(int index)
            {
                T removedItem = this[index];

                SetOwned(removedItem, false);

                base.RemoveItem(index);
            }

            /// <summary>
            /// called by base class Collection&lt;T&gt; when an item is added to list;
            /// raises a CollectionChanged event to any listeners
            /// </summary>
            protected override void InsertItem(int index, T item)
            {
                //if (ItemOwned(item))
                //    throw new ArgumentException(SR.Get(Grid.AlreadyHasParent));

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
                    SetOwned(item, true);
                    return;
                }

                base.InsertItem(index, item);
                SetOwned(item, true);
            }

            /// <summary>
            /// called by base class Collection&lt;T&gt; when an item is added to list;
            /// raises a CollectionChanged event to any listeners
            /// </summary>
            protected override void SetItem(int index, T item)
            {
                //if (ItemOwned(item))
                //    throw new ArgumentException(SR.Get(SRID.AlreadyHasParent));

                T originalItem = this[index];

                SetOwned(originalItem, false);

                ProtectedSetItem(index, item);

                SetOwned(item, true);
            }

            /// <summary>
            ///     Virtual methods allowing subclasses to change the eventing
            ///     behavior for the ClearItems method.  The default behavior
            ///     is to call ObservableCollection's method.
            /// </summary>
            protected virtual void ProtectedClearItems()
            {
                // Use the standard built-in event
                base.ClearItems();
            }

            /// <summary>
            ///     Virtual methods allowing subclasses to change the eventing
            ///     behavior for the SetItem method.  The default behavior
            ///     is to call Collection's defaut method and fire a single
            ///     event.
            /// </summary>
            /// <param name="index">index of the item being set</param>
            /// <param name="item">item to set at that index</param>
            protected virtual void ProtectedSetItem(int index, T item)
            {
                // We only want to fire one event here - this collection contains items that
                // are within a Resource so an assignment means one resource change, period.
                Items[index] = item;    // directly set Collection<T> inner Items collection
                OnPropertyChanged(new PropertyChangedEventArgs(CountString));
                OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            /// <summary>
            ///     Allows subclasses to call superclass's SetItem method without
            ///     any additional functionality added by this class.
            /// </summary>
            /// <param name="index">index of item being set</param>
            /// <param name="item">item to set in that index</param>
            protected void ObservableCollectionSetItem(int index, T item)
            {
                base.SetItem(index, item);
            }

            // raise CollectionChanged event to any listeners


            /// <summary>
            ///     When an item we contain fires a PropertyChanged event we fire
            ///     a collection changed event letting listeners know the collection
            ///     has changed in some way.  We don't care about the particulars of
            ///     the event - just want to pass up the chain of objects that something
            ///     has changed.
            /// </summary>
            /// <param name="sender">the object whose property changed</param>
            /// <param name="e">the event args describing the property that changed</param>
            protected virtual void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            #endregion Protected Methods


            //------------------------------------------------------
            //
            //  Private Methods
            //
            //------------------------------------------------------

            #region Private Methods


            // returns whether this item already belongs to a parent object
            private bool ItemOwned(Object item)
            {
                if (item != null)
                {
                    return true;
                }
                return false;
            }

            // sets whether this object belongs to a parent object
            private void SetOwned(Object item, bool owned)
            {
                if (item != null)
                {
                    if (owned)
                    {
                        ((INotifyPropertyChanged)item).PropertyChanged += _listener;
                    }
                    else
                    {
                        ((INotifyPropertyChanged)item).PropertyChanged -= _listener;
                    }
                }
            }

            #endregion Private Methods

            //------------------------------------------------------
            //
            //  Private Fields
            //
            //------------------------------------------------------

            #region Private & Internal Fields

            private readonly PropertyChangedEventHandler _listener = null;
            internal readonly string CountString = "Count";
            internal readonly string IndexerName = "Item[]";

            #endregion Private & Internal Fields
        }
    }
}
