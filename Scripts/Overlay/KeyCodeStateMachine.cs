using Data.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal;
using Universal.UI;
using EditorCustom.Attributes;
using Data.Settings;

namespace Overlay
{
    public class KeyCodeStateMachine : DefaultStateMachine, IInitializable
    {
        #region fields & properties
        [SerializeField] private bool doubleClickDisableState = true;
        [SerializeField] private bool escapeClosesNonDefaultState = true;

        private List<KeyCodeStateChange> keyStates;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            Init();
        }
        public void Init()
        {
            keyStates = new();
            foreach (var el in Context.States.Cast<KeyCodeStateChange>())
            {
                if (el == null) continue;
                keyStates.Add(el);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InputController.OnKeyDown += TryCatchKey;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            InputController.OnKeyDown -= TryCatchKey;
        }
        private void TryCatchKey(KeyCode key)
        {
            StateChange neededState = keyStates.Find(x => x.ActivateKey == key);
            if (neededState == null) return;
            StateChange lastState = Context.CurrentState;
            ApplyDefaultState();
            if (lastState != Context.DefaultState && key == KeyCode.Escape && escapeClosesNonDefaultState) return; //for [escape] only
            if (lastState == neededState && doubleClickDisableState) return; //for double click
            Context.TryApplyState(neededState);
        }
        #endregion methods
    }
}