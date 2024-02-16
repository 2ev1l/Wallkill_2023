using System.Collections;
using UnityEngine;
using EditorCustom.Attributes;

namespace Universal
{
    [System.Serializable]
    public class TimerFixed : Timer
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
        #endregion methods
    }
}