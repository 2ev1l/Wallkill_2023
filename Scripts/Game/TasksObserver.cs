using Data.Stored;
using DebugStuff;
using EditorCustom.Attributes;
using Game.Labyrinth;
using Game.Player;
using Game.Rooms;
using Game.Tutorial;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game
{
    public class TasksObserver : SingleSceneInstance<TasksObserver>
    {
        #region fields & properties
        [SerializeField] private HelpMessages helpMessages;
        [SerializeField] private PortalOverlay portalOverlay;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            WorldLoader.Instance.OnWorldLoaded += CheckWorldLoaded;
            GameData.Data.TasksData.OnTaskCompleted += OnTaskCompleted;
            GameData.Data.TasksData.OnTaskStarted += OnTaskStarted;
            Room.OnRoomStart += CheckRoomStarted;
            Room.OnRoomCompleted += CheckRoomCompleted;
            GameData.Data.PlayerData.OpenedWeapons.OnItemOpened += CheckWeaponOpened;
            portalOverlay.OnPortalUsed += CheckPortalUsed;
            GameData.Data.PlayerData.OnModifierOpened += CheckModifierOpened;
            GameData.Data.ShopData.OnItemBought += CheckItemBought;
            InstancesProvider.Instance.PlayerInventory.OnItemAdded += CheckPlayerItems;
            InstancesProvider.Instance.PlayerInventory.OnItemCountIncreased += CheckPlayerItems;

            CheckWorldLoaded(WorldType.Portal);
            CheckEnableActions();
        }
        private void OnDisable()
        {
            WorldLoader.Instance.OnWorldLoaded -= CheckWorldLoaded;
            GameData.Data.TasksData.OnTaskCompleted -= OnTaskCompleted;
            GameData.Data.TasksData.OnTaskStarted -= OnTaskStarted;
            Room.OnRoomStart -= CheckRoomStarted;
            Room.OnRoomCompleted -= CheckRoomCompleted;
            GameData.Data.PlayerData.OpenedWeapons.OnItemOpened -= CheckWeaponOpened;
            portalOverlay.OnPortalUsed -= CheckPortalUsed;
            GameData.Data.PlayerData.OnModifierOpened -= CheckModifierOpened;
            GameData.Data.ShopData.OnItemBought -= CheckItemBought;
            InstancesProvider.Instance.PlayerInventory.OnItemAdded -= CheckPlayerItems;
            InstancesProvider.Instance.PlayerInventory.OnItemCountIncreased -= CheckPlayerItems;
        }
        private void CheckPlayerItems(int _) => CheckPlayerItems();
        private void CheckPlayerItems()
        {
            if (IsTaskStarted(12))
            {
                if (CheckRBYKeys())
                    TryCompleteTaskId12();
            }
            if (IsTaskStarted(17))
            {
                if (CheckScrapMetal(9))
                    TryCompleteTaskId17();
            }
        }
        private bool CheckScrapMetal(int count)
        {
            int inventoryScrapCount = 0;
            IReadOnlyList<Inventory.PageItem> playerItems = InstancesProvider.Instance.PlayerInventory.Items;
            if (playerItems.Exists(x => x.Id == 10, out Inventory.PageItem scrapMetal))
            {
                inventoryScrapCount += scrapMetal.Count;
            }
            if (playerItems.Exists(x => x.Id == 4, out Inventory.PageItem rustyGear))
            {
                inventoryScrapCount += rustyGear.Count;
            }
            return inventoryScrapCount >= count;
        }

        private bool CheckRBYKeys()
        {
            IReadOnlyList<Inventory.PageItem> playerItems = InstancesProvider.Instance.PlayerInventory.Items;
            bool containRedKey = playerItems.Exists(x => x.Id == 6, out _);
            bool containBlueKey = playerItems.Exists(x => x.Id == 8, out _);
            bool containYellowKey = playerItems.Exists(x => x.Id == 9, out _);
            return containRedKey && containBlueKey && containYellowKey;
        }
        private void CheckItemBought(int shopItemId)
        {
            TryCompleteTask(11);
            switch (shopItemId)
            {
                default: break;
            }

        }
        private void CheckWeaponOpened(int weaponId)
        {
            switch (weaponId)
            {
                case 2: TryCompleteTaskId4(); break;
                default: break;
            }
        }
        private void CheckModifierOpened(int modifierId)
        {
            TryCompleteTask(6);
            switch (modifierId)
            {
                case 5: TryCompleteTaskId15(); break;
                default: break;
            }
        }
        /// <summary>
        /// Any changes made in this method will be saved after the world was loaded in first time
        /// </summary>
        /// <param name="currentWorld"></param>
        private void CheckWorldLoaded(WorldType currentWorld)
        {
            switch (currentWorld)
            {
                case WorldType.Portal:
                    TryCompleteTaskId3();
                    TryStartTaskId11();
                    if (IsTaskCompleted(16) && !IsTaskStarted(17) && !IsTaskCompleted(17))
                        OnTaskCompletedId16Message2();
                    break;
                case WorldType.Factory:
                    TryCompleteTaskId1();
                    break;
                case WorldType.CrystalClockwork:
                    TryCompleteTaskId5();
                    break;
                case WorldType.MetalRecycling:
                    TryCompleteTaskId13();
                    break;
                case WorldType.Chaos:
                    TryCompleteTaskId18();
                    break;
                default: break;
            }
        }
        private void CheckPortalUsed(WorldType worldLoaded)
        {
            switch (worldLoaded)
            {
                case WorldType.Portal:
                    TryCompleteTask(7);
                    break;
                default: break;
            }
        }
        private void CheckEnableActions()
        {
            if (!GameData.Data.TutorialData.IsCompleted) return;
            TryCompleteTask(0);
            TryStartTask(1);
            FixTasks();
        }
        /// <summary>
        /// Place any tasks that may be corrupted by player leave from game
        /// </summary>
        private void FixTasks()
        {
            TryStartTaskIfCompleted(taskToStart: 2, completedTask: 1);
            TryStartTaskIfCompleted(3, 2);
            TryStartTaskIfCompleted(4, 3);
            TryStartTaskIfCompleted(8, 5);
            TryStartTaskIfCompleted(9, 5);
            TryStartTaskIfCompleted(10, 8);
            TryStartTaskIfCompleted(11, 10);
            TryStartTaskIfCompleted(13, 12);
            TryStartTaskIfCompleted(14, 13);
            TryStartTaskIfCompleted(15, 14);
            TryStartTaskIfCompleted(16, 15);
            TryStartTaskIfCompleted(17, 16);
            TryStartTaskIfCompleted(18, 17);
            TryStartTaskIfCompleted(19, 18);
            TryStartTaskIfCompleted(20, 19);
            TryStartTaskIfCompleted(21, 20);
        }
        private void TryStartTaskIfCompleted(int taskToStart, int completedTask)
        {
            if (IsTaskCompleted(completedTask))
                TryStartTask(taskToStart);
        }

        private void CheckRoomStarted(int roomId)
        {
            switch (roomId)
            {
                case 13: TryStartTaskId7(); break; //factory portal room
                case 20: TryCompleteTaskId2(); break; //final factory room
                case 21: TryStartTaskId6(); break; //modifier factory room 1
                case 22: TryStartTaskId6(); break; //modifier factory room 2
                case 37: TryCompleteTaskId8(); break; //golem room
                case 30: TryCompleteTaskId9(); break; //final cc room
                case 73: TryCompleteTaskId20(); break; //final chaos room
                default: break;
            }
        }
        private void CheckRoomCompleted(int roomId)
        {
            switch (roomId)
            {
                case 58: TryCompleteTaskId16(); break; //first portals mr room
                default: break;
            }
        }
        private void OnTaskCompleted(int taskId)
        {
            switch (taskId)
            {
                case 10: OnTaskCompletedId10(); break;
                case 14: OnTaskCompletedId14(); break;
                case 19: OnTaskCompletedId19(); break;
                case 21: OnTaskCompletedId21(); break;
                default: break;
            }
        }
        private void OnTaskStarted(int taskId)
        {
            switch (taskId)
            {
                case 0: break;
                default: break;
            }
        }
        private void OnTaskCompletedId21() //solve chaos puzzle
        {
            StatsBehaviour.Stats.Health.ChangeMaxRange(50, false);
            GameData.Data.WorldsData.TryCloseWorld(WorldType.Factory);
            GameData.Data.WorldsData.TryCloseWorld(WorldType.CrystalClockwork);
            GameData.Data.WorldsData.TryCloseWorld(WorldType.MetalRecycling);
            GameData.Data.WorldsData.TryCloseWorld(WorldType.Chaos);
            GameData.Data.WorldsData.TryCompleteWorld(WorldType.Chaos);
            GameData.Data.WorldsData.TryOpenWorld(WorldType.Hopelessness);
        }
        private void TryCompleteTaskId20() //find final chaos room
        {
            if (!TryCompleteTask(20)) return;
            TryStartTask(21);
        }

        private void OnTaskCompletedId19() //find item 12
        {
            DisablePlayerInput();
            ShowGameMessage(58);
            TryStartTask(20);
            helpMessages.OnMessageHidden += OnTaskCompletedId19Message1;
        }
        private void OnTaskCompletedId19Message1()
        {
            helpMessages.OnMessageHidden -= OnTaskCompletedId19Message1;
            ShowGameMessage(59);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }
        private void TryCompleteTaskId18() //solve mr final puzzle
        {
            if (!TryCompleteTask(18)) return;
            DisablePlayerInput();
            TryStartTask(19);
            ShowGameMessage(57);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }
        private void TryCompleteTaskId17() //find scrap metal
        {
            if (!TryCompleteTask(17)) return;
            DisablePlayerInput();
            TryStartTask(18);
            ShowGameMessage(55);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }
        private void TryCompleteTaskId16() //find room with portals 2
        {
            if (!TryCompleteTask(16)) return;
            DisablePlayerInput();
            ShowGameMessage(52);
            helpMessages.OnMessageHidden += OnTaskCompletedId16Message1;
        }
        private void OnTaskCompletedId16Message1()
        {
            StatsBehaviour.Stats.Health.SetToMin();
        }
        private void OnTaskCompletedId16Message2()
        {
            DisablePlayerInput();
            ShowGameMessage(53);
            helpMessages.OnMessageHidden += OnTaskCompletedId16Message3;
        }
        private void OnTaskCompletedId16Message3()
        {
            TryStartTask(17);
            helpMessages.OnMessageHidden -= OnTaskCompletedId16Message3;
            ShowGameMessage(54);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }
        private void TryCompleteTaskId15() //find infrared glasses
        {
            if (!TryCompleteTask(15)) return;
            DisablePlayerInput();
            TryStartTask(16);
            ShowGameMessage(51);
            GameData.Data.ShopData.GenerateData();
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }
        private void OnTaskCompletedId14() //find room with portals 1
        {
            DisablePlayerInput();
            TryStartTask(15);
            ShowGameMessage(50);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }
        private void TryCompleteTaskId13() //solve cc final puzzle
        {
            if (!TryCompleteTask(13)) return;
            DisablePlayerInput();
            TryStartTask(14);
            StatsBehaviour.Stats.Health.ChangeMaxRange(StatsBehaviour.Stats.Health.GetRange().y + 40, false);
            StatsBehaviour.Stats.Stamina.ChangeMaxRange(StatsBehaviour.Stats.Stamina.GetRange().y + 40, false);
            ShowGameMessage(48);
            helpMessages.OnMessageHidden += OnTaskCompletedId13Message1;
        }
        private void OnTaskCompletedId13Message1()
        {
            helpMessages.OnMessageHidden -= OnTaskCompletedId13Message1;
            ShowGameMessage(49);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }
        private void ShowGameMessage(int id) => helpMessages.TryShowMessage(LanguageLoader.GetTextByType(TextType.Game, id));
        private void TryCompleteTaskId12() //find all keys
        {
            helpMessages.OnMessageHidden -= TryCompleteTaskId12;
            if (!TryCompleteTask(12)) return;
            TryStartTask(13);
            DisablePlayerInput();
            ShowGameMessage(47);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }
        private void TryCompleteTaskId9() //find final cc room
        {
            if (!TryCompleteTask(9)) return;
            TryStartTask(12);
            DisablePlayerInput();
            ShowGameMessage(46);
            if (CheckRBYKeys())
            {
                helpMessages.OnMessageHidden += TryCompleteTaskId12;
            }
            else
            {
                helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
            }
        }
        private void TryStartTaskId11() //buy something from golem
        {
            if (!IsTaskCompleted(10)) return;
            if (!GameData.Data.TasksData.IsTaskCanBeStarted(11)) return;
            DisablePlayerInput();
            ShowGameMessage(43);
            helpMessages.OnMessageHidden += TryStartTaskId11Message2;
        }
        private void TryStartTaskId11Message2() //buy something from golem
        {
            helpMessages.OnMessageHidden -= TryStartTaskId11Message2;
            TryStartTask(11);
            ShowGameMessage(44);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }

        private void OnTaskCompletedId10() //free the golem
        {
            DisablePlayerInput();
            ShowGameMessage(42);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }
        private void TryCompleteTaskId8() //find room with golem
        {
            if (!TryCompleteTask(8)) return;
            TryStartTask(10);
            DisablePlayerInput();
            ShowGameMessage(41);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }

        private void TryCompleteTaskId5() //to the crystal clockwork world
        {
            if (!TryCompleteTask(5)) return;
            TryStartTask(9);
            DisablePlayerInput();
            ShowGameMessage(39);
            helpMessages.OnMessageHidden += OnTaskCompletedId5Message2;
        }
        private void OnTaskCompletedId5Message2()
        {
            TryStartTask(8);
            helpMessages.OnMessageHidden -= OnTaskCompletedId5Message2;
            ShowGameMessage(40);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }

        private void TryStartTaskId7() //use portal
        {
            if (!TryStartTask(7)) return;
            DisablePlayerInput();
            ShowGameMessage(37);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }

        private void TryStartTaskId6() //find room with yellow mark (21, 22)
        {
            if (!TryStartTask(6)) return;
            DisablePlayerInput();
            ShowGameMessage(38);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }

        private void TryCompleteTaskId4() //find ricochet
        {
            if (!TryCompleteTask(4)) return;
            TryStartTask(5);
            DisablePlayerInput();
            ShowGameMessage(34);
            helpMessages.OnMessageHidden += EnablePlayerInputAndUnsubscribe;
        }
        private void TryCompleteTaskId3() //leave the factory
        {
            if (!TryCompleteTask(3)) return;
            DisablePlayerInput();
            ShowGameMessage(32);
            helpMessages.OnMessageHidden += OnTaskCompletedId3Message1;
        }
        private void OnTaskCompletedId3Message1()
        {
            EnablePlayerInput();
            helpMessages.OnMessageHidden -= OnTaskCompletedId3Message1;
            TryStartTask(4);
        }
        private void TryCompleteTaskId2() //find room id 20 (factory final room)
        {
            if (!TryCompleteTask(2)) return;
            DisablePlayerInput();
            ShowGameMessage(30);
            helpMessages.OnMessageHidden += OnRoom20Message2;
        }
        private void OnRoom20Message2()
        {
            helpMessages.OnMessageHidden -= OnRoom20Message2;
            ShowGameMessage(31);
            helpMessages.OnMessageHidden += OnRoom20Message2InvokeUI;
        }
        private void OnRoom20Message2InvokeUI()
        {
            EnablePlayerInput();
            helpMessages.OnMessageHidden -= OnRoom20Message2InvokeUI;
            TryStartTask(3);
        }

        private void TryCompleteTaskId1()
        {
            if (!TryCompleteTask(1)) return;
            DisablePlayerInput();
            ShowGameMessage(24);
            helpMessages.OnMessageHidden += OnMechanicsTutorial2;
        }
        private void OnMechanicsTutorial2()
        {
            helpMessages.OnMessageHidden -= OnMechanicsTutorial2;
            ShowGameMessage(25);
            helpMessages.OnMessageHidden += OnMechanicsTutorialInvokeUI;
        }
        private void OnMechanicsTutorialInvokeUI()
        {
            helpMessages.OnMessageHidden -= OnMechanicsTutorialInvokeUI;
            EnablePlayerInput();
            TryStartTask(2);
        }
        private void EnablePlayerInputAndUnsubscribe()
        {
            helpMessages.OnMessageHidden -= EnablePlayerInputAndUnsubscribe;
            EnablePlayerInput();
        }

        private bool IsTaskCompleted(int id) => GameData.Data.TasksData.IsTaskCompleted(id);
        private bool IsTaskStarted(int id) => GameData.Data.TasksData.IsTaskStarted(id);
        private bool TryStartTask(int id) => GameData.Data.TasksData.TryStartTask(id);
        private bool TryCompleteTask(int id) => GameData.Data.TasksData.TryCompleteTask(id);

        private void DisablePlayerInput() => InstancesProvider.Instance.PlayerInput.StopInput();
        private void EnablePlayerInput() => InstancesProvider.Instance.PlayerInput.StartInput();
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private bool ___testBool;
        [SerializeField][Min(0)] private int taskToTest;

        [Button(nameof(TestCompleteTask))]
        private void TestCompleteTask()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            TryCompleteTask(taskToTest);
        }
        [Button(nameof(TestStartTask))]
        private void TestStartTask()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            TryStartTask(taskToTest);
        }

#endif //UNITY_EDITOR

    }
}