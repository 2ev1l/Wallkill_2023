using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Universal.UI;

namespace Universal
{
    public abstract class ShowHelp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region fields
        public UnityAction OnPanelShowed;
        public bool PanelState => HelpUpdater.State;
        protected abstract HelpUpdater HelpUpdater { get; }
        #endregion fields

        #region methods
        public void OnPointerEnter(PointerEventData eventData)
        {
            OpenPanel(eventData);
        }
        public virtual void OpenPanel(PointerEventData eventData)
        {
            HelpUpdater.OpenPanel(Vector3.zero);
        }
        public void OnPointerExit(PointerEventData eventData) => HidePanel();
        protected virtual void OnDisable()
        {
            HidePanel();
        }
        public void HidePanel()
        {
            HelpUpdater.HidePanel();
        }
        #endregion methods
    }
}