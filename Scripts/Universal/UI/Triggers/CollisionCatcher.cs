using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.UI.Triggers
{
    public class CollisionCatcher : MonoBehaviour
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> tag;
        /// </summary>
        public UnityAction<string, Collision> OnEnterTag;
        public UnityAction OnEnterTagSimple;

        public UnityAction<Component, Collision> OnEnterComponent;
        public UnityAction OnEnterComponentSimple;

        /// <summary>
        /// <see cref="{T0}"/> tag;
        /// </summary>
        public UnityAction<string, Collision> OnExitTag;
        public UnityAction OnExitTagSimple;

        public UnityAction<Component, Collision> OnExitComponent;
        public UnityAction OnExitComponentSimple;

        [Header("Tag collide")]
        [SerializeField] private bool collideTag = true;
        [SerializeField] private List<string> tagsToCollide = new() { "Player" };

        [Header("Component collide")]
        [SerializeField] private bool collideComponent = false;
        [SerializeField] private List<Component> componentsToCollide = new();
        #endregion fields & properties

        #region methods
        private void OnCollisionEnter(Collision collision)
        {
            if (collideTag)
            {
                if (!IsTagExists(collision, out string result)) return;
                OnEnterTag?.Invoke(result, collision);
                OnEnterTagSimple?.Invoke();
            }
            if (collideComponent)
            {
                if (!IsComponentExists(collision, out Component component)) return;
                OnEnterComponent?.Invoke(component, collision);
                OnEnterComponentSimple?.Invoke();
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            if (collideTag)
            {
                if (!IsTagExists(collision, out string tag)) return;
                OnExitTag?.Invoke(tag, collision);
                OnExitTagSimple?.Invoke();
            }

            if (collideComponent)
            {
                if (!IsComponentExists(collision, out Component component)) return;
                OnExitComponent?.Invoke(component, collision);
                OnExitComponentSimple?.Invoke();
            }
        }
        private bool IsComponentExists(Collision collision, out Component component)
        {
            Component foundComponent = null;
            componentsToCollide.Find(x => collision.collider.TryGetComponent(x.GetType(), out foundComponent));
            component = foundComponent;
            return component != null;
        }
        private bool IsTagExists(Collision collision, out string tag)
        {
            tag = tagsToCollide.Find(x => collision.collider.CompareTag(x));
            return tag != null;
        }
        #endregion methods
    }
}