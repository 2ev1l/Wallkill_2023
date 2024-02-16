using EditorCustom.Attributes;
using UnityEngine;
using Universal.UI.Layers;

namespace Animation
{
    /// <summary>
    /// Using fixed update for physics callbacks
    /// </summary>
    public class RotateAround : RotateAroundBase
    {
        #region fields & properties
        protected override float DeltaTime => Time.fixedDeltaTime;
        #endregion fields & properties

        #region methods
        private void FixedUpdate()
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