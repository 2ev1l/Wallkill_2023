using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.DataBase
{
    [System.Serializable]
    public class LanguageInfo
    {
        #region fields & properties
        [SerializeField][Min(-1)] private int id;
        [SerializeField] private TextType textType;
        public string Text => LanguageLoader.GetTextByType(textType, id);
        public string RawText => textType.GetRawText(id);
        #endregion fields & properties

        #region methods
        public LanguageInfo() { }
        public LanguageInfo(int id, TextType textType)
        {
            this.id = id;
            this.textType = textType;
        }
        #endregion methods
    }
}