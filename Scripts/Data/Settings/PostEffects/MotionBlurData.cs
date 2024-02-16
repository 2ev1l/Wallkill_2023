using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Settings.PostEffects
{
    [System.Serializable]
    public class MotionBlurData
    {
        #region fields & properties
        /// <summary>
        /// 0..1f
        /// </summary>
        public float Intensity => intensity;
        [Range(0f, 1f)][SerializeField] private float intensity = 0.2f;
        #endregion fields & properties

        #region methods
        public MotionBlurData() { }
        public MotionBlurData(float intensity)
        {
            this.intensity = intensity;
        }
        #endregion methods
    }
}