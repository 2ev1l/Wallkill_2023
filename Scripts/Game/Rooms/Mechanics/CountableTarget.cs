using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Rooms.Mechanics
{
    public class CountableTarget : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnMaxCountReachedEvent;
        public UnityAction OnMaxCountReached;
        /// <summary>
        /// <see cref="{T0}"/> - Increased Amount
        /// <see cref="{T1}"/> - this
        /// </summary>
        public UnityAction<int, CountableTarget> OnCountIncreased;
        public UnityAction OnCountChanged;
        public int MaxCount => maxCount;
        [SerializeField][Min(0)] private int maxCount = 1;
        public int CurrentCount => currentCount;
        [Title("Read Only")]
        [SerializeField][ReadOnly][Min(0)] private int currentCount = 0;
        #endregion fields & properties

        #region methods
        public void ResetCount()
        {
            currentCount = 0;
            OnCountChanged?.Invoke();
        }
        public void IncreaseCount(int amount)
        {
            if (maxCount <= currentCount) return;
            if (amount <= 0) return;
            amount = Mathf.Min(amount, maxCount - currentCount);
            currentCount += amount;
            OnCountChanged?.Invoke();
            OnCountIncreased?.Invoke(amount, this);
            CheckMaxCount();
        }
        [SerializedMethod]
        public void IncreaseCount()
        {
            IncreaseCount(1);
        }
        private bool CheckMaxCount()
        {
            if (maxCount != currentCount) return false;
            OnMaxCountReached?.Invoke();
            OnMaxCountReachedEvent?.Invoke();
            return true;
        }
#if UNITY_EDITOR
        private void Awake()
        {
            if (currentCount != 0)
            {
                Debug.Log($"Check current count in {nameof(CountableTarget)} at {gameObject.name} (click to hover)", this);
            }
        }
        [Title("Tests")]
        [SerializeField][DontDraw] private string ___testString;
        [Button(nameof(IncreaseCountTest))]
        private void IncreaseCountTest()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;

            IncreaseCount();
        }
        //[Button(nameof(DoAllTests))]
        private void DoAllTests()
        {
            //save values
            int storedCurrent = currentCount;
            int storedMaxCount = maxCount;

            //test1
            currentCount = 0;
            maxCount = 2;
            IncreaseCount();
            if (CheckMaxCount()) ShowTestError();
            IncreaseCount(1);
            if (!CheckMaxCount()) ShowTestError();

            //test2
            currentCount = 0;
            maxCount = 7;
            IncreaseCount(7);
            if (!CheckMaxCount()) ShowTestError();

            //test3
            currentCount = 0;
            maxCount = 7;
            IncreaseCount(6);
            if (CheckMaxCount()) ShowTestError();
            IncreaseCount(1);
            if (!CheckMaxCount()) ShowTestError();
            IncreaseCount();
            if (!CheckMaxCount()) ShowTestError();
            IncreaseCount(0);
            if (!CheckMaxCount()) ShowTestError();

            //test4
            currentCount = 0;
            maxCount = 1;
            IncreaseCount(0);
            if (CheckMaxCount()) ShowTestError();
            IncreaseCount(2);
            if (!CheckMaxCount()) ShowTestError();

            //test5
            currentCount = 0;
            maxCount = 0;
            if (!CheckMaxCount()) ShowTestError();
            IncreaseCount(2);
            if (!CheckMaxCount()) ShowTestError();

            //restore values
            maxCount = storedMaxCount;
            currentCount = storedCurrent;
        }
        private void ShowTestError()=> Debug.LogError("Test Failed");
#endif //UNITY_EDITOR
        #endregion methods
    }
}