using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI.Layers
{
    [System.Serializable]
    public class ColorLayer : SmoothLayer
    {
        #region fields & properties
        public Color OutColor => outColor;
        private Color outColor;
        #endregion fields & properties

        #region methods
        public void ChangeColor(Color start, Color end, float seconds, System.Action<Color> setValue, System.Action onEnd, System.Func<bool> breakCondition)
        {
            Context.StartCoroutine(ChangeColorSmooth(start, end, seconds, setValue, onEnd, breakCondition));
        }
        public IEnumerator ChangeColorSmooth(Color start, Color end, float seconds, System.Action<Color> setValue, System.Action onEnd, System.Func<bool> breakCondition)
        {
            AnimationCurve curve = vtc == null ? ValueTimeChanger.DefaultCurve : vtc.Curve;
            outColor = start;
            vtc?.Break();
            vtc = new(0, 1, seconds, curve, x => outColor = Color.Lerp(start, end, x), onEnd + delegate { setValue.Invoke(outColor); }, breakCondition, true);
            while (!vtc.IsEnded)
            {
                if (breakCondition != null && breakCondition.Invoke()) yield break;
                setValue.Invoke(outColor);
                yield return CustomMath.WaitAFrame();
            }
        }
        #endregion methods
    }
}