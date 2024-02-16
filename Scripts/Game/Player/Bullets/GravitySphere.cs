using EditorCustom.Attributes;
using Game.Rooms.Mechanics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal;

namespace Game.Player.Bullets
{
    public class GravitySphere : DestroyablePoolableObject
    {
        #region fields & properties
        [Title("UI")]
        [SerializeField] private Renderer render;

        public float SphereRadius
        {
            get => sphereRadius;
            set 
            {
                float oldValue = sphereRadius;
                sphereRadius = value;
                transform.localScale /= oldValue / sphereRadius;
            }
        }
        [Title("Gravity")][SerializeField][Min(0)] private float sphereRadius = 1f;
        [SerializeField][Min(0)] private int maxActivations = 1;
        [SerializeField] private bool provideOnEnable = true;
        public bool NegateGravity
        {
            get => negateGravity;
            set => negateGravity = value;
        }
        [SerializeField] private bool negateGravity = false; 

        [Title("Read Only")]
        [SerializeField][ReadOnly] private int currentActivations = 0;

        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (provideOnEnable)
                ProvideGravity();
        }
        public void ResetActivations()
        {
            currentActivations = 0;
            DisableUI();
        }
        private Collider[] CatchObjectsInSphere() => Physics.OverlapSphere(transform.position, sphereRadius);
        public bool CanProvideGravity() => currentActivations < maxActivations;
        public void ProvideGravity()
        {
            if (!CanProvideGravity()) return;
            currentActivations++;
            EnableUI();
            Collider[] objects = CatchObjectsInSphere();
            foreach (var el in objects)
            {
                if (!el.TryGetComponent(out GravityReceiver gravityReceiver)) continue;
                gravityReceiver.ReceiveGravity(this);
            }
        }
        public void DisableUI()
        {
            if (render == null) return;
            render.enabled = false;
        }
        public void EnableUI()
        {
            if (render == null) return;
            render.enabled = true;
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField] private bool doDebug = true;
        [SerializeField][DrawIf(nameof(doDebug), true)] private bool debugAlways = false;
        private void OnDrawGizmosSelected()
        {
            if (!doDebug) return;
            if (debugAlways) return;
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }
        private void OnDrawGizmos()
        {
            if (!doDebug) return;
            if (!debugAlways) return;
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }
#endif //UNITY_EDITOR
    }
}