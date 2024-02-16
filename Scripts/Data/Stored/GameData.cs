using Data.Interfaces;
using UnityEngine;

namespace Data.Stored
{
    [System.Serializable]
    internal class GameData : ISaveable
    {
        #region fields & properties
        public static readonly string SaveName = "save";
        public static readonly string SaveExtension = ".data";

        public static GameData Data
        {
            get => data;
            set => SetData(value);
        }
        private static GameData data;

        public PlayerData PlayerData => playerData;
        [SerializeField] private PlayerData playerData = new();
        public WorldsData WorldsData => worldsData;
        [SerializeField] private WorldsData worldsData = new();
        public KeyCodesData KeyCodesData => keyCodesData;
        [SerializeField] private KeyCodesData keyCodesData = new();
        public TutorialData TutorialData => tutorialData;
        [SerializeField] private TutorialData tutorialData = new();
        public StatisticData StatisticData => statisticData;
        [SerializeField] private StatisticData statisticData = new();
        public TasksData TasksData => tasksData;
        [SerializeField] private TasksData tasksData = new();
        public ShopData ShopData => shopData;
        [SerializeField] private ShopData shopData = new();
        #endregion fields & properties

        #region methods
        private static void SetData(GameData value) => data = value ?? new GameData();
        #endregion methods
    }
}