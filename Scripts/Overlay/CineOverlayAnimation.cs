using EditorCustom.Attributes;
using Game.CameraView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overlay
{
    public class CineOverlayAnimation : CineAnimation
    {
        #region fields & properties
        [Title("Overlay")]
        [SerializeField] private DelayUI delayUI;
        [SerializeField] private bool openAtDelay = true;
        [SerializeField] private bool closeAtReset = true;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public override void ResetAnimation()
        {
            base.ResetAnimation();
            delayUI.RevokeOpening();
            if (closeAtReset)
                delayUI.TryCloseImmediately();
        }
        protected override void OnStartAnimation()
        {
            base.OnStartAnimation();
            if (openAtDelay)
                delayUI.OpenAtDelay();

            delayUI.RevokeTryClosing();
        }
        protected override void OnEndAnimation()
        {
            base.OnEndAnimation();
            if (!openAtDelay)
                delayUI.OpenImmediately();
        }
        #endregion methods
    }
}