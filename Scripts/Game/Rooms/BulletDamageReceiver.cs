using EditorCustom.Attributes;
using Game.Player.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Rooms
{
    [DisallowMultipleComponent]
    public class BulletDamageReceiver : DamageReceiver
    {
        #region fields & properties
        [Title("Modifiers")]
        [SerializeField] private bool useRicochetModifier = false;
        public int RicochetsCountToReceive => ricochetCountToReceive;
        [SerializeField][DrawIf(nameof(useRicochetModifier), true)][Min(1)] private int ricochetCountToReceive = 1;
        
        [SerializeField] private bool usePortalModifier = false;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// This will invoke after <see cref="Bullet.OnCollisionEnter(Collision)"/>
        /// </summary>
        /// <param name="collision"></param>
        protected override void OnCollisionEnter(Collision collision)
        {
            if (!ReceiveOnCollision) return;
            TryReceiveDamageByCollider<Bullet>(collision.collider, false, PhysicsCallback.CollisionEnter);
        }
        protected override void OnTriggerEnter(Collider other)
        {
            if (!ReceiveOnTrigger) return;
            TryReceiveDamageByCollider<Bullet>(other, true, PhysicsCallback.TriggerEnter);
        }

        protected override void OnCollisionStay(Collision collision)
        {
            if (!ReceiveOnCollisionStay) return;
            TryReceiveDamageByCollider<Bullet>(collision.collider, false, PhysicsCallback.CollisionStay);
        }
        protected override void OnTriggerStay(Collider other)
        {
            if (!ReceiveOnTriggerStay) return;
            TryReceiveDamageByCollider<Bullet>(other, true, PhysicsCallback.TriggerStay);
        }

        protected override bool CanReceiveDamageByModifiers<T>(T damageProvider)
        {
            Bullet bullet = (Bullet)(DamageProvider)damageProvider;
            int currentRicochetAmount = bullet.CurrentCollisionHits - 1;
            if (useRicochetModifier && ricochetCountToReceive > currentRicochetAmount) return false;
            if (usePortalModifier)
            {
                if (!bullet.TryGetComponent(out PortalReceiver portalReceiver)) return false;
                if (portalReceiver.CurrentPortalReceives == 0) return false;
            }

            return base.CanReceiveDamageByModifiers(damageProvider);
        }
        #endregion methods
    }
}