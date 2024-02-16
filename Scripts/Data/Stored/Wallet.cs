using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Stored
{
    [System.Serializable]
    public class Wallet : ICloneable<Wallet>
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - amount changed (<>=0)
        /// </summary>
        public UnityAction<int> OnValueChangedAmount;
        public UnityAction<int> OnValueChanged;
        public int Value => value;
        [SerializeField][Min(0)] private int value = 0;
        #endregion fields & properties

        #region methods
        public bool CanSpent(int amount) => value >= amount;
        public bool TrySpent(int amount)
        {
            if (amount <= 0) return false;
            if (!CanSpent(amount)) return false;
            value -= amount;
            OnValueChanged?.Invoke(value);
            OnValueChangedAmount?.Invoke(-amount);
            return true;
        }
        public void AddValue(int amount)
        {
            if (amount <= 0) return;
            value += amount;
            OnValueChanged?.Invoke(value);
            OnValueChangedAmount?.Invoke(amount);
        }

        public Wallet Clone() => new()
        {
            value = value,
        };

        public Wallet() { }
        public Wallet(int value) 
        {
            this.value = value;
        }
        #endregion methods
    }
}