using System.Collections;
using UnityEngine;
using EditorCustom.Attributes;

namespace Universal
{
    [System.Serializable]
    public class Timer
    {
        #region fields & properties
        public System.Action OnChangeEnd;
        /// <summary>
        /// In seconds
        /// </summary>
        public float Time => time;
        [SerializeField][ReadOnly] private float time = 0;
        public bool IsEnded => isEnded;
        [SerializeField][ReadOnly] private bool isEnded = false;
        private static MonoBehaviour Context => SingleGameInstance.Instance;
        private float secondsToWait;
        private bool isStarted;
        protected System.Func<bool> BreakCondition = delegate { return false; };
        protected bool InvokeEndAtBreak = true;
        private bool isForceStopped = false;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Simulates <see cref="BreakCondition"/> with ~1 frame delay.
        /// </summary>
        public void Break()
        {
            isForceStopped = true;
        }
        /// <summary>
        /// Simulates <see cref="BreakCondition"/> immediately. Be careful with <see cref="OnChangeEnd"/>
        /// </summary>
        /// <exception cref="System.StackOverflowException"></exception>
        public void BreakImmediately()
        {
            Break();
            if (InvokeEndAtBreak)
            {
                InvokeEndAtBreak = false;
                End();
            }
            else
            {
                time = secondsToWait;
                isEnded = true;
            }
        }
        /// <summary>
        /// Timer can be started only once.
        /// </summary>
        /// <param name="secondsToWait"></param>
        /// <returns></returns>
        public bool TryStartTimer(float secondsToWait)
        {
            if (isStarted) return false;
            isStarted = true;
            StartTimer(secondsToWait);
            return true;
        }
        private void StartTimer(float secondsToWait)
        {
            this.secondsToWait = secondsToWait;
            isEnded = false;
            time = 0;
            if (Context != null)
                Context.StartCoroutine(Change());
        }
        private IEnumerator Change()
        {
            while (time < secondsToWait)
            {
                yield return WaitForTimeUnit();
                if (isForceStopped || (BreakCondition != null && BreakCondition.Invoke()))
                {
                    if (InvokeEndAtBreak) break;
                    time = secondsToWait;
                    isEnded = true;
                    yield break;
                }

                OnTimeChanged(time, secondsToWait);
                time += GetTimeUnit();
            }
            End();
        }
        public virtual float GetTimeUnit() => UnityEngine.Time.deltaTime;
        public virtual IEnumerator WaitForTimeUnit()
        {
            yield return CustomMath.WaitAFrame();
        }
        public IEnumerator WaitUntilEnd()
        {
            while (!IsEnded)
                yield return WaitForTimeUnit();
        }
        protected virtual void OnTimeChanged(float lerp, float time) { }
        protected virtual void End()
        {
            time = secondsToWait;
            isEnded = true;
            OnChangeEnd?.Invoke();
            BreakCondition = null;
            OnChangeEnd = null;
        }
        #endregion methods
    }
}