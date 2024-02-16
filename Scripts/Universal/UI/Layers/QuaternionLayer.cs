using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI.Layers
{
    [System.Serializable]
    public class QuaternionLayer : SmoothLayer
    {
        #region fields & properties
        public Quaternion OutQuaternion => outQuaternion;
        private Quaternion outQuaternion;
        #endregion fields & properties

        #region methods
        public void ChangeQuaternion(Quaternion start, Quaternion end, float seconds, System.Action<Quaternion> setValue, System.Action onEnd, System.Func<bool> breakCondition, bool invokeEndAtBreak = true)
        {
            Context.StartCoroutine(ChangeQuaternionSmooth(start, end, seconds, setValue, onEnd, breakCondition, invokeEndAtBreak));
        }
        public IEnumerator ChangeQuaternionSmooth(Quaternion start, Quaternion end, float seconds, System.Action<Quaternion> setValue, System.Action onEnd, System.Func<bool> breakCondition, bool invokeEndAtBreak)
        {
            AnimationCurve curve = vtc == null ? ValueTimeChanger.DefaultCurve : vtc.Curve;
            outQuaternion = start;
            vtc?.Break();
            vtc = new(0, 1, seconds, curve, x => outQuaternion = Quaternion.Lerp(start, end, x), onEnd , breakCondition, invokeEndAtBreak);
            while (!vtc.IsEnded)
            {
                if (breakCondition != null && breakCondition.Invoke()) yield break;
                setValue.Invoke(outQuaternion);
                yield return vtc.WaitForTimeUnit();
            }
        }
        #endregion methods
    }
}