using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Settings.PostEffects
{
    [System.Serializable]
    public class WhiteBalanceData
    {
        #region fields & properties
        /// <summary>
        /// -100..100f
        /// Blue..Orange
        /// </summary>
        public float Temperature => temperature;
        [Range(-100, 100)][SerializeField] private float temperature = 0f;
        /// <summary>
        /// -100..100f
        /// Green..Violet
        /// </summary>
        public float Tint => tint;
        [Range(-100, 100)][SerializeField] private float tint = 0f;
        #endregion fields & properties

        #region methods
        public WhiteBalanceData() { }
        public WhiteBalanceData(float temperature, float tint)
        {
            this.temperature = temperature;
            this.tint = tint;
        }
        #endregion methods
    }
}