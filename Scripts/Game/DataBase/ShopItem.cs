using Data.Stored;
using EditorCustom.Attributes;
using Game.Player;
using Game.Rooms;
using Game.Rooms.Mechanics;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.DataBase
{
    [System.Serializable]
    public class ShopItem
    {
        #region fields & properties
        public int Id => id;
        [SerializeField][Min(0)] private int id;

        public int ReferenceId => referenceId;
        [Title("Reference")][SerializeField][Min(0)] private int referenceId = 0;
        public ExistItemType ItemType => itemType;
        [SerializeField] private ExistItemType itemType = ExistItemType.Item;
        public Wallet Price => price;
        [SerializeField] private Wallet price = new(1);

        [Title("Spawn")]
        [SerializeField] private bool requiredTaskCompletion = false;
        [SerializeField][DrawIf(nameof(requiredTaskCompletion), true)][Min(0)] private int requiredTaskId = 0;
        [SerializeField][Range(0, 100)] private int spawnProbability = 100;
        public Vector2Int SpawnCount => spawnCount;
        [SerializeField][MinMaxSlider(1, 5)] private Vector2Int spawnCount = Vector2Int.one;

        [Title("Override")]
        [SerializeField] private bool overrideName = false;
        [SerializeField] private bool overrideDescription = false;
        [SerializeField] private bool overrideSprite = false;
        [SerializeField][DrawIf(nameof(overrideName), true)] private LanguageInfo nameOverride = new();
        [SerializeField][DrawIf(nameof(overrideDescription), true)] private LanguageInfo descriptionOverride = new();
        [SerializeField][DrawIf(nameof(overrideSprite), true)] private Sprite spriteOverride = null; //820x512
        #endregion fields & properties

        #region methods
        /// <summary>
        /// You need to check Wallet for possibility to buy manually. See also: <see cref="ShopData.BuyItem(int)"/>
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SimulateBuy()
        {
            switch (ItemType)
            {
                case ExistItemType.Item: InstancesProvider.Instance.PlayerInventory.AddItem(ReferenceId); break;
                case ExistItemType.Weapon: GameData.Data.PlayerData.OpenedWeapons.TryOpenItem(ReferenceId); break;
                case ExistItemType.Modifier:
                    if (!GameData.Data.PlayerData.TryOpenModifier(ReferenceId, out Modifier exists))
                    {
                        GameData.Data.PlayerData.TryIncreaseModifierRank(exists.Id); break;
                    }
                    break;
                default: throw new System.NotImplementedException(itemType.ToString());
            }
            GameData.Data.PlayerData.Wallet.TrySpent(Price.Value);
        }
        public bool TrySpawn(out int spawnedCount)
        {
            spawnedCount = 0;
            if (!CanSpawn()) return false;
            spawnedCount = Random.Range(spawnCount.x, spawnCount.y + 1);
            return true;
        }

        public bool CanSpawn()
        {
            if (requiredTaskCompletion && !GameData.Data.TasksData.IsTaskCompleted(requiredTaskId)) return false;
            switch (ItemType)
            {
                case ExistItemType.Item:
                    if (GameData.Data.PlayerData.OpenedItemsSkills.IsOpened(ReferenceId))
                        return false;
                    break;
                case ExistItemType.Weapon:
                    if (GameData.Data.PlayerData.OpenedWeapons.IsOpened(ReferenceId))
                        return false;
                    break;
                case ExistItemType.Modifier:
                    if (GameData.Data.PlayerData.OpenedModifiers.Exists(x => x.Id == ReferenceId, out Modifier exist))
                    {
                        if (!exist.CanIncreaseRank()) 
                            return false;
                    }
                    break;
            }
            if (!CustomMath.GetRandomChance(spawnProbability)) return false;
            return true;
        }
        public void GetInfo(out string name, out string description, out Sprite sprite)
        {
            MemoryType memoryType = ItemType switch
            {
                ExistItemType.Item => MemoryType.Item,
                ExistItemType.Weapon => MemoryType.Weapon,
                ExistItemType.Modifier => MemoryType.Modifier,
                _ => throw new System.NotImplementedException(itemType.ToString()),
            };
            DB.Instance.IsMemoryExists(ReferenceId, memoryType, out Memory memory); //this will be checked automatically in DB

            if (overrideName) name = nameOverride.Text;
            else memory.TryGetName(out name);

            if (overrideDescription) description = descriptionOverride.Text;
            else memory.TryGetDescription(out description);

            if (overrideSprite) sprite = spriteOverride;
            else memory.TryGetSprite(out sprite);
        }
        #endregion methods
    }
}