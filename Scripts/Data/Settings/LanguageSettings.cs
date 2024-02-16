using Data.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Settings
{
    [System.Serializable]
    public class LanguageSettings: ICloneable<LanguageSettings>
    {
        #region fields & properties
        
        [SerializeField] private string choosedLanguage = "English";

        [SerializeField] private int lineSpacing = 0;
        [SerializeField] private int wordSpacing = 0;
        [SerializeField] private int characterSpacing = 0;

        [SerializeField] private FontStyles fontStyle = FontStyles.Normal;


        public string ChoosedLanguage { get => choosedLanguage;  }
        public int LineSpacing { get => lineSpacing;  }
        public int WordSpacing { get => wordSpacing; }
        public int CharacterSpacing { get => characterSpacing; }
        public FontStyles FontStyle { get => fontStyle; }
        #endregion fields & properties

        #region methods
        public void ResetLanguage() => choosedLanguage = "English";
        public LanguageSettings() { }
        public LanguageSettings(string choosedLanguage, int lineSpacing, int wordSpacing, int characterSpacing, FontStyles fontStyle)
        {
            this.choosedLanguage = choosedLanguage;
            this.lineSpacing = lineSpacing;
            this.wordSpacing = wordSpacing;
            this.characterSpacing = characterSpacing;
            this.fontStyle = fontStyle;
        }
        public LanguageSettings Clone()
        {
            return new(choosedLanguage, lineSpacing, wordSpacing, characterSpacing, fontStyle);
        }
        #endregion methods
    }
}