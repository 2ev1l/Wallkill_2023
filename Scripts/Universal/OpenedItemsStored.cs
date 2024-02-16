using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal
{
    [System.Serializable]
    public class OpenedItemsStored<T> where T : class
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - item
        /// </summary>
        public UnityAction<T> OnItemOpened;
        /// <summary>
        /// <see cref="{T0}"/> - item
        /// </summary>
        public UnityAction<T> OnItemClosed;
        public IReadOnlyList<T> Items => items;
        [SerializeField] private List<T> items = new();
        #endregion fields & properties

        #region methods
        public void ClearAllItems() => items.Clear();
        public T Find(System.Predicate<T> match) => items.Find(match);
        public int FindIndex(System.Predicate<T> match) => items.FindIndex(match);
        public bool IsOpened(System.Predicate<T> match) => items.Exists(match);
        public bool IsOpened(System.Predicate<T> match, out T item)
        {
            item = items.Find(match);
            return item != null;
        }
        public bool TryOpenItem(T newItem, System.Predicate<T> containsMatch, out T existItem)
        {
            if (IsOpened(containsMatch, out existItem)) return false;
            OpenItem(newItem);
            return true;
        }
        public bool TryCloseItem(System.Predicate<T> containsMatch)
        {
            if (!IsOpened(containsMatch, out T item)) return false;
            CloseItem(item);
            return true;
        }
        protected virtual void CloseItem(T item)
        {
            items.Remove(item);
            OnItemClosed?.Invoke(item);
        }
        protected virtual void OpenItem(T item)
        {
            items.Add(item);
            OnItemOpened?.Invoke(item);
        }
        public void CloseAllItems()
        {
            for (int i = 0; i < items.Count; ++i)
            {
                CloseItem(items[i]);
                --i;
            }
        }
        public OpenedItemsStored() { }
        public OpenedItemsStored(List<T> openedItems)
        {
            this.items = openedItems;
        }
        #endregion methods
    }
}