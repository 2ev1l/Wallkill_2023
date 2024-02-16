using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Data.Stored
{
    [System.Serializable]
    public class ShopData
    {
        #region fields & properties
        public UnityAction OnBeforeGenerate;
        public UnityAction OnItemsGenerated;
        public UnityAction<int> OnItemBought;
        public IReadOnlyList<int> Items => items;
        [SerializeField] private List<int> items = new(); //this will be better than list with id & count because it's anyway will be decoded into sequence but with performance leaks
        public int LastTimeGenerated => lastTimeGenerated;
        [SerializeField] private int lastTimeGenerated = int.MinValue;
        #endregion fields & properties

        #region methods
        public void BuyItem(int itemId)
        {
            items.Remove(itemId);
            DB.Instance.ShopItems.GetObjectById(itemId).Data.SimulateBuy();
            OnItemBought?.Invoke(itemId);
        }
        public void GenerateData()
        {
            OnBeforeGenerate?.Invoke();
            items = new();
            foreach (var el in DB.Instance.ShopItems.Data)
            {
                ShopItem item = el.Data;
                if (!item.TrySpawn(out int spawnedCount)) continue;
                for (int i = 0; i < spawnedCount; ++i)
                {
                    items.Add(item.Id);
                }
            }
            lastTimeGenerated = GameData.Data.StatisticData.TimePlayed;
            OnItemsGenerated?.Invoke();
        }
        #endregion methods
    }
}