using Data.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using Universal.UI;

namespace Universal
{
    [System.Serializable]
    public class TimeDelay : ICloneable<TimeDelay>
    {
        #region fields & properties
        public UnityAction OnDelayReady;
        public UnityAction OnActivated;
        /// <summary>
        /// <see cref="{T0}"/> - time lasts until delay is ready
        /// </summary>
        public UnityAction<float> OnTimeLasts;

        public float Delay => delay;
        [SerializeField][Min(0)] protected float delay;
        public float LastTimeActivation => lastTimeActivation;
        protected float lastTimeActivation = -Mathf.Infinity;
        public float TimeSinceLastActivation => Time.time - lastTimeActivation;
        public virtual bool CanActivate => TimeSinceLastActivation > Delay;
        #endregion fields & properties

        #region methods
        public static bool IsDelayReady(float currentTime, float lastTimeActivation, float delay)
        {
            return currentTime - lastTimeActivation > delay;
        }
        /// <summary>
        /// Just allowes you to invoke delay again. Other actions in previous delay will be called anyways.
        /// </summary>
        public void ResetDelay() => lastTimeActivation = -Mathf.Infinity;
        /// <summary>
        /// Activates immediate ignoring any conditions. You might want to use <see cref="CanActivate"/>
        /// </summary>
        public virtual void Activate()
        {
            SetLastTimeActivation();
            InvokeActions();
        }
        private void SetLastTimeActivation() => lastTimeActivation = Time.time;
        protected virtual void InvokeActions()
        {
            OnActivated?.Invoke();
            ValueTimeChanger vtc = new(Delay, 0, Delay, ValueTimeChanger.DefaultCurve,
                setValue: x => OnTimeLasts?.Invoke(x),
                onEnd: delegate { OnTimeLasts?.Invoke(0); OnDelayReady?.Invoke(); });
        }

        public TimeDelay(float delay)
        {
            this.delay = delay;
        }

        public TimeDelay() { }

        public TimeDelay Clone()
        {
            return new()
            {
                delay = delay,
                lastTimeActivation = lastTimeActivation,
            };
        }
        #endregion methods
    }
}