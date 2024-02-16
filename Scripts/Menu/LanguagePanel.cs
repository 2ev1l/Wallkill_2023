using Data.Settings;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Menu
{
    public class LanguagePanel : LanguageNamePanel
    {
        #region fields & properties
        [SerializeField] private IntItemList characterSpacingList;
        [SerializeField] private IntItemList lineSpacingList;
        [SerializeField] private IntItemList wordSpacingList;

        [SerializeField] private IntItemList fontStyleList;

        protected override LanguageSettings Context => SettingsData.Data.LanguageSettings;
        #endregion fields & properties

        #region methods
        protected override void OnEnable() { }
        protected override void OnDisable() { }
        protected override LanguageSettings GetSettings()
        {
            string language = LanguageList.CurrentPageItems[0].Value;
            int cSpacing = characterSpacingList.CurrentPageItems[0].Value;
            int lSpacing = lineSpacingList.CurrentPageItems[0].Value;
            int wSpacing = wordSpacingList.CurrentPageItems[0].Value;
            int fontStyle = fontStyleList.CurrentPageItems[0].Value;
            return new(language, lSpacing, wSpacing, cSpacing, (FontStyles)fontStyle);
        }

        protected override void UpdateAllLists()
        {
            base.UpdateAllLists();
            UpdateSpacingLists();
            UpdateFontStyleList();
        }

        private void UpdateSpacingLists()
        {
            List<int> possibleOptions = new() { -20, -18, -16, -14, -12, -10, -8, -6, -4, -2, 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
            characterSpacingList.UpdateListData(possibleOptions);
            characterSpacingList.ItemsList.ShowAt(Context.CharacterSpacing);

            lineSpacingList.UpdateListData(possibleOptions);
            lineSpacingList.ItemsList.ShowAt(Context.LineSpacing);

            wordSpacingList.UpdateListData(possibleOptions);
            wordSpacingList.ItemsList.ShowAt(Context.WordSpacing);
        }
        private void UpdateFontStyleList()
        {
            List<FontStyles> fontStyles = new() { FontStyles.Normal, FontStyles.Bold, FontStyles.Italic, FontStyles.Bold | FontStyles.Italic, FontStyles.UpperCase, FontStyles.SmallCaps, FontStyles.Bold | FontStyles.SmallCaps };
            List<int> fontStylesInt = fontStyles.Select(x => (int)x).ToList();
            fontStyleList.UpdateListData(fontStylesInt);
            fontStyleList.ItemsList.ShowAt((int)Context.FontStyle);
        }
        #endregion methods
    }
}