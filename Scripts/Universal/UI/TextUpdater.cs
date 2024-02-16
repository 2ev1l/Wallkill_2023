using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Data;
using System.Linq;
using UnityEngine.Events;
using Data.Settings;
using TMPro;

namespace Universal.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextUpdater : MonoBehaviour
    {
        #region fields & properties
        protected TextMeshProUGUI Text
        {
            get
            {
                text = text != null ? text : GetComponent<TextMeshProUGUI>();
                return text;
            }
        }
        private TextMeshProUGUI text;

        [field: SerializeField] public bool UpdateFont { get; set; } = true;
        [field: SerializeField] public bool UpdateFontStyle { get; set; } = true;
        [field: SerializeField] public bool UpdateSpacing { get; set; } = true;
        private static LanguageSettings Context => SettingsData.Data.LanguageSettings;
        #endregion fields & properties

        #region methods
        private void Start() => UpdateText();
        public void UpdateText()
        {
            if (UpdateFontStyle) SetStyle();
            if (UpdateSpacing) SetSpacing();
        }
        private void SetStyle()
        {
            Text.fontStyle = Context.FontStyle;
        }
        private void SetSpacing()
        {
            Text.lineSpacing = Context.LineSpacing;
            Text.characterSpacing = Context.CharacterSpacing;
            Text.wordSpacing = Context.WordSpacing;
        }
        #endregion methods
    }
}