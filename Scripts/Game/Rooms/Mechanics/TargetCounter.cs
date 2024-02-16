using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Rooms.Mechanics
{
    public class TargetCounter : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnCountReachedEvent;
        public UnityAction OnCountReached;
        public UnityAction OnCountChanged;

        protected List<CountableTarget> CountableTargets => countableTargets;
        [SerializeField] private List<CountableTarget> countableTargets = new();
        [SerializeField] private bool overrideDefaultCount = false;
        [DrawIf(nameof(overrideDefaultCount), true)][SerializeField][Min(0)] private int countToReach;

        public int CurrentCount => currentCount;
        [Title("Read Only")][SerializeField][ReadOnly][Min(0)] protected int currentCount = 0;
        public bool IsCountReached => isCountReached;
        [SerializeField][ReadOnly] private bool isCountReached;
        #endregion fields & properties

        #region methods

        private void OnEnable()
        {
            countableTargets.ForEach(x => x.OnCountIncreased += IncreaseTotalCount);
        }
        private void OnDisable()
        {
            countableTargets.ForEach(x => x.OnCountIncreased -= IncreaseTotalCount);
        }
        [SerializedMethod]
        public void ResetTotalCount()
        {
            currentCount = 0;
            countableTargets.ForEach(x => x.ResetCount());
            isCountReached = false;
            OnCountChanged?.Invoke();
        }
        private void IncreaseTotalCount(int amount) => IncreaseTotalCount(amount, null);
        protected virtual void IncreaseTotalCount(int amount, CountableTarget sender)
        {
            currentCount += amount;
            OnCountChanged?.Invoke();
            CheckReach();
        }
        protected bool CheckReach()
        {
            int neededCount = GetNeededCount();
            if (neededCount != currentCount) return false;

            isCountReached = true;
            OnCountReached?.Invoke();
            OnCountReachedEvent?.Invoke();
            return true;
        }
        public int GetNeededCount()
        {
            int neededCount = 0;
            if (overrideDefaultCount)
            {
                neededCount = countToReach;
            }
            else
            {
                foreach (var el in countableTargets)
                {
                    neededCount += el.MaxCount;
                }
            }
            return neededCount;
        }
        #endregion methods
#if UNITY_EDITOR
        private void Awake()
        {
            CheckFields();
        }
        protected virtual void CheckFields()
        {
            if (currentCount != 0)
            {
                Debug.Log($"Check current count in {nameof(TargetCounter)} at {gameObject.name} (click to hover)", this);
            }
            if (isCountReached)
            {
                Debug.Log($"Check reached bool in {nameof(TargetCounter)} at {gameObject.name} (click to hover)", this);
            }
        }
        [Title("Debug")]
        [SerializeField][ReadOnly][Label("Needed Count")] private int neededCountDebug;
        private void OnDrawGizmosSelected()
        {
            try { neededCountDebug = GetNeededCount(); }
            catch { }
            for (int i = 0; i < countableTargets.Count; ++i)
            {
                CountableTarget el = countableTargets[i];
                if (countableTargets.Count(x => x == el) == 1) continue;
                Debug.LogError($"Targets must be unique", this);
            }
        }

        [Button(nameof(GetAllTargetsInChild))]
        protected virtual void GetAllTargetsInChild()
        {
            List<CountableTarget> targets = transform.GetComponentsInChildren<CountableTarget>(true).ToList();
            targets = targets.Where(x => !countableTargets.Contains(x)).ToList();
            UnityEditor.Undo.RecordObject(this, "Added countable targets");
            countableTargets.AddRange(targets);
        }

        [Title("Tests")]
        [SerializeField][DontDraw] private string ___testString;
        //[Button(nameof(DoAllTests))]
        private void DoAllTests()
        {
            //save values
            int storedCurrent = currentCount;
            bool storedOverride = overrideDefaultCount;
            int storedOverrideCount = countToReach;

            //test1
            currentCount = 0;
            overrideDefaultCount = true;
            countToReach = 2;
            IncreaseTotalCount(1);
            if (CheckReach()) ShowTestError();
            IncreaseTotalCount(1);
            if (!CheckReach()) ShowTestError();

            //test2
            currentCount = 0;
            countToReach = 7;
            IncreaseTotalCount(7);
            if (!CheckReach()) ShowTestError();

            //test3
            currentCount = 0;
            countToReach = 7;
            IncreaseTotalCount(6);
            if (CheckReach()) ShowTestError();
            IncreaseTotalCount(1);
            if (!CheckReach()) ShowTestError();
            IncreaseTotalCount(1);
            if (CheckReach()) ShowTestError();


            //restore values
            countToReach = storedOverrideCount;
            overrideDefaultCount = storedOverride;
            currentCount = storedCurrent;
            isCountReached = false;
        }
        private void ShowTestError() => Debug.LogError("Test Failed");
        [Button(nameof(SimulateFullCount))]
        protected virtual void SimulateFullCount()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            currentCount = 0;
            IncreaseTotalCount(GetNeededCount());
        }
#endif
    }
}