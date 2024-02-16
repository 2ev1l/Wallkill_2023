using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class ShopItemChoosable : TextItemChoosable<ShopItemChoosable>
    {
        #region fields & properties
        public ShopItem Item => item;
        private ShopItem item = null;
        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            item = DB.Instance.ShopItems.GetObjectById(param).Data;
            base.OnListUpdate(param);
        }
        protected override string GetDescription()
        {
            item.GetInfo(out _, out string description, out _);
            return description;
        }

        protected override string GetName()
        {
            item.GetInfo(out string name, out _, out _);
            return name;
        }

        protected override void OnItemChoosed()
        {
            ShopUI.Instance.ChooseItem(this);
        }
        #endregion methods
    }
}