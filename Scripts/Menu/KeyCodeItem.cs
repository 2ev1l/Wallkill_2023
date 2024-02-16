using UnityEngine;
using TMPro;
using Data.Interfaces;
using Data.Settings;
using Universal.UI;
using System.Linq;
using EditorCustom.Attributes;

namespace Menu
{
    public class KeyCodeItem : MonoBehaviour, IInitializable<KeyCodeInfo>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private CustomButton button;
        [SerializeField] private GameObject outlineUI;
        private static string defaultColor => "#FF4A00";
        private static string copyColor => "#FF0005";
        private KeyCodeInfo Context;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            button.OnEnter += EnableOutlineUI;
            button.OnExit += DisableOutlineUI;
            ControlsPanel.Instance.KeyCatcher.OnCatchStart += DisableUI;
            ControlsPanel.Instance.KeyCatcher.OnKeyReturns += EnableUI;
            ControlsPanel.Instance.KeyCatcher.OnKeyCatched += EnableUI;
            EnableUI();
        }
        private void OnDisable()
        {
            button.OnEnter -= EnableOutlineUI;
            button.OnExit -= DisableOutlineUI;
            ControlsPanel.Instance.KeyCatcher.OnCatchStart -= DisableUI;
            ControlsPanel.Instance.KeyCatcher.OnKeyReturns -= EnableUI;
            ControlsPanel.Instance.KeyCatcher.OnKeyCatched -= EnableUI;
        }
        public void Init(KeyCodeInfo value)
        {
            Context = value;
            EnableUI();
        }
        private void EnableUI(KeyCodeInfo key) => EnableUI();
        private void EnableUI()
        {
            LanguageLoader.GetTextByKeyCode(Context.Key);
            text.text = LanguageLoader.GetTextByKeyCode(Context.Key);
            button.enabled = true;
            outlineUI.SetActive(false);
            int sameKeys = ControlsPanel.Instance.CurrentKeys.Count(x => x.Key == Context.Key);
            ColorUtility.TryParseHtmlString(sameKeys > 1 ? copyColor : defaultColor, out Color col);
            text.color = col;
        }
        private void DisableUI()
        {
            button.enabled = false;
            DisableOutlineUI();
        }
        private void DisableOutlineUI()
        {
            outlineUI.SetActive(false);
        }
        private void EnableOutlineUI()
        {
            outlineUI.SetActive(true);
        }
        
        [SerializedMethod]
        public void ChangeKey()
        {
            ControlsPanel.Instance.KeyCatcher.CatchKey(Context);
            EnableOutlineUI();
        }
        #endregion methods
    }
}