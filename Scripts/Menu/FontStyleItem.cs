using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Menu
{
    public class FontStyleItem : TextItem<int>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            FontStyles fontParam = (FontStyles)param;
            Text.text = fontParam switch
            {
                FontStyles.Normal => "N",
                FontStyles.Bold => "B",
                FontStyles.Italic => "I",
                FontStyles.Bold | FontStyles.Italic => "B&I",
                FontStyles.UpperCase => "UC",
                FontStyles.SmallCaps => "Sc",
                FontStyles.Bold | FontStyles.SmallCaps => "B&Sc",
                _ => throw new System.NotImplementedException("Font Style")
            };
            Text.fontStyle = fontParam;
            value = (int)param;
        }
        #endregion methods
    }
}