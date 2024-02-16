using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.UI
{
    [System.Serializable]
    public class StateMachine
    {
        #region fields & properties
        public UnityAction<StateChange> OnStateChanged;
        public IReadOnlyList<StateChange> States => states;
        [SerializeField] protected List<StateChange> states = new();
        public StateChange DefaultState => (states.Count == 0) ? null : states[0];
        public StateChange CurrentState
        {
            get
            {
                TryApplyDefaultState();
                return currentState;
            }
            private set => SetCurrentStateValue(value);
        }
        private StateChange currentState;
        #endregion fields & properties

        #region methods
        private void SetCurrentStateValue(StateChange value)
        {
            currentState = value;
            states.ForEach(x => x.SetActive(CurrentState == x));
            OnStateChanged?.Invoke(CurrentState);
        }
        public virtual void ApplyDefaultState() => ApplyState(states[0]);
        public bool TryApplyDefaultState()
        {
            if (currentState != null) return false;
            SetCurrentStateValue(DefaultState);
            return true;
        }
        public virtual void TryApplyState(StateChange choosedState)
        {
            if (CurrentState != null && choosedState == CurrentState && CurrentState != states[0]) return;
            ApplyState(choosedState);
        }
        public virtual void TryApplyState(int stateId) => TryApplyState(states[stateId]);
        protected virtual void ApplyState(StateChange choosedState) => CurrentState = choosedState;
        #endregion methods
    }
}