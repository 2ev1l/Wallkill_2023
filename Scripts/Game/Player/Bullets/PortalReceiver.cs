using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Bullets
{
    public class PortalReceiver : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Bullet bullet;
        [SerializeField] private BulletAnimations bulletAnimations;
        [SerializeField][Min(0)] private int maxPortalReceives = 1;
        public int CurrentPortalReceives => currentPortalReceives;
        private int currentPortalReceives = 0;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            bullet.OnCollisionEntered += OnCollisionEntered;
            currentPortalReceives = 0;
        }
        private void OnDisable()
        {
            bullet.OnCollisionEntered -= OnCollisionEntered;
        }
        private void OnCollisionEntered(Collision collision)
        {
            if (currentPortalReceives >= maxPortalReceives) return;
            if (!collision.collider.TryGetComponent(out PortalProvider portalProvider))
            {
                bullet.CurrentCollisionHits = bullet.MaxCollisionHits;
                return;
            }
            currentPortalReceives++;
            portalProvider.EnterPortal(bullet.transform);
            bulletAnimations.ClearTrail();
            float oldMagnitude = bullet.Rigidbody.velocity.magnitude;
            bullet.Rigidbody.velocity = portalProvider.Exit.transform.forward * oldMagnitude;
        }
        #endregion methods
    }
}