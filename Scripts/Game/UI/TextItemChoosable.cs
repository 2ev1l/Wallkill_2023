using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public abstract class TextItemChoosable<T> : TextItemDescription where T : TextItemChoosable<T>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI raycastText;
        [SerializeField] private CustomButton customButton;
        [SerializeField] private GameObject choosedUI;
        private static T choosedItemObject = null;
        private static int choosedItemValue = -1;
        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            base.OnListUpdate(param);

            if (IsThisChoosed())
            {
                choosedUI.SetActive(true);
            }
            else
            {
                EnableButton();
            }
        }
        private void OnEnable()
        {
            customButton.OnClicked += ChooseThisItem;
            customButton.OnEnter += OnEnter;
            customButton.OnExit += OnExit;
        }
        private void OnDisable()
        {
            customButton.OnClicked -= ChooseThisItem;
            customButton.OnEnter -= OnEnter;
            customButton.OnExit -= OnExit;
        }
        private void OnEnter()
        {
            if (IsThisChoosed()) return;
            choosedUI.SetActive(true);
        }
        private void OnExit()
        {
            if (IsThisChoosed()) return;
            choosedUI.SetActive(false);
        }
        private void ChooseThisItem()
        {
            if (choosedItemObject != null)
            {
                choosedItemObject.EnableButton();
            }
            choosedItemObject = (T)this;
            choosedItemValue = choosedItemObject.value;
            DisableButton();
            choosedUI.SetActive(true);
            OnItemChoosed();
        }
        private bool IsThisChoosed() => choosedItemObject == this && choosedItemObject.value == choosedItemValue;
        private void EnableButton()
        {
            raycastText.raycastTarget = true;
            choosedUI.SetActive(false);
        }
        private void DisableButton()
        {
            raycastText.raycastTarget = false;
            choosedUI.SetActive(false);
        }
        /// <summary>
        /// Base UI is already set.
        /// </summary>
        protected abstract void OnItemChoosed();
        public void DeselectItem()
        {
            choosedItemObject = null;
            choosedItemValue = -1;
            EnableButton();
        }
        #endregion methods
    }
}