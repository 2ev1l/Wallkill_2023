using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Settings.PostEffects
{
    [System.Serializable]
    public class VignetteData
    {
        #region fields & properties
        /// <summary>
        /// 0..1f
        /// </summary>
        public float Intensity => intensity;
        [Range(0f, 1f)][SerializeField] private float intensity = 0.15f;
        /// <summary>
        /// 0..1f
        /// </summary>
        public float Smoothness => smoothness;
        [Range(0f, 1f)][SerializeField] private float smoothness = 1f;
        #endregion fields & properties

        #region methods
        public VignetteData() { }
        public VignetteData(float intensity, float smoothness)
        {
            this.intensity = intensity;
            this.smoothness = smoothness;
        }
        #endregion methods
    }
}