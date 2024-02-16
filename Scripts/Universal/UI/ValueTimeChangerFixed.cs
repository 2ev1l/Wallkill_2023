using System;
using UnityEngine;
using EditorCustom.Attributes;
using System.Collections;

namespace Universal.UI
{
    [System.Serializable]
    public class ValueTimeChangerFixed : ValueTimeChanger
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override float GetTimeUnit()
        {
            return UnityEngine.Time.fixedDeltaTime;
        }
        public override IEnumerator WaitForTimeUnit()
        {
            yield return new WaitForFixedUpdate();
        }
        public ValueTimeChangerFixed(float startValue, float finalValue, float seconds) : base(startValue, finalValue, seconds) { }
        public ValueTimeChangerFixed(float startValue, float finalValue, float seconds, Func<bool> breakCondition) : base(startValue, finalValue, seconds, breakCondition) { }
        public ValueTimeChangerFixed(float startValue, float finalValue, float seconds, Action<float> setValue) : base(startValue, finalValue, seconds, setValue) { }
        public ValueTimeChangerFixed(float startValue, float finalValue, float seconds, AnimationCurve curve) : base(startValue, finalValue, seconds, curve) { }
        public ValueTimeChangerFixed(float startValue, float finalValue, float seconds, Action<float> setValue, Func<bool> breakCondition) : base(startValue, finalValue, seconds, setValue, breakCondition) { }
        public ValueTimeChangerFixed(float startValue, float finalValue, float seconds, AnimationCurve curve, Action<float> setValue) : base(startValue, finalValue, seconds, curve, setValue) { }
        public ValueTimeChangerFixed(float startValue, float finalValue, float seconds, AnimationCurve curve, Action<float> setValue, Action onEnd) : base(startValue, finalValue, seconds, curve, setValue, onEnd) { }
        public ValueTimeChangerFixed(float startValue, float finalValue, float seconds, AnimationCurve curve, Action<float> setValue, Action onEnd, Func<bool> breakCondition, bool invokeEndAtBreak) : base(startValue, finalValue, seconds, curve, setValue, onEnd, breakCondition, invokeEndAtBreak) { }
        #endregion methods
    }
}