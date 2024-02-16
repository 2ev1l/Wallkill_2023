using UnityEngine;
using Universal.UI;
using EditorCustom.Attributes;

namespace Menu
{
    public class TextChanger : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private LanguageLoader languageLoader;
        [SerializeField] private string textBefore;
        [SerializeField] private string textAfter;
        [SerializeField] private bool changeOnAwake = false;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            if (!changeOnAwake) return;
            ChangeText();
        }
        [SerializedMethod]
        public void ChangeText()
        {
            languageLoader.AddTextBefore(textBefore);
            languageLoader.AddTextAfter(textAfter);
        }
        [SerializedMethod]
        public void ResetText()
        {
            languageLoader.RemoveAdditionalText();
        }
        #endregion methods
    }
}