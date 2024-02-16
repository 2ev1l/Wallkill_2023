using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Animation
{
    public class AnimationRandomizer : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Animator animator;
        public string RandomStateName => stateName;
        [SerializeField] private string stateName;
        [SerializeField] private string defaultStateName = "Default State";
        [SerializeField] private int stateLayer = 0;

        [Header("Random Parameters")]
        [SerializeField] private bool useNormalizedStateTime = false;
        [SerializeField] private Vector2 normalizedStateTime = new(0, 1);
        [SerializeField] private Vector2 animatorSpeed = new(0.9f, 1.1f);
        [SerializeField] private bool useNegativeSpeed = true;
        [SerializeField] private bool enableOnAwake = true;
        private float normalizedTime = 0;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            if (!enableOnAwake) return;
            StartAnimationRandomly();
        }
        public void StartAnimationRandomly()
        {
            Randomize();
            Play();
        }
        public void Randomize()
        {
            normalizedTime = Random.Range(normalizedStateTime.x, normalizedStateTime.y);
            animator.speed = Random.Range(animatorSpeed.x, animatorSpeed.y);
            if (useNegativeSpeed)
                animatorSpeed *= CustomMath.GetRandomChance(50) ? -1 : 1;
        }
        public void Play()
        {
            animator.Play(stateName, stateLayer, useNormalizedStateTime ? normalizedTime : 0);
        }

        /// <summary>
        /// Just invokes <see cref="WaitForReset(float)"/>
        /// </summary>
        /// <param name="resetSpeed"></param>
        public void ResetAnimatorSmoothly(float resetSpeed = 1f) => StartCoroutine(WaitForReset(resetSpeed));
        /// <summary>
        /// Smoothly reset animator
        /// </summary>
        /// <param name="resetSpeed"></param>
        /// <returns></returns>
        public IEnumerator WaitForReset(float resetSpeed = 1f)
        {
            AnimatorClipInfo clipInfo = new();
            bool isClipInitialized = true;
            try { clipInfo = animator.GetCurrentAnimatorClipInfo(stateLayer)[0]; }
            catch { isClipInitialized = false; }
            if (!isClipInitialized)
            {
                yield break;
            }
            float clipTime = clipInfo.clip.length;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(stateLayer);
            float currentNormalizedTime = stateInfo.normalizedTime % 2;
            if (currentNormalizedTime > 1) currentNormalizedTime -= 1;
            float animatorSpeed = animator.speed;
            animator.speed = resetSpeed;

            float timeLasts = 0;
            if (currentNormalizedTime > 0.02f) timeLasts = (1 - currentNormalizedTime) * clipTime / animator.speed;

            yield return new WaitForSeconds(timeLasts);
            animator.speed = animatorSpeed;
            ResetAnimator();
        }
        /// <summary>
        /// Immediately reset animator
        /// </summary>
        public void ResetAnimator()
        {
            animator.Play(stateName, stateLayer, 0);
            animator.Play(defaultStateName);
        }
        #endregion methods
    }
}