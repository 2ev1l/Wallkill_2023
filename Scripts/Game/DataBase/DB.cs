using Data.Interfaces;
using UnityEngine;
using Data.Stored;
using EditorCustom.Attributes;
using Universal;
using System.Linq;
using Game.Player;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Game.DataBase
{
    [ExecuteAlways]
    public class DB : MonoBehaviour, IInitializable
    {
        #region fields & properties
        public static DB Instance { get; private set; }
        public DBSOSet<WorldInfoSO> WorldsInfo => worldsInfo;
        [SerializeField] private DBSOSet<WorldInfoSO> worldsInfo;
        public DBSOSet<WeaponSO> Weapons => weapons;
        [SerializeField] private DBSOSet<WeaponSO> weapons;
        public DBSOSet<HitImpactSO> HitImpacts => hitImpacts;
        [SerializeField] private DBSOSet<HitImpactSO> hitImpacts;
        public DBSOSet<ItemSO> Items => items;
        [SerializeField] private DBSOSet<ItemSO> items;
        public DBSOSet<TaskSO> Tasks => tasks;
        [SerializeField] private DBSOSet<TaskSO> tasks;
        public DBSOSet<MemorySO> Memories => memories;
        [SerializeField] private DBSOSet<MemorySO> memories;
        public DBSOSet<ShopItemSO> ShopItems => shopItems;
        [SerializeField] private DBSOSet<ShopItemSO> shopItems;

        [Title("Read Only")]
        [SerializeField] private GameData gameData;
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
            gameData = GameData.Data;
        }
        public bool IsMemoryExists(int referenceId, MemoryType memoryType, out Memory memoryFound)
        {
            memoryFound = null;
            MemorySO found = memories.Find(x => x.Data.ReferenceId == referenceId && x.Data.MemoryType == memoryType);
            if (found == null) return false;

            memoryFound = found.Data;
            return true;
        }
        #endregion methods

#if UNITY_EDITOR
        [SerializeField] private bool automaticallyUpdateEditor = true;
        public void Start()
        {
            if (!automaticallyUpdateEditor) return;
            GetAllDBInfo();
            CheckAllErrors();
        }
        /// <summary>
        /// You need to manually add code
        /// </summary>
        [ContextMenu(nameof(GetAllDBInfo))]
        private void GetAllDBInfo()
        {
            if (Application.isPlaying) return;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            worldsInfo.CollectAll();
            weapons.CollectAll();
            hitImpacts.CollectAll();
            items.CollectAll();
            tasks.CollectAll();
            memories.CollectAll();
            shopItems.CollectAll();

            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// You need to manually add code
        /// </summary>
        [Button(nameof(CheckAllErrors))]
        private void CheckAllErrors()
        {
            if (!Application.isPlaying) return;
            worldsInfo.CatchExceptions(x => _ = x.Data.Language.Text, "wrong text id");
            worldsInfo.CatchExceptions(x => _ = x.Data.Rooms.Count(r => x.Data.Rooms.Count(rcheck => rcheck.Id == r.Id) > 1) != 0 ? throw new System.Exception() : 0, "check rooms id");

            weapons.CatchExceptions(x => _ = x.Data.Name.Text, "wrong text id");
            weapons.CatchExceptions(x => _ = x.Data.Id != x.ObjectId ? throw new System.Exception() : 0, "wrong data & object id");
            weapons.CatchExceptions(x => _ = x.Data.Model.Prefab == null ? throw new System.Exception() : 0, "null model prefab");
            weapons.CatchExceptions(x => _ = x.Data.BulletsInfo.Bullets.OriginalPrefab == null ? throw new System.Exception() : 0, "null bullets prefab");

            hitImpacts.CatchExceptions(x => _ = x.Data.DecalTexture == null ? throw new System.Exception() : 0, "null decal tex");
            hitImpacts.CatchExceptions(x => _ = x.Data.DecalNormalTexture == null ? throw new System.Exception() : 0, "null decal normal");

            items.CatchExceptions(x => _ = x.Data.Id != x.ObjectId ? throw new System.Exception() : 0, "wrong data & object id");
            items.CatchExceptions(x => _ = x.Data.Sprite512x == null ? throw new System.Exception() : 0, "null sprite");
            items.CatchExceptions(x => _ = x.Data.Material == null ? throw new System.Exception() : 0, "null mat");
            items.CatchExceptions(x => _ = x.Data.Name.Text, "wrong text id");

            tasks.CatchExceptions(x => _ = x.Data.Id != x.ObjectId ? throw new System.Exception() : 0, "wrong data & object id");
            tasks.CatchExceptions(x => _ = x.Data.Name.Text, "wrong name text id");
            tasks.CatchExceptions(x => _ = x.Data.Description.Text, "wrong description text id");
            tasks.CatchExceptions(x => _ = x.Data.DoReward && x.Data.Rewards.Count == 0 ? throw new System.Exception() : 0, "wrong rewards count");
            tasks.CatchExceptions(x => _ = !x.Data.DoReward && x.Data.Rewards.Count > 0 ? throw new System.Exception() : 0, "check doReward bool (?)");
            tasks.CatchExceptions(x => _ = x.Data.Rewards.Count(r => x.Data.Rewards.Count(rcheck => rcheck.ReferenceId == r.ReferenceId && rcheck.RewardType == r.RewardType) > 1) != 0 ? throw new System.Exception() : 0, "check reward");
            tasks.CatchExceptions(x => _ = Reward.GetRewardsText(x.Data.Rewards), "wrong rewards text");
            tasks.CatchExceptions(x => x.Data.Rewards.ToList().ForEach(x => _ = x.Count == 0 ? throw new System.Exception() : 0), "wrong sub-reward count");

            memories.CatchExceptions(x => _ = x.Data.Id != x.ObjectId ? throw new System.Exception() : 0, "wrong data & object id");
            memories.CatchExceptions(x => _ = x.Data.ReferenceId < 0 ? throw new System.Exception() : 0, "wrong reference id");
            memories.CatchExceptions(x => _ = x.Data.Sprite == null && x.Data.OverrideSprite ? throw new System.Exception() : 0, "wrong sprite");
            memories.CatchExceptions(x => _ = x.Data.Name.Text, "wrong name text id");
            memories.CatchExceptions(x => _ = x.Data.Description.Text, "wrong description text id");
            memories.CatchExceptions(x => _ = x.Data.TryGetName(out string result), "wrong name method");
            memories.CatchExceptions(x => _ = x.Data.TryGetDescription(out string result), "wrong description method");
            memories.CatchExceptions(x => _ = x.Data.TryGetSprite(out Sprite sprite), "wrong sprite method");

            shopItems.CatchExceptions(x => _ = x.Data.Id != x.ObjectId ? throw new System.Exception() : 0, "wrong data & object id");
            shopItems.CatchExceptions(x => x.Data.GetInfo(out _, out _, out _), "wrong info method");
            shopItems.CatchExceptions(x => _ = x.Data.Price.Value < 1 ? throw new System.Exception() : 0, "wrong price");
            shopItems.CatchExceptions(x => _ = x.Data.CanSpawn(), "wrong spawn condition");
            shopItems.CatchExceptions(x => _ = shopItems.Data.Count(found => found.Data.ItemType == x.Data.ItemType && found.Data.ReferenceId == x.Data.ReferenceId) > 1 ? throw new System.Exception() : 0, "wrong reference parameters (same data)");
            shopItems.CatchExceptions(x => _ = (x.Data.TrySpawn(out int spawnedCount) && (spawnedCount > x.Data.SpawnCount.y || spawnedCount < x.Data.SpawnCount.x)) ? throw new System.Exception() : 0, "wrong generation data");
        }
#endif //UNITY_EDITOR
    }
}