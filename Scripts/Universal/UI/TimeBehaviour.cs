using Data.Interfaces;
using Data.Stored;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI
{
    public class TimeBehaviour : MonoBehaviour, IInitializable
    {
        #region fields & properties
        public const int UpdateRate = 1;
        private static MonoBehaviour Context => SingleGameInstance.Instance;
        [Title("Read Only")]
        [SerializeField][ReadOnly] private bool isInitialized = false;
        [SerializeField][ReadOnly] private int timeReference;
        #endregion fields & properties

        #region methods
        public void Init()
        {
            if (isInitialized) return;
            isInitialized = true;
            Context.StartCoroutine(IncreaseTime());
        }
        private IEnumerator IncreaseTime()
        {
            yield return new WaitForSecondsRealtime(UpdateRate);
            if (Context == null) yield break;

            GameData.Data.StatisticData.IncreaseTimePlayed(UpdateRate);
            timeReference = GameData.Data.StatisticData.TimePlayed;

            Context.StartCoroutine(IncreaseTime());
        }
        #endregion methods
    }
}