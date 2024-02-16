using Data.Stored;
using DebugStuff;
using EditorCustom.Attributes;
using Game.Labyrinth;
using Game.Player;
using Game.Rooms;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game
{
    public class AchievementsObserver : SingleSceneInstance<AchievementsObserver>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            StatsBehaviour.Stats.Health.OnMaxRangeChanged += CheckMaxHealth;
            foreach (var world in GameData.Data.WorldsData.Stats)
            {
                world.OnCompleted += CheckWorldCompleted;
            }
            Room.OnCompletedRoom += CheckRoomCompleted;
            GameData.Data.StatisticData.OnGearsSpentChanged += CheckGearsSpent;
        }
        private void OnDisable()
        {
            StatsBehaviour.Stats.Health.OnMaxRangeChanged -= CheckMaxHealth;
            foreach (var world in GameData.Data.WorldsData.Stats)
            {
                world.OnCompleted -= CheckWorldCompleted;
            }
            Room.OnCompletedRoom -= CheckRoomCompleted;
            GameData.Data.StatisticData.OnGearsSpentChanged -= CheckGearsSpent;
        }
        private void CheckGearsSpent(int amount)
        {
            if (amount >= 100)
            {
                SetAchievement("ACH_GEARS_SPENT_100");
            }
        }
        private void CheckRoomCompleted(Room room)
        {
            if (room.Id < 12) return;
            if (room.TimeSpentToComplete <= 5)
            {
                SetAchievement("ACH_ROOM_FAST");
            }
        }
        private void CheckWorldCompleted(WorldType worldType)
        {
            SetAchievement($"ACH_WORLD_{(int)worldType}");
        }
        private void CheckMaxHealth(int maxHealth)
        {
            if (maxHealth > 1) return;
            SetAchievement("ACH_HP_1");
        }

        public static void SetAchievement(string name)
        {
            if (!SteamManager.Initialized) return;
            SteamUserStats.SetAchievement(name);
            SteamUserStats.StoreStats();
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private bool ___testBool;
        [SerializeField][Label("Achievement To Set")] private string ___testAchievement;
        [Button(nameof(TestSetAchievement))]
        private void TestSetAchievement()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            SetAchievement(___testAchievement);
        }
#endif //UNITY_EDITOR

    }
}