using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Data.Stored
{
    [System.Serializable]
    public class StatisticData
    {
        #region fields & properties
        public UnityAction<int> OnDeathCountChanged;
        public UnityAction<int> OnGearsSpentChanged;
        /// <summary>
        /// <see cref="{T0}"/> - timePlayed
        /// </summary>
        public UnityAction<int> OnTimePlayedChanged;

        public string PlayerId => $"R{DeathCount + 1:000}";
        public int DeathCount
        {
            get => deathCount;
            set => SetDeathCount(value);
        }
        [SerializeField] private int deathCount = 0;
        public int TimePlayed => timePlayed;
        [SerializeField] private int timePlayed = 0;
        public int GearsSpent
        {
            get => gearsSpent;
            set => SetGearsSpent(value);
        }
        [SerializeField] private int gearsSpent = 0;
        public OpenedItems OpenedMemories => openedMemories;
        [SerializeField] private OpenedItems openedMemories = new();

        #endregion fields & properties

        #region methods
        private void SetGearsSpent(int value)
        {
            gearsSpent = Mathf.Max(value, 0);
            OnGearsSpentChanged?.Invoke(gearsSpent);
        }
        private void SetDeathCount(int value)
        {
            deathCount = Mathf.Max(value, 0);
            OnDeathCountChanged?.Invoke(deathCount);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">>0</param>
        public void IncreaseTimePlayed(int value)
        {
            value = Mathf.Max(value, 0);
            timePlayed += value;
            OnTimePlayedChanged?.Invoke(timePlayed);
        }
        #endregion methods
    }
}