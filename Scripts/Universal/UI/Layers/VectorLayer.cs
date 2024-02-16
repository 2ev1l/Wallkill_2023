using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI.Layers
{
    [System.Serializable]
    public class VectorLayer : SmoothLayer
    {
        #region fields & properties
        public Vector3 OutVector => outVector;
        private Vector3 outVector;
        #endregion fields & properties

        #region methods
        public void ChangeVector(Vector3 start, Vector3 end, float seconds, System.Action<Vector3> setValue, System.Action onEnd, System.Func<bool> breakCondition, bool invokeEndAtBreak = true)
        {
            if (Context == null) return;
            Context.StartCoroutine(ChangeVectorSmooth(start, end, seconds, setValue, onEnd, breakCondition, true));
        }
        public IEnumerator ChangeVectorSmooth(Vector3 start, Vector3 end, float seconds, System.Action<Vector3> setValue, System.Action onEnd, System.Func<bool> breakCondition, bool invokeEndAtBreak)
        {
            AnimationCurve curve = vtc == null ? ValueTimeChanger.DefaultCurve : vtc.Curve;
            outVector = start;
            vtc?.Break();
            vtc = new(0, 1, seconds, curve, x => outVector = Vector3.Lerp(start, end, x), onEnd , breakCondition, invokeEndAtBreak);
            while (!vtc.IsEnded)
            {
                if (breakCondition != null && breakCondition.Invoke()) yield break;
                setValue.Invoke(outVector);
                yield return vtc.WaitForTimeUnit();
            }
        }
        #endregion methods
    }
}