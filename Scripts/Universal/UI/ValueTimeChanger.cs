using System;
using UnityEngine;
using EditorCustom.Attributes;

namespace Universal.UI
{
    [System.Serializable]
    public class ValueTimeChanger : Timer
    {
        #region fields & properties
        public float Value => value;
        [SerializeField, ReadOnly] float value;
        public static AnimationCurve DefaultCurve => AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve Curve
        {
            get => curve;
            set => curve = value;
        }
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        private readonly float startValue;
        private readonly float finalValue;
        private Action<float> OnTimeChange;
        #endregion fields & properties

        #region methods
        private void Start(float seconds) => base.TryStartTimer(seconds);
        protected override void OnTimeChanged(float time, float totalSeconds)
        {
            value = Mathf.Lerp(startValue, finalValue, curve.Evaluate(time / totalSeconds));
            OnTimeChange?.Invoke(value);
        }
        protected override void End()
        {
            value = finalValue;
            base.End();
        }
        public ValueTimeChanger(float startValue, float finalValue, float seconds)
        {
            this.startValue = startValue;
            this.finalValue = finalValue;
            Start(seconds);
        }
        public ValueTimeChanger(float startValue, float finalValue, float seconds, System.Func<bool> breakCondition)
        {
            this.startValue = startValue;
            this.finalValue = finalValue;
            this.BreakCondition = breakCondition;
            Start(seconds);
        }
        public ValueTimeChanger(float startValue, float finalValue, float seconds, Action<float> setValue)
        {
            this.startValue = startValue;
            this.finalValue = finalValue;
            this.OnTimeChange = setValue;
            Start(seconds);
        }
        public ValueTimeChanger(float startValue, float finalValue, float seconds, Action<float> setValue, System.Func<bool> breakCondition)
        {
            this.startValue = startValue;
            this.finalValue = finalValue;
            this.OnTimeChange = setValue;
            this.BreakCondition = breakCondition;
            Start(seconds);
        }
        public ValueTimeChanger(float startValue, float finalValue, float seconds, AnimationCurve curve)
        {
            this.startValue = startValue;
            this.finalValue = finalValue;
            this.curve = curve;
            Start(seconds);
        }
        public ValueTimeChanger(float startValue, float finalValue, float seconds, AnimationCurve curve, Action<float> setValue)
        {
            this.startValue = startValue;
            this.finalValue = finalValue;
            this.curve = curve;
            this.OnTimeChange = setValue;
            Start(seconds);
        }
        public ValueTimeChanger(float startValue, float finalValue, float seconds, AnimationCurve curve, Action<float> setValue, Action onEnd)
        {
            this.curve = curve;
            this.startValue = startValue;
            this.finalValue = finalValue;
            this.OnTimeChange = setValue;
            this.OnChangeEnd = onEnd;
            Start(seconds);
        }
        public ValueTimeChanger(float startValue, float finalValue, float seconds, AnimationCurve curve, Action<float> setValue, Action onEnd, System.Func<bool> breakCondition, bool invokeEndAtBreak)
        {
            this.curve = curve;
            this.startValue = startValue;
            this.finalValue = finalValue;
            this.OnTimeChange = setValue;
            this.OnChangeEnd = onEnd;
            this.BreakCondition = breakCondition;
            InvokeEndAtBreak = invokeEndAtBreak;
            Start(seconds);
        }
        #endregion methods
    }
}