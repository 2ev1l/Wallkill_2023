using EditorCustom.Attributes;
using Game.Rooms;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Game.CameraView
{
    public class CineAnimation : MonoBehaviour
    {
        #region fields & properties
        [HelpBox("You need to invoke StartAnimation & ResetAnimation manually")]
        [SerializeField] public UnityEvent OnAnimationStart;
        [SerializeField] public UnityEvent OnAnimationEnd;
        public UnityAction OnAnimationStartAction;
        public UnityAction OnAnimationEndAction;

        private CameraPosition CameraPosition => InstancesProvider.Instance.CameraPosition;
        private CameraCrop CameraCrop => InstancesProvider.Instance.CameraCrop;
        private Player.Animations PlayerAnimations => InstancesProvider.Instance.PlayerAnimations;
        private Player.Moving PlayerMoving => InstancesProvider.Instance.PlayerMoving;
        private Player.Attack PlayerAttack => InstancesProvider.Instance.PlayerAttack;
        [SerializeField] private Transform startPlayerForwardAxis;
        [SerializeField] private Transform finalPlayerPosition;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void StartAnimation()
        {
            StartCoroutine(WaitForAnimation());
        }
        [SerializedMethod]
        public virtual void ResetAnimation()
        {
            CameraCrop.enabled = true;
            CameraPosition.DisableFollow();
            PlayerAnimations.EnableRotateWithCamera();
        }
        private IEnumerator WaitForAnimation()
        {
            OnAnimationStart?.Invoke();
            OnAnimationStartAction?.Invoke();
            OnStartAnimation();

            CameraCrop.enabled = false;
            if (PlayerAttack.IsAiming)
                PlayerAttack.TryEndAiming();
            CameraPosition.FollowPlayer();
            PlayerAnimations.ChangePlayerForawrdAxis(startPlayerForwardAxis);
            PlayerAnimations.DisableRotateWithCamera();
            yield return PlayerMoving.WaitForControlledMove(finalPlayerPosition, false, true, false);

            OnAnimationEnd?.Invoke();
            OnAnimationEndAction?.Invoke();
            OnEndAnimation();
        }
        protected virtual void OnStartAnimation() { }
        protected virtual void OnEndAnimation() { }
        #endregion methods
    }
}