using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.UI.Triggers
{
    public class TriggerCatcher : MonoBehaviour
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> tag;
        /// </summary>
        public UnityAction<string, Collider> OnEnterTag;
        public UnityAction OnEnterTagSimple;

        public UnityAction<Component, Collider> OnEnterComponent;
        public UnityAction OnEnterComponentSimple;

        /// <summary>
        /// <see cref="{T0}"/> tag;
        /// </summary>
        public UnityAction<string, Collider> OnStayTag;
        public UnityAction OnStayTagSimple;

        /// <summary>
        /// <see cref="{T0}"/> tag;
        /// </summary>
        public UnityAction<string, Collider> OnExitTag;
        public UnityAction OnExitTagSimple;

        public UnityAction<Component, Collider> OnExitComponent;
        public UnityAction OnExitComponentSimple;

        [Title("Settings")]
        [SerializeField] private bool catchOnStay = false;

        [Title("Tag trigger")]
        [SerializeField] private bool triggerTag = true;
        [SerializeField][DrawIf(nameof(triggerTag), true, DisablingType.ReadOnly)] private List<string> tagsToTrigger = new() { "Player" };

        [Title("Component trigger")]
        [SerializeField] private bool triggerComponent = false;
        [SerializeField][DrawIf(nameof(triggerComponent), true, DisablingType.ReadOnly)] private List<Component> componentsToTrigger = new();
        #endregion fields & properties

        #region methods
        private void OnTriggerEnter(Collider other)
        {
            if (triggerTag)
            {
                if (!IsTagExists(other, out string result)) return;
                OnEnterTag?.Invoke(result, other);
                OnEnterTagSimple?.Invoke();
            }
            if (triggerComponent)
            {
                if (!IsComponentExists(other, out Component component)) return;
                Debug.Log("Trigger bullet");
                OnEnterComponent?.Invoke(component, other);
                OnEnterComponentSimple?.Invoke();
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (!catchOnStay) return;
            if (triggerTag)
            {
                if (!IsTagExists(other, out string result)) return;
                OnStayTag?.Invoke(result, other);
                OnStayTagSimple?.Invoke();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (triggerTag)
            {
                if (!IsTagExists(other, out string tag)) return;
                OnExitTag?.Invoke(tag, other);
                OnExitTagSimple?.Invoke();
            }

            if (triggerComponent)
            {
                if (!IsComponentExists(other, out Component component)) return;
                OnExitComponent?.Invoke(component, other);
                OnExitComponentSimple?.Invoke();
            }
        }
        private bool IsComponentExists(Collider other, out Component component)
        {
            Component foundComponent = null;
            componentsToTrigger.Find(x => other.TryGetComponent(x.GetType(), out foundComponent));
            component = foundComponent;
            return component != null;
        }
        private bool IsTagExists(Collider other, out string tag)
        {
            tag = tagsToTrigger.Find(x => other.CompareTag(x));
            return tag != null;
        }
        #endregion methods
    }
}