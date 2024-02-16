using Data.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using Universal.UI;

namespace Universal
{
    [System.Serializable]
    public class TimeDelayContinuous : TimeDelay, ICloneable<TimeDelayContinuous>
    {
        #region fields & properties
        public UnityAction OnDelayBreak;
        public bool IsDelaying => isDelaying;
        private bool isDelaying = false;
        public override bool CanActivate => base.CanActivate && !isDelaying;
        private System.Action InvokeAtEnd = null;
        #endregion fields & properties

        #region methods
        public bool TryBreakDelaying()
        {
            if (!IsDelaying) return false;
            isDelaying = false;
            lastTimeActivation = -Mathf.Infinity;
            OnDelayBreak?.Invoke();
            return true;
        }
        public void Activate(System.Action invokeAtEnd)
        {
            this.InvokeAtEnd = invokeAtEnd;
            Activate();
        }
        public override void Activate()
        {
            isDelaying = true;
            base.Activate();
        }
        protected override void InvokeActions()
        {
            OnActivated?.Invoke();
            ValueTimeChanger vtc = new(Delay, 0, Delay, ValueTimeChanger.DefaultCurve,
                setValue: x => OnTimeLasts?.Invoke(x),
                onEnd: delegate 
                {
                    isDelaying = false;
                    InvokeAtEnd?.Invoke();
                    OnDelayReady?.Invoke(); 
                },
                breakCondition: delegate
                {
                    return !isDelaying;
                },
                invokeEndAtBreak: false);
        }

        public new TimeDelayContinuous Clone()
        {
            return new()
            {
                delay = delay,
                lastTimeActivation = lastTimeActivation,
                isDelaying = isDelaying,
            };
        }
        #endregion methods
    }
}