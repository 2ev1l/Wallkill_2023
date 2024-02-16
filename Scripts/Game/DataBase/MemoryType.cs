using Data.Settings;
using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game.DataBase
{
    #region enum
    public enum MemoryType
    {
        Weapon,
        Item,
        Modifier,
        Entity,
        Room,
        World,
        Tutorial
    }
    #endregion enum

    public static class MemoryTypeExtension
    {
        #region methods
        public static void GetDataByType(this MemoryType memoryType, int referenceId, out string name, out string description, out Sprite sprite)
        {
            switch (memoryType)
            {
                case MemoryType.Weapon: GetWeaponData(referenceId, out name, out description, out sprite); break;
                case MemoryType.Item: GetItemData(referenceId, out name, out description, out sprite); break;
                case MemoryType.Modifier: GetModifierData(referenceId, out name, out description, out sprite); break;
                case MemoryType.Entity: GetEntityData(referenceId, out name, out description, out sprite); break;
                case MemoryType.Room: GetRoomData(referenceId, out name, out description, out sprite); break;
                case MemoryType.World: GetWorldData(referenceId, out name, out description, out sprite); break;
                case MemoryType.Tutorial: GetTutorialData(referenceId, out name, out description, out sprite); break;
                default: throw new System.NotImplementedException("Memory type");
            }
        }
        private static void GetWeaponData(int referenceId, out string name, out string description, out Sprite sprite)
        {
            sprite = null;
            description = "";

            Weapon info = DB.Instance.Weapons.GetObjectById(referenceId).Data;
            name = info.Name.Text;

            switch (referenceId)
            {
                case 0: description = $"{GetMemoriesText(23)}"; break;
                case 1: description = $"{GetMemoriesText(23)}"; break;
                case 2: description = $"{GetMemoriesText(28)}"; break;
                case 3: description = $"{GetMemoriesText(42)}"; break;
                case 4: description = $"{GetMemoriesText(52)}"; break;
            }
            description += GetWeaponDefaultDescription(info);
        }
        private static string GetWeaponDefaultDescription(Weapon weapon)
        {
            string result = "";
            result += $"\n{GetMemoriesText(13)}: {weapon.Damage}";
            result += $"\n{GetMemoriesText(14)}: {GetStringOrInfinity(weapon.BulletsInfo.BulletsMax)} / {GetStringOrInfinity(weapon.BulletsInfo.BulletsAtClip)}";
            result += $"\n{GetMemoriesText(15)}: {weapon.ReloadDelay.Delay:F2} s. / {weapon.ShootDelay.Delay:F2} s.";
            return result;
        }
        private static string GetStringOrInfinity(int number) => number == -1 ? "~" : number.ToString();
        private static void GetItemData(int referenceId, out string name, out string description, out Sprite sprite)
        {
            description = null;

            Item info = DB.Instance.Items.GetObjectById(referenceId).Data;
            name = info.Name.Text;
            sprite = info.Sprite512x;
            switch (referenceId)
            {
                case 0: break;
            }
        }
        private static void GetModifierData(int referenceId, out string name, out string description, out Sprite sprite)
        {
            name = null;
            description = null;
            sprite = null;
            int rank = -1;
            int maxRank = -1;
            if (GameData.Data.PlayerData.OpenedModifiers.Exists(x => x.Id == referenceId, out Modifier exist))
            {
                rank = exist.Rank;
                maxRank = exist.GetMaxRank();
            }
            string GetReplaceableRank(int memoriesTextId) => GetModifierReplaceableRank(memoriesTextId, referenceId, rank);

            switch (referenceId)
            {
                case 0: description = $"{GetReplaceableRank(29)}"; break;
                case 1: description = $"{GetModifierUsableText()} {GetMemoriesText(30)}"; break;
                case 2: description = $"{GetReplaceableRank(34)}"; break;
                case 3: description = $"{GetReplaceableRank(35)}"; break;
                case 4: description = $"{GetModifierUsableText()} {GetMemoriesText(36)}"; break;
                case 5: description = $"{GetMemoriesText(49)}"; break;
                case 6: description = $"{GetModifierUsableText()} {GetMemoriesText(50)}"; break;
            }
            description += $" {GetModifierRankText(rank, maxRank)}";
        }
        private static string GetModifierRankText(int rank, int maxRank) => $"({GetMemoriesText(33)}: {GetStringOrInfinity(rank)}/{GetStringOrInfinity(maxRank)})";
        private static string GetModifierReplaceableRank(int memoriesTextId, int referenceId, int rank)
        {
            string replaced = referenceId switch
            {
                0 => rank switch //ricochets
                {
                    0 => "2",
                    1 => "3",
                    2 => "4",
                    _ => "??"
                },
                2 => rank switch //revert bullets
                {
                    0 => "10%",
                    1 => "20%",
                    _ => "??"
                },
                3 => rank switch //gravity sphere scale
                {
                    0 => "+50%",
                    1 => "+100%",
                    _ => "??"
                },
                _ => "?"
            };
            string memoriesText = GetMemoriesText(memoriesTextId);
            memoriesText = memoriesText.Replace("[-1-]", $"{replaced}");
            return memoriesText;
        }
        private static string GetModifierUsableText() => GetMemoriesText(27).Replace($"[X]", $"[{LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Modifier.Key)}]");
        private static void GetEntityData(int referenceId, out string name, out string description, out Sprite sprite)
        {
            name = null;
            description = null;
            sprite = null;
        }
        private static void GetRoomData(int referenceId, out string name, out string description, out Sprite sprite)
        {
            name = null;
            description = null;
            sprite = null;
        }
        private static void GetWorldData(int referenceId, out string name, out string description, out Sprite sprite)
        {
            description = null;

            WorldInfo info = DB.Instance.WorldsInfo.GetObjectById(referenceId).Data;
            name = info.Language.Text;
            sprite = info.Texture;
            switch (referenceId)
            {
                case 0: break;
            }
        }
        private static void GetTutorialData(int referenceId, out string name, out string description, out Sprite sprite)
        {
            name = null;
            description = null;
            sprite = null;

            switch (referenceId)
            {
                case 0:
                    description = GetMemoriesText(0);
                    string wasd = LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.MoveForward.Key);
                    wasd += "/" + LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.MoveBackward.Key);
                    wasd += "/" + LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.MoveRight.Key);
                    wasd += "/" + LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.MoveLeft.Key);
                    description = description.Replace("[WASD]", wasd);
                    break;
                case 1:
                    description = GetMemoriesText(1);
                    description = description.Replace("[ROT]", LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CameraRotation.Key));
                    description = description.Replace("[CROP]", LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CameraCrop.Key));
                    break;
                case 2:
                    description = GetMemoriesText(2);
                    description = description.Replace("[AIM]", LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Aim.Key));
                    description = description.Replace("[FIRE]", LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Fire.Key));
                    break;
                case 3:
                    description = GetMemoriesText(3);
                    description = description.Replace("[RUN]", LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Run.Key));
                    description = description.Replace("[JUMP]", LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Jump.Key));
                    description = description.Replace("[CROUCH]", LanguageLoader.GetTextByKeyCode(SettingsData.Data.KeyCodeSettings.CharacterSettings.Crouch.Key));
                    break;

            }
        }
        private static string GetMemoriesText(int id) => LanguageLoader.GetTextByType(TextType.Memories, id);
        #endregion methods
    }
}