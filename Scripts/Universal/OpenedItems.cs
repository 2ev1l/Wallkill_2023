using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal
{
    [System.Serializable]
    public class OpenedItems
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - itemId
        /// </summary>
        public UnityAction<int> OnItemOpened;
        /// <summary>
        /// <see cref="{T0}"/> - itemId
        /// </summary>
        public UnityAction<int> OnItemClosed;
        public IReadOnlyList<int> ItemsId => itemsId;
        [SerializeField] private List<int> itemsId = new();
        #endregion fields & properties

        #region methods
        public int FindIndex(int itemId) => itemsId.FindIndex(x => x == itemId);
        public bool IsOpened(int itemId) => itemsId.Exists(x => x == itemId);
        public bool TryOpenItem(int itemId)
        {
            if (IsOpened(itemId)) return false;
            OpenItem(itemId);
            return true;
        }
        public bool TryCloseItem(int itemId)
        {
            if (!IsOpened(itemId)) return false;
            CloseItem(itemId);
            return true;
        }
        protected virtual void CloseItem(int itemId)
        {
            itemsId.Remove(itemId);
            OnItemClosed?.Invoke(itemId);
        }
        protected virtual void OpenItem(int itemId)
        {
            itemsId.Add(itemId);
            OnItemOpened?.Invoke(itemId);
        }
        public void CloseAllItems()
        {
            for (int i = 0; i < itemsId.Count; ++i)
            {
                CloseItem(itemsId[i]);
                --i;
            }
        }
        public OpenedItems()
        {
            this.itemsId = new();
        }
        public OpenedItems(List<int> openedItemsId)
        {
            this.itemsId = openedItemsId;
        }
        #endregion methods
    }
}