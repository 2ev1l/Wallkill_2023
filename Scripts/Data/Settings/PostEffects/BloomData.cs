using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Settings.PostEffects
{
    [System.Serializable]
    public class BloomData
    {
        #region fields & properties
        /// <summary>
        /// 0..inf
        /// </summary>
        public float Threshold => threshold;
        [Min(0)][SerializeField] private float threshold = 1f;
        /// <summary>
        /// 0..inf
        /// </summary>
        public float Intensity => intensity;
        [Min(0)][SerializeField] private float intensity = 1f;
        #endregion fields & properties

        #region methods
        public BloomData() { }
        public BloomData(float threshold, float intensity) 
        {
            this.threshold = threshold;
            this.intensity = intensity;
        }
        #endregion methods
    }
}