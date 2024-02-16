using Data.Stored;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public class MemoriesItemList : ItemListBase<MemoryItem, int>
    {
        #region fields & properties
        [SerializeField] private MemoryType memoryType;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            GameData.Data.StatisticData.OpenedMemories.OnItemOpened += UpdateListData;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            GameData.Data.StatisticData.OpenedMemories.OnItemOpened -= UpdateListData;
            base.OnDisable();
        }
        private void UpdateListData(int _) => UpdateListData();
        public override void UpdateListData()
        {
            ItemList.UpdateListDefault(StatisticObserver.Instance.StoredMemories.Where(x => x.MemoryType == memoryType), x => x.Id);
        }
        #endregion methods
    }
}