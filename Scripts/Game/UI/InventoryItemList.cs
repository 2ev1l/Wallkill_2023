using Data.Settings;
using Game.Player;
using Game.Rooms;
using Menu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game.UI
{
    public class InventoryItemList : MonoBehaviour
    {
        #region fields & properties
        public ItemList<InventoryItem, Player.Inventory.PageItem> ItemsList => itemsList;
        [SerializeField] private ItemList<InventoryItem, Player.Inventory.PageItem> itemsList;
        public IReadOnlyList<InventoryItem> CurrentPageItems => itemsList.CurrentPageItems;
        private List<KeyCodeInfo> InventoryKeys
        {
            get
            {
                inventoryKeys ??= SettingsData.Data.KeyCodeSettings.GetInventoryKeys();
                return inventoryKeys;
            }
        }
        private List<KeyCodeInfo> inventoryKeys = null;
        private bool isActivationKeysInitialized = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            InputController.OnKeyDown += CheckInventoryPressedKey;
            InstancesProvider.Instance.PlayerInventory.OnItemAdded += UpdateListData;
            InstancesProvider.Instance.PlayerInventory.OnItemRemoved += UpdateListData;
            InstancesProvider.Instance.PlayerInventory.OnItemsClear += UpdateListData;
            InitActivationKeys();
            UpdateListData();
        }
        private void OnDisable()
        {
            InputController.OnKeyDown -= CheckInventoryPressedKey;
            InstancesProvider.Instance.PlayerInventory.OnItemAdded -= UpdateListData;
            InstancesProvider.Instance.PlayerInventory.OnItemRemoved -= UpdateListData;
            InstancesProvider.Instance.PlayerInventory.OnItemsClear -= UpdateListData;
        }
        private void InitActivationKeys()
        {
            if (isActivationKeysInitialized) return;
            isActivationKeysInitialized = true;
            List<Inventory.PageItem> nullItems = new()
            {
                new(0, 0, new()), new(0, 0, new()), new(0, 0, new()),
                new(0, 0, new()), new(0, 0, new()), new(0, 0, new()),
                new(0, 0, new()), new(0, 0, new()), new(0, 0, new()),
            };
            itemsList.UpdateListDefault(nullItems, x => x);

            int count = 0;
            foreach (var el in CurrentPageItems)
            {
                el.SetActivationCode((KeyCodeDescription)((int)(KeyCodeDescription.Inventory1) + count));
                count++;
            }
        }
        public void SwitchPage(bool isNext) => itemsList.SwitchPage(isNext);
        private void UpdateListData(int _) => UpdateListData();
        private void UpdateListData()
        {
            itemsList.UpdateListDefault(InstancesProvider.Instance.PlayerInventory.Items, x => x);
        }
        private void CheckInventoryPressedKey(KeyCode keyCode)
        {
            if (InstancesProvider.Instance.PlayerInput.IsInputStopped()) return;
            KeyCodeInfo keyCodeInfo = InventoryKeys.Find(x => x.Key == keyCode);
            if (keyCodeInfo == null) return;
            
            foreach (var el in CurrentPageItems)
            {
                if (el.ActivateCodeDescription != keyCodeInfo.Description) continue;
                if (el.ItemInfo == null) return;
                bool isUsed = InstancesProvider.Instance.PlayerInventory.TryUseItem(el.ItemInfo.Id, el.ItemInfo.CountToUse);
                if (!isUsed)
                    el.OnBlockedToUse();
            }
        }
        #endregion methods
    }
}