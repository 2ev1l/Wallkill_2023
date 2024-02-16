using Data.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI.Triggers
{
    public class AnimatorTrigger : DefaultTrigger
    {
        #region fields & properties
        [SerializeField] private Animator animator;

        [Header("States")]
        [SerializeField] private bool changeState = true;
        [SerializeField] private string stateEnter = "";
        [SerializeField] private string stateExit = "Default State";

        [Header("Properties")]
        [SerializeField] private bool changeProperty = true;
        [SerializeField] private AnimatorPropertyType propertyType;
        [SerializeField] private string propertyName = "";

        [SerializeField] private string propertyEnterValue = "0";
        [SerializeField] private string propertyExitValue = "1";
        #endregion fields & properties

        #region methods
        private void PlayAnimatorOnEnter()
        {
            animator.Play(stateEnter);
        }
        private void PlayAnimatorOnExit()
        {
            animator.Play(stateExit);
        }
        private void ChangePropertyOnEnter()
        {
            propertyType.ExposeToAnimator(animator, propertyName, propertyEnterValue);
        }
        private void ChangePropertyOnExit()
        {
            propertyType.ExposeToAnimator(animator, propertyName, propertyExitValue);
        }

        protected override void OnEnterTriggered()
        {
            if (changeState) PlayAnimatorOnEnter();
            if (changeProperty) ChangePropertyOnEnter();
        }

        protected override void OnExitTriggered()
        {
            if (changeState) PlayAnimatorOnExit();
            if (changeProperty) ChangePropertyOnExit();
        }
        #endregion methods
    }
}