using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI
{
    public class DefaultStateMachine : MonoBehaviour
    {
        #region fields & properties
        public StateMachine Context => context;
        [SerializeField] private StateMachine context;
        [SerializeField] private bool resetOnStart = true;
        [SerializeField] private bool resetOnEnable = false;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            if (resetOnStart) Context.TryApplyDefaultState();
        }
        protected virtual void OnEnable()
        {
            if (resetOnEnable) Context.ApplyDefaultState();
        }
        protected virtual void OnDisable() { }
        [SerializedMethod]
        public virtual void ApplyState(int state) => Context.TryApplyState(state);
        [SerializedMethod]
        public virtual void ApplyState(StateChange state) => Context.TryApplyState(state);
        [SerializedMethod]
        public virtual void ApplyDefaultState() => Context.ApplyDefaultState();
        #endregion methods
    }
}