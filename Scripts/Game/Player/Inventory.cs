using Data.Stored;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Labyrinth;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Game.Player
{
    public class Inventory : MonoBehaviour
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - itemId
        /// </summary>
        public UnityAction<int> OnItemAdded;
        /// <summary>
        /// <see cref="{T0}"/> - itemId
        /// </summary>
        public UnityAction<int> OnItemCountIncreased;
        /// <summary>
        /// <see cref="{T0}"/> - itemId
        /// </summary>
        public UnityAction<int> OnItemRemoved;
        /// <summary>
        /// <see cref="{T0}"/> - itemId
        /// </summary>
        public UnityAction<int> OnItemUsed;
        public UnityAction OnItemsClear;

        [SerializeField] private ItemsUse itemsUse;
        public IReadOnlyList<PageItem> Items => items.Items;
        [Title("Read Only")][SerializeField][ReadOnly] private OpenedItemsStored<PageItem> items;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            GameData.Data.WorldsData.OnCurrentWorldChanged += CheckNewWorld;
            CheckNewWorld(GameData.Data.WorldsData.CurrentWorld);
        }
        private void OnDisable()
        {
            GameData.Data.WorldsData.OnCurrentWorldChanged -= CheckNewWorld;
        }
        public bool ContainItem(System.Predicate<PageItem> match) => items.IsOpened(match);
        public void AddItem(int id) => AddItem(id, 1);
        public void AddItem(int id, int count)
        {
            if (count <= 0) return;

            Item item = DB.Instance.Items.GetObjectById(id).Data;
            if (item.IsSkill) GameData.Data.PlayerData.OpenedItemsSkills.TryOpenItem(id);
            PageItem pageItem = new(id, count, item.ActivationDelay.Clone());
            if (items.TryOpenItem(pageItem, x => x.Id == id, out PageItem existItem))
            {
                OnItemAdded?.Invoke(id);
            }
            else
            {
                existItem.Count += count;
                OnItemCountIncreased?.Invoke(id);
            }
        }
        public bool TryRemoveItem(int id)
        {
            if (!items.TryCloseItem(x => x.Id == id)) return false;
            OnItemRemoved?.Invoke(id);
            return true;
        }
        public bool TryUseItem(int id, int count)
        {
            if (!items.IsOpened(x => x.Id == id, out PageItem pageItem)) return false;
            if (!itemsUse.CanUseItem(id)) return false;
            if (!pageItem.ActivationDelay.CanActivate) return false;

            if (count == 0)
            {
                pageItem.ActivationDelay.Activate();
                itemsUse.UseItem(id);
                OnItemUsed?.Invoke(id);
                return true;
            }
            if (pageItem.Count < count) return false;

            pageItem.Count -= count;
            pageItem.ActivationDelay.Activate();
            itemsUse.UseItem(id);
            OnItemUsed?.Invoke(id);

            if (pageItem.Count <= 0)
            {
                TryRemoveItem(id);
            }
            return true;
        }

        private void CheckNewWorld(WorldType type)
        {
            if (type != WorldType.Portal) return;
            if (WorldLoader.Instance.IsLoadingFromPortalObject) return;
            ResetAllItems();
        }
        private void ResetAllItems()
        {
            items.CloseAllItems();
            foreach (int skillId in GameData.Data.PlayerData.OpenedItemsSkills.ItemsId)
            {
                AddItem(skillId);
            }
            OnItemsClear?.Invoke();
        }
        #endregion methods

        [System.Serializable]
        public class PageItem
        {
            /// <summary>
            /// <see cref="{T0}"/> - newCount
            /// </summary>
            public UnityAction<int> OnCountChanged;
            public int Id => id;
            [SerializeField] private int id = 0;
            public int Count
            {
                get => count;
                set
                {
                    count = Mathf.Max(value, 0);
                    OnCountChanged?.Invoke(count);
                }
            }
            [SerializeField] private int count = 0;
            public TimeDelay ActivationDelay => activationDelay;
            [SerializeField] private TimeDelay activationDelay;

            public PageItem(int id, int count, TimeDelay delay)
            {
                this.id = id;
                this.count = count;
                this.activationDelay = delay;
            }
        }

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField] private int itemToTest = 0;
        [Button(nameof(AddItemTest))]
        private void AddItemTest() => AddItem(itemToTest, 1);
#endif //UNITY_EDITOR
    }
}