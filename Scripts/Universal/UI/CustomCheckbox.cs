using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Universal.UI
{
    public class CustomCheckbox : CursorChanger
    {
        #region fields & properties
        public UnityEvent OnActiveState => onActiveState;
        [SerializeField] private UnityEvent onActiveState;
        public UnityEvent OnDisableState => onDisableState;
        [SerializeField] private UnityEvent onDisableState;
        public UnityAction<bool> OnStateChanged;

        public UnityEvent OnHoverEvent => onHoverEvent;
        [SerializeField] private UnityEvent onHoverEvent;
        public UnityEvent OnExitEvent => onExitEvent;
        [SerializeField] private UnityEvent onExitEvent;

        [SerializeField][Required] private GameObject activeMark;
        public bool CurrentState
        {
            get => currentState;
            set => SetCurrentState(value);
        }
        [SerializeField] private bool currentState = false;
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

            CurrentState = !CurrentState;
        }
        private void SetCurrentState(bool value)
        {
            currentState = value;
            OnStateChanged?.Invoke(CurrentState);
            
            if (CurrentState) OnActiveState?.Invoke();
            else OnDisableState?.Invoke();
            UpdateUI();
        }
        private void UpdateUI()
        {
            activeMark.SetActive(CurrentState);
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
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (activeMark == null) return;
            UpdateUI();
        }
#endif //UNITY_EDITOR
    }
}