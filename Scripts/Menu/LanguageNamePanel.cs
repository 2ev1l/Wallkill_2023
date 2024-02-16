using Data.Settings;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal;
using EditorCustom.Attributes;

namespace Menu
{
    public class LanguageNamePanel : SettingsPanel<LanguageSettings>
    {
        #region fields & properties
        protected StringItemList LanguageList => languageList;
        [SerializeField] private StringItemList languageList;
        protected override LanguageSettings Context => SettingsData.Data.LanguageSettings;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            languageList.ItemsList.OnPageSwitched += SaveSettings;
        }
        protected virtual void OnDisable()
        {
            languageList.ItemsList.OnPageSwitched -= SaveSettings;
        }
        protected override LanguageSettings GetSettings()
        {
            string language = languageList.CurrentPageItems[0].Value;
            int cSpacing = Context.CharacterSpacing;
            int lSpacing = Context.LineSpacing;
            int wSpacing = Context.WordSpacing;
            FontStyles fontStyle = Context.FontStyle;
            return new(language, lSpacing, wSpacing, cSpacing, fontStyle);
        }
        
        [SerializedMethod]
        public override void SaveSettings()
        {
            SettingsData.Data.LanguageSettings = GetSettings();
        }

        protected override void UpdateAllLists()
        {
            UpdateLanguageList();
        }
        private void UpdateLanguageList()
        {
            List<string> languages = SavingUtils.GetLanguageNames();
            languageList.UpdateListData(languages);
            languageList.ItemsList.ShowAt(Context.ChoosedLanguage);
        }
        #endregion methods
    }
}