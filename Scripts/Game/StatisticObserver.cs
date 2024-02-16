using Data.Stored;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Labyrinth;
using Game.Player;
using Game.Rooms;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game
{
    public class StatisticObserver : SingleSceneInstance<StatisticObserver>
    {
        #region fields & properties
        private StatisticData Context => GameData.Data.StatisticData;
        public IReadOnlyList<Memory> StoredMemories => OpenedMemories;
        private List<Memory> OpenedMemories
        {
            get
            {
                openedMemories ??= GetAllMemories();
                return openedMemories;
            }
        }
        private List<Memory> openedMemories = null;

        [SerializeField] private MessageReceiver horizontalMessages;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            StatsBehaviour.Stats.Health.OnValueReachedMinimum += IncreaseDeathCount;

            GameData.Data.TutorialData.OnStageChanged += CheckTutorialStage;

            Context.OpenedMemories.OnItemOpened += StoreNewMemory;
            Context.OpenedMemories.OnItemClosed += OnMemoryClosed;

            foreach (var world in GameData.Data.WorldsData.Stats)
            {
                world.OnCompleted += AddWorldMemory;
            }
            InstancesProvider.Instance.PlayerInventory.OnItemAdded += AddItemMemory;
            GameData.Data.PlayerData.OpenedWeapons.OnItemOpened += AddWeaponMemory;
            GameData.Data.PlayerData.OnModifierRankIncreased += ShowModifierRankText;
            GameData.Data.PlayerData.OnModifierOpened += AddModifierOpenMemory;
            GameData.Data.TasksData.OnTaskCompleted += OnTaskCompleted;
            GameData.Data.PlayerData.Wallet.OnValueChangedAmount += OnGearsChanged;

            Room.OnRoomStart += AddRoomMemory;
            Room.OnRoomStart += CheckRoomStarted;

            _ = OpenedMemories;
        }
        private void OnDisable()
        {
            StatsBehaviour.Stats.Health.OnValueReachedMinimum -= IncreaseDeathCount;

            GameData.Data.TutorialData.OnStageChanged -= CheckTutorialStage;

            Context.OpenedMemories.OnItemOpened -= StoreNewMemory;
            Context.OpenedMemories.OnItemClosed -= OnMemoryClosed;

            foreach (var world in GameData.Data.WorldsData.Stats)
            {
                world.OnCompleted -= AddWorldMemory;
            }
            InstancesProvider.Instance.PlayerInventory.OnItemAdded -= AddItemMemory;
            GameData.Data.PlayerData.OpenedWeapons.OnItemOpened -= AddWeaponMemory;
            GameData.Data.PlayerData.OnModifierRankIncreased -= ShowModifierRankText;
            GameData.Data.PlayerData.OnModifierOpened -= AddModifierOpenMemory;
            GameData.Data.PlayerData.Wallet.OnValueChangedAmount -= OnGearsChanged;

            Room.OnRoomStart -= AddRoomMemory;
            Room.OnRoomStart -= CheckRoomStarted;
        }
        private void OnGearsChanged(int value)
        {
            if (value >= 0) return;
            Context.GearsSpent += Mathf.Abs(value);
        }
        private void CheckRoomStarted(int roomId)
        {
            if (roomId == 84)
            {
                GameData.Data.WorldsData.TryCompleteWorld(WorldType.Hopelessness);
            }
        }
        private void OnTaskCompleted(int taskId)
        {
            switch (taskId)
            {
                case 10: AddEntityMemory(0); break;
                case 21: AddWorldMemory(WorldType.Chaos); break;
                default: break;
            }
        }
        private void AddEntityMemory(int entityId)
        {
            if (!TryAddMemory(entityId, MemoryType.Entity, out Memory memory)) return;

        }
        private void AddModifierOpenMemory(int modifierId)
        {
            if (!TryAddMemory(modifierId, MemoryType.Modifier, out Memory memory)) return;
            //'New Modifier!'
            string text = LanguageLoader.GetTextByType(TextType.Game, 28);
            memory.TryGetName(out string modifierName);
            text += $"\n{modifierName}";
            horizontalMessages.ReceiveMessage(text);
        }
        private void ShowModifierRankText(Modifier modifier)
        {
            if (!DB.Instance.IsMemoryExists(modifier.Id, MemoryType.Modifier, out Memory memory)) return;
            //'New Modifier Rank!'
            string text = LanguageLoader.GetTextByType(TextType.Game, 29);
            memory.TryGetName(out string modifierName);
            text += $"\n{modifierName}: {modifier.Rank -1} => {modifier.Rank}";
            horizontalMessages.ReceiveMessage(text);
        }
        private void AddItemMemory(int itemId)
        {
            if (!TryAddMemory(itemId, MemoryType.Item, out _)) return;
            //'New Item Memory!'
            string text = LanguageLoader.GetTextByType(TextType.Game, 26);
            text += $"\n{DB.Instance.Items.GetObjectById(itemId).Data.Name.Text}";
            horizontalMessages.ReceiveMessage(text);
        }
        private void AddWeaponMemory(int weaponId)
        {
            if (!TryAddMemory(weaponId, MemoryType.Weapon, out _)) return;
            //'New Weapon!'
            string text = LanguageLoader.GetTextByType(TextType.Game, 27);
            text += $"\n{DB.Instance.Weapons.GetObjectById(weaponId).Data.Name.Text}";
            horizontalMessages.ReceiveMessage(text);
        }
        private void AddWorldMemory(WorldType worldType)
        {
            TryAddMemory((int)worldType, MemoryType.World, out _);
        }
        private void AddRoomMemory(int roomId)
        {
            TryAddMemory(roomId, MemoryType.Room, out _);
        }
        private void CheckTutorialStage(TutorialStage stage)
        {
            if ((int)stage < (int)TutorialStage.Info) return;
            for (int i = 0; i <= 11; ++i)
                AddMemory(i);
        }

        private void IncreaseDeathCount()
        {
            Context.DeathCount += 1;
        }

        private bool TryAddMemory(int referenceId, MemoryType memoryType, out Memory newMemory)
        {
            newMemory = null;
            if (!DB.Instance.IsMemoryExists(referenceId, memoryType, out Memory memory)) return false;
            if (!GameData.Data.StatisticData.OpenedMemories.TryOpenItem(memory.Id)) return false;
            newMemory = memory;
            return true;
        }
        private void AddMemory(int id) => GameData.Data.StatisticData.OpenedMemories.TryOpenItem(id);
        private void OnMemoryClosed(int _)
        {
            Debug.LogError("Are you sure you want to remove memory?", this);
            openedMemories = GetAllMemories();
        }
        private List<Memory> GetAllMemories()
        {
            List<Memory> list = new();
            IEnumerable<int> openedMemoriesId = Context.OpenedMemories.ItemsId;
            foreach (var el in openedMemoriesId)
            {
                list.Add(GetMemory(el));
            }
            return list;
        }
        private Memory GetMemory(int memoryId) => DB.Instance.Memories.GetObjectById(memoryId).Data;
        private void StoreNewMemory(int memoryId)
        {
            OpenedMemories.Add(GetMemory(memoryId));
        }

        #endregion methods
    }
}