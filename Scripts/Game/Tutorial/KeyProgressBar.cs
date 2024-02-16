using Data.Settings;
using Data.Stored;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI;

namespace Game.Tutorial
{
    public class KeyProgressBar : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnKeyFilled;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private TextMeshProUGUI keyText;

        [Title("Settings")]
        [SerializeField] private KeyCodeDescription keyDescription;
        [SerializeField][Min(0)] private float maxHoldTime = 1f;
        [SerializeField] private bool resetTimeOnDisable = true;
        [SerializeField] private bool disableOnFilled = true;
        private float currentHoldTime;
        private KeyCodeInfo KeyInfo
        {
            get
            {
                keyInfo ??= SettingsData.Data.KeyCodeSettings.GetKeys().Find(x => x.Description == keyDescription);
                return keyInfo;
            }
        }
        private KeyCodeInfo keyInfo;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            KeyInfo.OnKeyCodeChanged += ChangeKeyCodeText;
            InputController.OnKeyHold += CheckKey;
            ChangeKeyCodeText(KeyInfo.Key);
        }
        private void OnDisable()
        {
            KeyInfo.OnKeyCodeChanged -= ChangeKeyCodeText;
            InputController.OnKeyHold -= CheckKey;

            if (resetTimeOnDisable) currentHoldTime = 0;
        }
        private void ChangeKeyCodeText(KeyCode value)
        {
            keyText.text = LanguageLoader.GetTextByKeyCode(value);
        }
        private void SetCurrentHoldTime(float value)
        {
            currentHoldTime = value;
            progressBar.SetValue(currentHoldTime);
            if (currentHoldTime >= maxHoldTime)
            {
                OnKeyFilled?.Invoke();
                if (disableOnFilled)
                    this.enabled = false;
            }
        }
        private void CheckKey(KeyCode key)
        {
            if (key != KeyInfo.Key) return;
            SetCurrentHoldTime(currentHoldTime + Time.deltaTime);
        }
        #endregion methods
    }
}