using Data.Stored;
using EditorCustom.Attributes;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI
{
    public abstract class ItemListBase<Item, UpdateValue> : MonoBehaviour where Item : Component, IListUpdater<UpdateValue>
    {
        #region fields & properties
        public ItemList<Item, UpdateValue> ItemList => itemList;
        [SerializeField] private ItemList<Item, UpdateValue> itemList;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            UpdateListData();
        }
        protected virtual void OnDisable()
        {

        }
        [SerializedMethod]
        public void SwitchPage(bool isNext) => itemList.SwitchPage(isNext);
        public abstract void UpdateListData();
        #endregion methods
    }
}