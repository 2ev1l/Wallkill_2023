using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Universal.UI;

namespace WeakSoul
{
    public class ShowTextHelp : ShowHelp
    {
        #region fields & properties
        protected override HelpUpdater HelpUpdater => HelpTextUpdater.Instance;
        public int Id { get => id; set => id = value; }
        [SerializeField] private int id = 0;
        public TextType TextType { get => textType; set => textType = value; }
        [SerializeField] private TextType textType = TextType.Help;
        public bool ReverseX { get => reverseX; set => reverseX = value; }
        [SerializeField] private bool reverseX = false;
        public string AdditionalText { get => additionalText; set => additionalText = value; }
        private string additionalText = "";
        #endregion fields & properties

        #region methods
        public override void OpenPanel(PointerEventData eventData)
        {
            if (Id < 0) return;
            HelpTextUpdater.Instance.OpenPanel(eventData.pointerCurrentRaycast.worldPosition, Id, TextType, AdditionalText, ReverseX);
            OnPanelShowed?.Invoke();
        }
        #endregion methods
    }
}