using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI.Triggers
{
    public abstract class DefaultTrigger : MonoBehaviour
    {
        #region fields & properties
        protected TriggerCatcher TriggerCatcher => triggerCatcher;
        [Header("Default Settings")] [SerializeField] private TriggerCatcher triggerCatcher;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            TriggerCatcher.OnEnterTagSimple += OnEnterTriggered;
            TriggerCatcher.OnEnterComponentSimple += OnEnterTriggered;

            TriggerCatcher.OnExitTagSimple += OnExitTriggered;
            TriggerCatcher.OnExitComponentSimple += OnExitTriggered;
        }
        protected virtual void OnDisable()
        {
            TriggerCatcher.OnEnterTagSimple -= OnEnterTriggered;
            TriggerCatcher.OnEnterComponentSimple -= OnEnterTriggered;

            TriggerCatcher.OnExitTagSimple -= OnExitTriggered;
            TriggerCatcher.OnExitComponentSimple -= OnExitTriggered;
        }

        protected abstract void OnEnterTriggered();
        protected abstract void OnExitTriggered();
        #endregion methods
    }
}