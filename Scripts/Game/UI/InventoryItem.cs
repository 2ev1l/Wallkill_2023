using Data.Settings;
using Game.DataBase;
using Game.Player;
using UnityEngine;
using UnityEngine.Events;
using Universal.UI;

namespace Game.UI
{
    public class InventoryItem : MonoBehaviour, IListUpdater<Inventory.PageItem>
    {
        #region fields & properties
        public UnityAction OnItemBlockedToUse;
        public UnityAction OnBeforeItemSet;
        public UnityAction OnItemSet;
        public Inventory.PageItem PageItem => pageItem;
        private Inventory.PageItem pageItem;
        /// <summary>
        /// Do not modify this property
        /// </summary>
        public Item ItemInfo => itemInfo;
        private Item itemInfo;
        public KeyCodeDescription ActivateCodeDescription => activateCodeDescription;
        [SerializeField] private KeyCodeDescription activateCodeDescription;
        #endregion fields & properties

        #region methods
        public void SetActivationCode(KeyCodeDescription description)
        {
            activateCodeDescription = description;
        }
        public void OnListUpdate(Inventory.PageItem param)
        {
            OnBeforeItemSet?.Invoke();
            this.pageItem = param;
            this.itemInfo = DB.Instance.Items.GetObjectById(pageItem.Id).Data;
            OnItemSet?.Invoke();
        }
        public void OnBlockedToUse() => OnItemBlockedToUse?.Invoke();
        #endregion methods
    }
}