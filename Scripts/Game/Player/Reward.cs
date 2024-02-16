using Data.Stored;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI;

namespace Game.Player
{
    [System.Serializable]
    public class Reward
    {
        #region fields & properties
        public RewardType RewardType => rewardType;
        [SerializeField] private RewardType rewardType;
        public int ReferenceId => referenceId;
        [SerializeField][Min(0)] private int referenceId;
        public int Count => count;
        [SerializeField][Min(1)] private int count = 1;
        [SerializeField][DrawIf(nameof(rewardType), RewardType.Modifier)] private bool increaseModifierRank = true;
        #endregion fields & properties

        #region methods
        public void AddReward() => AddReward(count);
        public void AddReward(int count)
        {
            if (count <= 0) return;
            switch (rewardType)
            {
                case RewardType.Gears: GameData.Data.PlayerData.Wallet.AddValue(count); break;
                case RewardType.Weapon: GameData.Data.PlayerData.OpenedWeapons.TryOpenItem(referenceId); break;
                case RewardType.Item: InstancesProvider.Instance.PlayerInventory.AddItem(referenceId, count); break;
                case RewardType.Modifier:
                    if (!GameData.Data.PlayerData.TryOpenModifier(referenceId, out Modifier existModifier))
                    {
                        if (increaseModifierRank)
                            GameData.Data.PlayerData.TryIncreaseModifierRank(existModifier);
                    }
                    break;
                default: throw new System.NotImplementedException("Reward Type: " + rewardType);
            }
        }
        public static string GetRewardsText(IEnumerable<Reward> rewards)
        {
            string finalText = LanguageLoader.GetTextByType(TextType.Tasks, 11);
            foreach(var reward in rewards)
            {
                finalText += $"\n{GetRewardText(reward)}";
            }
            return finalText;
        }
        private static string GetRewardText(Reward reward)
        {
            switch (reward.RewardType)
            {
                case RewardType.Gears: return $"{LanguageLoader.GetTextByType(TextType.Tasks, 12)}: {reward.Count}";
                case RewardType.Weapon: return $"{LanguageLoader.GetTextByType(TextType.Help, 41)}: {DB.Instance.Weapons.GetObjectById(reward.ReferenceId).Data.Name.Text}";
                case RewardType.Item:
                    Item item = DB.Instance.Items.GetObjectById(reward.ReferenceId).Data;
                    string itemTypeText = LanguageLoader.GetTextByType(TextType.Help, item.IsSkill ? 50 : 42);
                    return $"{itemTypeText}: {item.Name.Text}";
                case RewardType.Modifier:
                    string modifierText = LanguageLoader.GetTextByType(TextType.Help, 43);
                    string referenceText = "";
                    if (DB.Instance.IsMemoryExists(reward.ReferenceId, MemoryType.Modifier, out Memory memory))
                    {
                        memory.TryGetName(out referenceText);
                    }
                    else
                    {
                        referenceText = "???";
                    }
                    return $"{modifierText}: {referenceText}";
                default: throw new System.NotImplementedException("Reward Type: " + reward.RewardType);
            }
        }
        #endregion methods
    }
}