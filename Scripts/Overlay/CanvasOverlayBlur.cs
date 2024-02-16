using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Overlay
{
    public class CanvasOverlayBlur : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private CanvasGroup overlayCanvasGroup;
        [SerializeField] private DefaultStateMachine overlayStatesMachine;
        [SerializeField][Range(0, 2)] private float secondsToBlur = 0.5f;
        private ValueTimeChanger vtc;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            overlayStatesMachine.Context.OnStateChanged += CheckOverlayState;
        }
        private void OnDisable()
        {
            overlayStatesMachine.Context.OnStateChanged -= CheckOverlayState;
        }
        private void CheckOverlayState(StateChange state)
        {
            if (state == overlayStatesMachine.Context.DefaultState)
                BlurDown();
            else
                BlurUp();
        }
        public void BlurUp() => Blur(true);
        public void BlurDown() => Blur(false);
        public void Blur(bool blurUp)
        {
            float finalValue = blurUp ? 1f : 0f;
            float startValue = overlayCanvasGroup.alpha;
            vtc = new(startValue, finalValue, secondsToBlur, x => overlayCanvasGroup.alpha = x, delegate { return overlayCanvasGroup == null; });
        }
        #endregion methods
    }
}