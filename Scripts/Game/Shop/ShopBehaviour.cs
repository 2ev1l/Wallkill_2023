using Data.Stored;
using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Game.Shop
{
    public class ShopBehaviour : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnBeforeDataChanged;
        public UnityAction OnAfterDataChanged;

        [SerializeField] private bool useGameData = true;
        [SerializeField] private bool updateOnDisable = true;
        public ShopData ShopData
        {
            get => useGameData ? GameData.Data.ShopData : shopData;
            set => SetShopData(value);
        }
        [SerializeField][DrawIf(nameof(useGameData), false)] private ShopData shopData;

        [SerializeField][Min(1)] private int updateRate = 600;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private bool isSubscribed = false;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            CheckDataUpdate(GameData.Data.StatisticData.TimePlayed);
        }
        private void OnEnable()
        {
            if (updateOnDisable)
            {
                Unsubscribe();
            }
            else
            {
                Subscribe();
                CheckDataUpdate(GameData.Data.StatisticData.TimePlayed);
            }
        }
        private void OnDisable()
        {
            if (updateOnDisable)
            {
                Subscribe();
                CheckDataUpdate(GameData.Data.StatisticData.TimePlayed);
            }
            else
            {
                Unsubscribe();
            }
        }
        private void OnDestroy()
        {
            Unsubscribe();
        }
        private void SetShopData(ShopData value)
        {
            if (useGameData)
            {
                Debug.Log("Can't change shop data for GameData", this);
                return;
            }
            OnBeforeDataChanged?.Invoke();
            shopData = value;
            OnAfterDataChanged?.Invoke();
        }
        private void Subscribe()
        {
            if (isSubscribed) return;
            GameData.Data.StatisticData.OnTimePlayedChanged += CheckDataUpdate;
            isSubscribed = true;
        }
        private void Unsubscribe()
        {
            if (!isSubscribed) return;
            GameData.Data.StatisticData.OnTimePlayedChanged -= CheckDataUpdate;
            isSubscribed = false;
        }
        private void CheckDataUpdate(int currentTime)
        {
            if (!TimeDelay.IsDelayReady(currentTime, ShopData.LastTimeGenerated, updateRate)) return;
            ShopData.GenerateData();
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private bool ___testBool;
        [Button(nameof(TestGenerateNewData))]
        private void TestGenerateNewData()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            ShopData.GenerateData();
        }
#endif //UNITY_EDITOR

    }
}