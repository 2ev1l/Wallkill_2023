using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using UnityEngine;
using Universal.UI;
using Universal.UI.Layers;

namespace Game.Player
{
    [System.Serializable]
    public class StatChangeLayer : DefaultLayer<Stat>
    {
        #region fields & properties
        public float ChangeSpeed
        {
            get => changeSpeed;
            set => changeSpeed = value;
        }
        [SerializeField][Min(0)] private float changeSpeed = 1f;
        [SerializeField][Min(1)] private int minChangedAmount = 1;
        [SerializeField][ReadOnly] private float storedChangeAmount = 0f;
        public float ChangeSpeedScale 
        { 
            get => changeSpeedScale; 
            set => changeSpeedScale = value;
        }
        [SerializeField][ReadOnly] private float changeSpeedScale = 1f;
        public float ChangedAmountPerSecond => ChangeSpeed * ChangeSpeedScale;
        #endregion fields & properties

        #region methods
        public void IncreaseChangedAmountByTimeSpeed() => storedChangeAmount += ChangedAmountPerSecond * Time.deltaTime;
        public void IncreaseChangedAmountBySpeed() => storedChangeAmount += changeSpeed;
        public void SetChangedAmountToSpeed() => storedChangeAmount = changeSpeed;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trySetStoredAmountToMin"></param>
        /// <returns>True until <see cref="storedChangeAmount"/> is less than <see cref="minChangedAmount"/> 
        /// and <see cref="Stat.Value"/> can be changed</returns>
        public bool TryIncreaseStat(bool trySetStoredAmountToMin, bool increaseUntilLimit)
        {
            if (!TryGetChangedAmount(out int changedAmount)) return true;
            bool result = increaseUntilLimit ? Value.TryIncreaseValue(changedAmount, out int _) : Value.TryIncreaseValue(changedAmount);
            storedChangeAmount -= changedAmount;
            if (!result && trySetStoredAmountToMin) ResetStoredAmountToMin();
            return result;
        }
        /// <summary>
        /// Resets to zero
        /// </summary>
        public void ResetStoredAmount() => storedChangeAmount = 0;
        /// <summary>
        /// Resets to <see cref="minChangedAmount"/>
        /// </summary>
        public void ResetStoredAmountToMin() => storedChangeAmount = minChangedAmount;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trySetStoredAmountToMin"></param>
        /// <returns>True until <see cref="storedChangeAmount"/> is less than <see cref="minChangedAmount"/> 
        /// and <see cref="Stat.Value"/> can be changed</returns>
        public bool TryDecreaseStat(bool trySetStoredAmountToMin, bool decreaseUntilLimit)
        {
            if (!TryGetChangedAmount(out int changedAmount)) return true;
            bool result = decreaseUntilLimit ? Value.TryDecreaseValue(changedAmount, out int _) : Value.TryDecreaseValue(changedAmount);
            storedChangeAmount -= changedAmount;
            if (!result && trySetStoredAmountToMin) ResetStoredAmountToMin();
            return result;
        }
        /// <summary>
        /// Returns true only if changedAmount is >= 1
        /// </summary>
        /// <param name="changedAmount"></param>
        /// <returns></returns>
        private bool TryGetChangedAmount(out int changedAmount)
        {
            changedAmount = 0;
            if (storedChangeAmount < minChangedAmount) return false;
            changedAmount = Mathf.FloorToInt(storedChangeAmount);
            return true;
        }
        public StatChangeLayer() : base() { }
        public StatChangeLayer(int id, Stat value) : base(id, value) { }
        #endregion methods
    }
}