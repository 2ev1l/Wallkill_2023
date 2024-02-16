using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Rooms.Mechanics
{
    public class TargetCounterOrdered : TargetCounter
    {
        #region fields & properties
        public UnityAction OnWrongOrder;

        [Title("Order")]
        public UnityEvent OnWrondOrderEvent;
        public int CurrentOrder
        {
            get => currentOrder;
            set => currentOrder = value;
        }
        [ReadOnly][SerializeField] private int currentOrder = 0;
        #endregion fields & properties

        #region methods
        protected override void IncreaseTotalCount(int amount, CountableTarget sender)
        {
            int senderId = CountableTargets.FindIndex(x => x == sender);
            
            if (senderId == currentOrder)
            {
                ++currentOrder;
                base.IncreaseTotalCount(amount, sender);
            }
            else
            {
                ResetOrder();
                OnWrongOrder?.Invoke();
                OnWrondOrderEvent?.Invoke();
            }
        }
        public void ResetOrder()
        {
            currentOrder = 0;
            ResetTotalCount();
        }
        #endregion methods

#if UNITY_EDITOR
        protected override void CheckFields()
        {
            base.CheckFields();
            if (currentOrder != 0)
            {
                Debug.Log($"Check current order in {nameof(TargetCounter)} at {gameObject.name} (click to hover)", this);
            }
        }
        [Button(nameof(GetAllTargetsInChild))]
        protected override void GetAllTargetsInChild()
        {
            base.GetAllTargetsInChild();
        }
        [Button(nameof(SimulateFullCount))]
        protected override void SimulateFullCount()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            currentCount = GetNeededCount();
            OnCountChanged?.Invoke();
            CheckReach();
        }
#endif //UNITY_EDITOR
    }
}