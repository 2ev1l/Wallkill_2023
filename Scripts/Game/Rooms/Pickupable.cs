using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.UI.Triggers;

namespace Game.Rooms
{
    public class Pickupable : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnPickUp;
        public UnityEvent OnPickUpEvent;
        [SerializeField] private TriggerCatcher triggerCatcher;
        [SerializeField] private Collider activationCollider;
        public int Id => id;
        [SerializeField][Min(0)] private int id = 0;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            triggerCatcher.OnEnterTagSimple += PickUp;
        }
        protected virtual void OnDisable()
        {
            triggerCatcher.OnEnterTagSimple -= PickUp;
        }
        /// <summary>
        /// Invokes events
        /// </summary>
        public virtual void PickUp()
        {
            activationCollider.enabled = false;
            OnPickUp?.Invoke();
            OnPickUpEvent?.Invoke();
        }
        #endregion methods
    }
}