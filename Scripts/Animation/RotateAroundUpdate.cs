using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    /// <summary>
    /// Using default update for better UI
    /// </summary>
    public class RotateAroundUpdate : RotateAroundBase
    {
        #region fields & properties
        protected override float DeltaTime => Time.deltaTime;
        #endregion fields & properties

        #region methods
        private void Update()
        {
            TrySimulate();
        }
        [Button(nameof(Simulate))]
        protected override void Simulate()
        {
            base.Simulate();
        }
        #endregion methods
    }
}