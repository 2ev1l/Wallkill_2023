using Data.Settings;
using UnityEngine;
using Universal;
using Universal.UI;
using EditorCustom.Attributes;
using Data.Stored;

namespace Overlay
{
    public class KeyCodeStateChange : PanelStateChange
    {
        #region fields & properties
        public KeyCode ActivateKey
        {
            get
            {
                if (activateKey == KeyCode.None && !ignoreAll)
                    UpdateKey();
                return activateKey;
            }
        }
        [DrawIf(nameof(ignoreAll), false)][SerializeField] private KeyCodeDescription keyDescription;
        [SerializeField] private bool ignoreAll;

        [Header("Read Only")]
        [SerializeField][ReadOnly] private KeyCode activateKey = KeyCode.None;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            InputController.Instance.OnKeyUpdates += UpdateKey;
        }
        private void OnDisable()
        {
            InputController.Instance.OnKeyUpdates -= UpdateKey;
        }
        private void UpdateKey()
        {
            activateKey = InputController.FindKeyByDescription(keyDescription);
        }
        #endregion methods
    }
}