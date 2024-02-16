using Data.Interfaces;
using Data.Stored;
using Game.Player.Bullets;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI.Audio;

namespace Game.Rooms
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class DamageReceiver : MonoBehaviour, IInitializable<Stat>
    {
        #region fields & properties
        public UnityAction<Stat> OnHealthStatChanged;
        public UnityEvent OnDamageReceivedEvent;

        public UnityAction<CallbackInfo> OnDamageReceived;
        /// <summary>
        /// Invisible state starts when <see cref="OnDamageReceived"/> triggers.
        /// </summary>
        public UnityAction OnInvisibleStateEnd;
        public Stat Health => health;
        [SerializeField] private Stat health;

        public bool ReceiveOnTrigger => receiveOnTrigger;
        [Title("Settings")][SerializeField] private bool receiveOnTrigger = false;
        public bool ReceiveOnTriggerStay => receiveOnTriggerStay;
        [SerializeField] private bool receiveOnTriggerStay = false;
        [SerializeField] private Collider triggerCollider;
        public bool ReceiveOnCollision => receiveOnCollision;
        [SerializeField] private bool receiveOnCollision = true;
        public bool ReceiveOnCollisionStay => receiveOnCollisionStay;
        [SerializeField] private bool receiveOnCollisionStay = false;
        [SerializeField] private Collider collisionCollider;

        public bool UseLocalActions => useLocalActions;
        [Tooltip("Damage will be received when any damage provider is collided")]
        [SerializeField] private bool useLocalActions = true;
        [SerializeField] private bool initializeAtAwake = true;
        [SerializeField][Min(1)] private int minDamageToReceive = 1;
        public float InvincibleDelay => invincibleDelay.Delay;
        [SerializeField] private TimeDelay invincibleDelay;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private bool isInitialized = false;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            invincibleDelay.OnDelayReady = delegate { OnInvisibleStateEnd?.Invoke(); };
            if (initializeAtAwake) isInitialized = true;
        }
        private void OnDestroy()
        {
            invincibleDelay.OnDelayReady = null;
        }
        public void Init(Stat value)
        {
            if (isInitialized) return;
            this.health = value;
            isInitialized = true;
        }
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!ReceiveOnCollision) return;
            TryReceiveDamageByCollider<DamageProvider>(collision.collider, false, PhysicsCallback.CollisionEnter);
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!ReceiveOnTrigger) return;
            TryReceiveDamageByCollider<DamageProvider>(other, true, PhysicsCallback.TriggerEnter);
        }

        protected virtual void OnCollisionStay(Collision collision)
        {
            if (!ReceiveOnCollisionStay) return;
            TryReceiveDamageByCollider<DamageProvider>(collision.collider, false, PhysicsCallback.CollisionStay);
        }
        protected virtual void OnTriggerStay(Collider other)
        {
            if (!ReceiveOnTriggerStay) return;
            TryReceiveDamageByCollider<DamageProvider>(other, true, PhysicsCallback.TriggerStay);
        }

        protected void TryReceiveDamageByCollider<T>(Collider collider, bool isTrigger, PhysicsCallback physicsCallback) where T : DamageProvider
        {
            if (!UseLocalActions) return;
            if (!collider.TryGetComponent(out T damageProvider)) return;
            if (!damageProvider.CanDealDamageToCollider(isTrigger ? triggerCollider : collisionCollider, physicsCallback)) return;
            if (!CanReceiveDamageByModifiers(damageProvider)) return;
            if (!TryReceiveDamage(damageProvider.Damage, isTrigger, collider, out int changedAmount)) return;
            TryPlayHitSound(damageProvider, isTrigger ? triggerCollider : collisionCollider);
            damageProvider.OnDamageProvide(changedAmount);
        }
        protected virtual bool CanReceiveDamageByModifiers<T>(T damageProvider) where T : DamageProvider
        {
            return true;
        }
        public bool TryReceiveDamage(int amount, bool isTrigger, Collider invokedCollider, out int changedAmount)
        {
            changedAmount = 0;
            if (amount < minDamageToReceive) return false;
            if (isTrigger && !receiveOnTrigger) return false;
            if (!isTrigger && !receiveOnCollision) return false;
            if (!invincibleDelay.CanActivate) return false;
            bool isDecreased = health.TryDecreaseValue(amount, out changedAmount);
            if (!isDecreased) return false;

            invincibleDelay.Activate();

            if (OnDamageReceived != null)
            {
                CallbackInfo callbackInfo = new(isTrigger, invokedCollider, isTrigger ? triggerCollider : collisionCollider, changedAmount, Health);
                OnDamageReceived.Invoke(callbackInfo);
            }
            OnDamageReceivedEvent?.Invoke();
            return true;
        }
        [SerializedMethod]
        public void SimulateCollisionDamage(DamageProvider damageProvider)
        {
            if (!damageProvider.CanDealDamageToCollider(collisionCollider, PhysicsCallback.CollisionEnter)) return;
            if (!CanReceiveDamageByModifiers(damageProvider)) return;
            if (!TryReceiveDamage(damageProvider.Damage, false, null, out int changedAmount))
            TryPlayHitSound(damageProvider, collisionCollider);
        }
        private void TryPlayHitSound<T>(T damageProvider, Collider invokedCollider) where T : DamageProvider
        {
            if (!damageProvider.UseHitSound) return;
            if (damageProvider.HitSound == null) return;
            if (damageProvider.HitSoundLayers.ContainLayer(invokedCollider.gameObject.layer))
            {
                AudioManager.PlayClipAtPoint(damageProvider.HitSound, Universal.UI.Audio.AudioType.Audio, transform.position);
            }
        }

        [SerializedMethod]
        public void StopReceiveOnCollision() => receiveOnCollision = false;
        [SerializedMethod]
        public void StartReceiveOnCollision() => receiveOnCollision = true;
        #endregion methods

        public enum PhysicsCallback
        {
            TriggerEnter,
            TriggerStay,
            CollisionEnter,
            CollisionStay,
        }

        public class CallbackInfo
        {
            public bool IsCalledByTrigger { get; }
            public Collider ColliderInvoked { get; }
            public Collider ColliderOwn { get; }
            public int DamageReceived { get; }
            public Stat StatChanged { get; }

            public CallbackInfo(bool isCalledByTrigger, Collider colliderInvoked, Collider colliderOwn, int damageReceived, Stat statChanged)
            {
                IsCalledByTrigger = isCalledByTrigger;
                ColliderInvoked = colliderInvoked;
                ColliderOwn = colliderOwn;
                DamageReceived = damageReceived;
                StatChanged = statChanged;
            }
        }
    }
}