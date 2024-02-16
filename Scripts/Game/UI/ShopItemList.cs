using Data.Stored;
using Game.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public class ShopItemList : ItemListBase<ShopItemChoosable, int>
    {
        #region fields & properties
        [SerializeField] private ShopBehaviour shopBehaviour;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            shopBehaviour.OnAfterDataChanged += UpdateListData;
            shopBehaviour.ShopData.OnItemBought += UpdateListData;
            shopBehaviour.ShopData.OnItemsGenerated += UpdateListData;
            UpdateListData();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            shopBehaviour.OnAfterDataChanged -= UpdateListData;
            shopBehaviour.ShopData.OnItemBought -= UpdateListData;
            shopBehaviour.ShopData.OnItemsGenerated -= UpdateListData;
        }
        private void UpdateListData(int _) => UpdateListData();
        public override void UpdateListData()
        {
            ItemList.UpdateListDefault(shopBehaviour.ShopData.Items, x => x);
        }
        #endregion methods
    }
}