using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Settings.PostEffects
{
    [System.Serializable]
    public class SplitToningData
    {
        #region fields & properties
        /// <summary>
        /// -100..100f
        /// Gamma
        /// </summary>
        public float Balance => balance;
        [Range(-100, 100)][SerializeField] private float balance = 0f;
        #endregion fields & properties

        #region methods
        public SplitToningData() { }
        public SplitToningData(float balance)
        {
            this.balance = balance;
        }

        #endregion methods
    }
}