using Animation;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Tutorial
{
    public class HelpMessages : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnMessageHidden;
        public UnityAction OnMessageShown;

        [SerializeField] private ObjectMove panelMover;
        [SerializeField] private TextMeshProUGUI panelText;
        [SerializeField] private Image closeButtonRaycast;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private bool isMessageShown = false;
        #endregion fields & properties

        #region methods
        private void OnDestroy()
        {
            OnMessageHidden = null;
            OnMessageShown = null;
        }
        private void OnDisable()
        {
            if (isMessageShown)
                TryHideMessageInstantly();
        }
        [Button(nameof(Hide))]
        private void Hide() => TryHideMessage();
        public bool TryShowMessage(string text)
        {
            if (isMessageShown) return false;
            panelMover.MoveTo(1);
            isMessageShown = true;
            panelText.text = text;
            closeButtonRaycast.enabled = true;
            panelMover.gameObject.SetActive(true);
            OnMessageShown?.Invoke();
            return true;
        }
        [SerializedMethod]
        public void TryHideMessage()
        {
            if (!isMessageShown) return;
            closeButtonRaycast.enabled = false;
            panelMover.MoveTo(0);
            Invoke(nameof(InvokeHideActions), panelMover.MoveTime);
        }
        public void TryHideMessageInstantly()
        {
            if (!isMessageShown) return;
            closeButtonRaycast.enabled = false;
            InvokeHideActions();
        }

        private void InvokeHideActions()
        {
            isMessageShown = false;
            panelMover.gameObject.SetActive(false);
            OnMessageHidden?.Invoke();
        }

        #endregion methods
    }
}