using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Universal.UI
{
    public class CustomButton : CursorChanger
    {
        #region fields & properties
        public UnityEvent OnClickEvent => onClickEvent;
        [SerializeField] private UnityEvent onClickEvent;
        public UnityAction OnClicked;
        public UnityEvent OnHoverEvent => onHoverEvent;
        [SerializeField] private UnityEvent onHoverEvent;
        public UnityEvent OnExitEvent => onExitEvent;
        [SerializeField] private UnityEvent onExitEvent;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            OnEnter += HoverUI;
            OnExit += ExitUI;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            OnEnter -= HoverUI;
            OnExit -= ExitUI;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!IsButtonLeft(eventData)) return;
            base.OnPointerClick(eventData);
            onClickEvent?.Invoke();
            OnClicked?.Invoke();
        }
        private void HoverUI()
        {
            OnHoverEvent?.Invoke();
        }
        private void ExitUI()
        {
            OnExitEvent?.Invoke();
        }
        #endregion methods
    }
}