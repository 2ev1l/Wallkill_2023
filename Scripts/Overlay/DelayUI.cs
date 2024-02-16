using UnityEngine;
using Universal.UI;
using EditorCustom.Attributes;
using Game.Rooms;

namespace Overlay
{
    public class DelayUI : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private bool useInstances = false;
        private DefaultStateMachine OverlayStateMachine => useInstances ? InstancesProvider.Instance.OverlayStateMachine : overlayStateMachine;
        [SerializeField][DrawIf(nameof(useInstances), false)] private DefaultStateMachine overlayStateMachine;
        [SerializeField][DrawIf(nameof(useInstances), false)] private StateChange overlayState;
        [SerializeField][DrawIf(nameof(useInstances), true)] private int overlayStateId;

        [Header("Settings")]
        [SerializeField][Range(0, 3)] private float delaySeconds = 1f;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void OpenAtDelay() => Invoke(nameof(OpenImmediately), delaySeconds);
        [SerializedMethod]
        public void RevokeOpening() => CancelInvoke(nameof(OpenImmediately));
        [SerializedMethod]
        public void OpenImmediately()
        {
            if (useInstances)
            {
                OverlayStateMachine.ApplyState(overlayStateId);
            }
            else
            {
                OverlayStateMachine.ApplyState(overlayState);
            }
        }

        [SerializedMethod]
        public void TryCloseAtDelay() => Invoke(nameof(TryCloseImmediately), delaySeconds);
        [SerializedMethod]
        public void RevokeTryClosing() => CancelInvoke(nameof(TryCloseImmediately));
        [SerializedMethod]
        public void TryCloseImmediately()
        {
            if (OverlayStateMachine.Context.CurrentState != overlayState) return;
            CloseImmediately();
            return;
        }
        private void CloseImmediately()
        {
            if (useInstances)
            {
                OverlayStateMachine.ApplyDefaultState();
            }
            else
            {
                OverlayStateMachine.ApplyDefaultState();
            }
        }
        #endregion methods
    }
}