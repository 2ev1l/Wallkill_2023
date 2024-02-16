using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI.Layers
{
    [System.Serializable]
    public class SmoothLayer : DefaultLayer<float>
    {
        #region fields & properties
        public ValueTimeChanger VTC => vtc;
        [SerializeField] protected ValueTimeChanger vtc;
        protected MonoBehaviour Context => SingleGameInstance.Instance;
        #endregion fields & properties

        #region methods
        public virtual void ChangeLayerWeight(float startWeight, float finalWeight, float seconds, System.Action<float> setValue)
        {
            Context.StartCoroutine(ChangeLayerWeightSmooth(startWeight, finalWeight, seconds, setValue, null, null));
        }
        public virtual void ChangeLayerWeight(float startWeight, float finalWeight, float seconds, System.Action<float> setValue, System.Action onEnd)
        {
            Context.StartCoroutine(ChangeLayerWeightSmooth(startWeight, finalWeight, seconds, setValue, onEnd, null));
        }
        public virtual void ChangeLayerWeight(float startWeight, float finalWeight, float seconds, System.Action<float> setValue, System.Action onEnd, System.Func<bool> breakCondition, bool invokeEndAtBreak = false)
        {
            Context.StartCoroutine(ChangeLayerWeightSmooth(startWeight, finalWeight, seconds, setValue, onEnd, breakCondition, invokeEndAtBreak));
        }
        public virtual IEnumerator ChangeLayerWeightSmooth(float startWeight, float finalWeight, float seconds, System.Action<float> setValue, System.Action onEnd, System.Func<bool> breakCondition, bool invokeEndAtBreak = false)
        {
            AnimationCurve curve = vtc == null ? ValueTimeChanger.DefaultCurve : vtc.Curve;
            vtc?.Break();
            vtc = new(startWeight, finalWeight, seconds, curve, setValue, onEnd, breakCondition, invokeEndAtBreak);
            while (!vtc.IsEnded)
            {
                if (breakCondition != null && breakCondition.Invoke()) yield break;
                value = vtc.Value;
                yield return vtc.WaitForTimeUnit();
            }
            value = vtc.Value;
        }
        #endregion methods
    }
}