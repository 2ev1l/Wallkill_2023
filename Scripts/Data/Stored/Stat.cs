using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Stored
{
    [System.Serializable]
    public class Stat : ICloneable<Stat>
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - newValue
        /// </summary>
        public UnityAction<int> OnValueChanged;
        public UnityAction OnFailedToDecrease;
        public UnityAction OnFailedToIncrease;
        public UnityAction OnValueReachedMinimum;
        public UnityAction OnValueReachedMaximum;

        /// <summary>
        /// <see cref="{T0}"/> - newMinRange
        /// </summary>
        public UnityAction<int> OnMinRangeChanged;
        /// <summary>
        /// <see cref="{T0}"/> - newMaxRange
        /// </summary>
        public UnityAction<int> OnMaxRangeChanged;
        public UnityAction OnRangeChanged;

        public int Value => value;
        [SerializeField] private int value = 0;

        [SerializeField] private bool doMin = true;
        [DrawIf(nameof(doMin), true)][SerializeField] private int minRange = 0;

        [SerializeField] private bool doMax = true;
        [DrawIf(nameof(doMax), true)][SerializeField] private int maxRange = 1;

        private bool IsValueInWrongRange => value < minRange || value > maxRange;
        public int MinChangesLimit => doMin ? value - minRange : int.MaxValue;
        public int MaxChangesLimit => doMax ? maxRange - value : int.MaxValue;
        #endregion fields & properties

        #region methods
        private void SetValue(int value)
        {
            Vector2Int range = GetRange();
            value = Mathf.Clamp(value, range.x, range.y);
            this.value = value;
            OnValueChanged?.Invoke(this.value);
            if (IsReachedMinimum()) OnValueReachedMinimum?.Invoke();
            if (IsReachedMaximum()) OnValueReachedMaximum?.Invoke();
        }
        /// <summary>
        /// <see cref="Vector2.x"/> = Min; 
        /// <see cref="Vector2.y"/> = Max.
        /// </summary>
        /// <returns></returns>
        public Vector2Int GetRange() => new(doMin ? minRange : int.MinValue, doMax ? maxRange : int.MaxValue);
        public void SetToMax() => SetValue(maxRange);
        public void SetToMin() => SetValue(minRange);

        /// <summary>
        /// Decreases if <see cref="MinChangesLimit"/> is less than amount
        /// </summary>
        /// <param name="amount">Must be greater than 0</param>
        /// <returns></returns>
        public bool TryDecreaseValue(int amount)
        {
            if (amount > MinChangesLimit)
            {
                OnFailedToDecrease?.Invoke();
                return false;
            }
            return TryDecreaseValue(amount, out _);
        }
        /// <summary>
        /// Decreases until the <see cref="minRange"/>
        /// </summary>
        /// <param name="amount">Must be greater than 0</param>
        /// <param name="changedAmount"></param>
        /// <returns></returns>
        public bool TryDecreaseValue(int amount, out int changedAmount)
        {
            changedAmount = 0;
            if (amount <= 0 || MinChangesLimit == 0)
            {
                OnFailedToDecrease?.Invoke();
                return false;
            }
            changedAmount = Mathf.Min(amount, MinChangesLimit);
            DecreaseValue(amount);
            return true;
        }
        private void DecreaseValue(int amount) => SetValue(value - amount);
        private void IncreaseValue(int amount) => SetValue(value + amount);
        /// <summary>
        /// Increases until the <see cref="maxRange"/>
        /// </summary>
        /// <param name="amount">Must be greater than 0</param>
        /// <param name="changedAmount"></param>
        /// <returns></returns>
        public bool TryIncreaseValue(int amount, out int changedAmount)
        {
            changedAmount = 0;
            if (amount <= 0 || MaxChangesLimit == 0)
            {
                OnFailedToIncrease?.Invoke();
                return false;
            }
            changedAmount = Mathf.Min(amount, MaxChangesLimit);
            IncreaseValue(amount);
            return true;
        }
        /// <summary>
        /// Increases if <see cref="MaxChangesLimit"/> is less than amount
        /// </summary>
        /// <param name="amount">Must be greater than 0</param>
        /// <returns></returns>
        public bool TryIncreaseValue(int amount)
        {
            if (amount > MaxChangesLimit)
            {
                OnFailedToIncrease.Invoke();
                return false;
            }
            return TryIncreaseValue(amount, out _);
        }

        public void ChangeRange(int minRange, int maxRange, bool updateValueToLimit)
        {
            ChangeMinRange(minRange, false);
            ChangeMaxRange(maxRange, false);
            if (updateValueToLimit) TryUpdateValueToLimit();
        }
        public void ChangeMaxRange(int maxRange, bool updateValueToLimit)
        {
            this.maxRange = maxRange;
            if (updateValueToLimit) TryUpdateValueToLimit();
            OnMaxRangeChanged?.Invoke(maxRange);
            OnRangeChanged?.Invoke();
        }
        public void ChangeMinRange(int minRange, bool updateValueToLimit)
        {
            this.minRange = minRange;
            if (updateValueToLimit) TryUpdateValueToLimit();
            OnMinRangeChanged?.Invoke(minRange);
            OnRangeChanged?.Invoke();
        }
        private bool TryUpdateValueToLimit()
        {
            if (!IsValueInWrongRange) return false;
            SetValue(value);
            return true;
        }

        public bool IsReachedMinimum()
        {
            if (!doMin) return false;
            return Value == minRange;
        }
        public bool IsReachedMaximum()
        {
            if (!doMax) return false;
            return Value == maxRange;
        }
        public Stat(int value, bool doMin, int minRange, bool doMax, int maxRange)
        {
            this.value = value;
            this.doMin = doMin;
            this.minRange = minRange;
            this.doMax = doMax;
            this.maxRange = maxRange;
        }
        public Stat Clone()
        {
            return new(value, doMin, minRange, doMax, maxRange);
        }
        #endregion methods
    }
}