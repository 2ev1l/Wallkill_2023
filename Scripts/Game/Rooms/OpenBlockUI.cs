using Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.Rooms
{
    [DisallowMultipleComponent]
    public class OpenBlockUI : WallBlock
    {
        #region fields & properties
        [SerializeField] private Door relatedDoor;
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationRandomizer animationRandomizer;
        [SerializeField] private string animatorOpenStateName;
        [SerializeField] private string animatorCloseStateName;
        [SerializeField] private string animatorDefaultStateName = "Default State";
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            //opens door
            relatedDoor.OnDoorOpened += OpenObject;
            //closes door from <random> animation
            relatedDoor.OnDoorClosedUIOnly += CloseObjectFromStartAnimation;
            //closes door from opened state and returns <random> animation
            relatedDoor.OnDoorClosed += CloseObjectFromOpened;
            //starts <random> animation
            relatedDoor.OnCurrentRoomStart += StartBaseAnimation;
        }
        private void OnDisable()
        {
            relatedDoor.OnDoorOpened -= OpenObject;
            relatedDoor.OnDoorClosedUIOnly -= CloseObjectFromStartAnimation;
            relatedDoor.OnDoorClosed -= CloseObjectFromOpened;
            relatedDoor.OnCurrentRoomStart -= StartBaseAnimation;
        }
        private void StartBaseAnimation()
        {
            animationRandomizer.StartAnimationRandomly();
        }
        public IEnumerator WaitForOpen()
        {
            yield return animationRandomizer.WaitForReset(2);
            animator.Play(animatorOpenStateName);
        }
        private void OpenObject()
        {
            StartCoroutine(WaitForOpen());
        }
        private void CloseObjectFromOpened()
        {
            StartCoroutine(WaitForClose());
        }
        public void CloseObjectFromStartAnimation() => animationRandomizer.ResetAnimatorSmoothly();
        public IEnumerator WaitForClose()
        {
            float animatorSpeed = animator.speed;
            animator.Play(animatorCloseStateName, 0, 0);
            animator.speed = 1;
            float clipTime = 0f;
            bool isClipInitialized = true;
            try { clipTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length; }
            catch { isClipInitialized = false; }
            if (isClipInitialized)
                yield return new WaitForSeconds(clipTime);
            animator.speed = animatorSpeed;
            animator.Play(animatorCloseStateName, 0, 1);
            animationRandomizer.Play();
            yield break;
        }

        #endregion methods
    }
}