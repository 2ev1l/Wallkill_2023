using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.UI.Triggers
{
    public class EventTrigger : DefaultTrigger
    {
        #region fields & properties
        [SerializeField] private UnityEvent eventsOnEnter;
        [SerializeField] private UnityEvent eventsOnExit;

        public void SimulateEnter() => OnEnterTriggered();
        public void SimulateExit() => OnExitTriggered();

        protected override void OnEnterTriggered()
        {
            eventsOnEnter?.Invoke();
        }
        protected override void OnExitTriggered()
        {
            eventsOnExit?.Invoke();
        }
        #endregion fields & properties

        #region methods
        #endregion methods
    }
}