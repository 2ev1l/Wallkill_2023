using EditorCustom.Attributes;
using Game.DataBase;
using Menu;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public abstract class TextItemDescription : TextItem<int>
    {
        #region fields & properties
        public bool UseDescription => useDescription;
        [SerializeField] private bool useDescription = true;
        public TextMeshProUGUI DescriptionText => descriptionText;
        [SerializeField][DrawIf(nameof(useDescription), true)] private TextMeshProUGUI descriptionText;
        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            this.value = param;
            Text.text = GetName();
            if (UseDescription)
                DescriptionText.text = GetDescription();
        }
        protected abstract string GetDescription();
        protected abstract string GetName();
        #endregion methods
    }
}