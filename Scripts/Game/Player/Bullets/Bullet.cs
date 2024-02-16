using Data.Interfaces;
using Game.DataBase;
using Game.Rooms;
using System;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Data.Stored;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

namespace Game.Player.Bullets
{
    /// <summary>
    /// Works only with <see cref="ObjectPool{T}"/>
    /// </summary>
    public class Bullet : DamageProvider, IPoolableObject<Bullet>
    {
        #region fields & properties
        public UnityAction<Bullet> OnDestroyed { get; set; }
        public UnityAction<Collider> OnTriggerEntered;
        public UnityAction<Collision> OnCollisionEntered;
        public UnityAction OnDisabled;
        public UnityAction OnSubscribedAtActions;
        public UnityAction OnUnsubscribedAtActions;
        public UnityAction OnInitialized;
        public UnityAction OnInitializedSpawned;
        public Rigidbody Rigidbody => rigidBody;
        [SerializeField] private Rigidbody rigidBody;

        public float Speed => speed;
        [Title("Settings")][SerializeField][Min(0f)] private float speed = 1f;
        [SerializeField][Min(0)] private float disableDelay = 10;
        [SerializeField] private bool disableRigidbodyOnly = false;
        [SerializeField] private TimeDelayContinuous spawnDelay;
        [SerializeField] private bool doDamage = true;
        [SerializeField][DrawIf(nameof(doDamage), true)] private bool doDamageToPlayer = false;
        public float MinVelocityToDamage => minVelocityToDamage;
        [SerializeField][Min(0)] private float minVelocityToDamage = 3f;
        /// <summary>
        /// 1..inf
        /// </summary>
        public int MaxCollisionHits
        {
            get => maxCollisionHits;
            set => maxCollisionHits = Mathf.Max(value, 1);
        }
        [SerializeField][Min(0)] private int maxCollisionHits = 1;
        public int CurrentCollisionHits
        {
            get => currentCollisionHits;
            set => currentCollisionHits = value;
        }
        private int currentCollisionHits = 0;

        [Title("UI")]
        [SerializeField] private bool useAnimations = false;
        [SerializeField][DrawIf(nameof(useAnimations), true)] private BulletAnimations bulletAnimations;
        public bool IsUsing
        {
            get => isUsing;
            set => isUsing = value;
        }
        private bool isUsing;
        public override int Damage => StartDamage * damage * ModifiersDamageScale;
        public int ModifiersDamageScale
        {
            get => modifiersDamageScale;
            set => modifiersDamageScale = value;
        }
        private int modifiersDamageScale = 1;
        public int StartDamage => startDamage;
        private int startDamage;
        public WeaponType StartType => startType;
        private WeaponType startType;
        public Attack Context => context;
        private Attack context;

        public Vector3 LastCollisionHitDirection => lastCollisionHitDirection;
        private Vector3 lastCollisionHitDirection;
        public Collision LastHitCollision => lastHitCollision;
        private Collision lastHitCollision;

        public static readonly string tagWeapon = "Weapon";
        public static readonly string tagPlayer = "Player";
        public static readonly float destroyDelay = 60;
        #endregion fields & properties

        #region methods
        private void OnDestroy()
        {
            TryBreakSpawnDelaying(null);
            UnsubscribeAtActions();
            OnDestroyed?.Invoke(this);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(tagWeapon)) return;
            OnTriggerEntered?.Invoke(other);
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag(tagWeapon)) return;
            if (rigidBody.velocity.magnitude > 0)
                transform.rotation = Quaternion.LookRotation(rigidBody.velocity);
            lastCollisionHitDirection = -transform.forward;
            lastHitCollision = collision;
            currentCollisionHits++;

            OnCollisionEntered?.Invoke(collision);

            CheckCurrentCollisionHits();
        }
        private void CheckCurrentCollisionHits()
        {
            if (maxCollisionHits > 0 && currentCollisionHits >= maxCollisionHits)
            {
                DisableObject();
            }
        }

        public override void OnDamageProvide(int amount)
        {
            //needed because of too fast unsubscribe at actions
            base.OnDamageProvide(amount);
            if (useAnimations)
                bulletAnimations.OnDamageProvide(amount);
        }
        public override bool CanDealDamageToCollider(Collider collider, DamageReceiver.PhysicsCallback callback)
        {
            if (!doDamage) return false;
            if (!base.CanDealDamageToCollider(collider, callback)) return false;

            if (rigidBody.velocity.magnitude < minVelocityToDamage)
            {
                return false;
            }
            if (collider.CompareTag(tagPlayer))
            {
                return doDamageToPlayer;
            }
            return true;
        }
        private void TryBreakSpawnDelaying(Weapon _) => TryBreakSpawnDelaying();
        private void TryBreakSpawnDelaying()
        {
            spawnDelay.TryBreakDelaying();
        }
        /// <summary>
        /// Use this instead of <see cref="OnEnable()"/>
        /// </summary>
        private void SubscribeAtActions()
        {
            Context.OnWeaponChanged += TryBreakSpawnDelaying;
            Context.OnAimingEnd += TryBreakSpawnDelaying;
            spawnDelay.OnDelayBreak += DisableObject;

            OnSubscribedAtActions?.Invoke();
        }
        /// <summary>
        /// Use this instead of <see cref="OnDisable()"/>
        /// </summary>
        private void UnsubscribeAtActions()
        {
            Context.OnWeaponChanged -= TryBreakSpawnDelaying;
            Context.OnAimingEnd -= TryBreakSpawnDelaying;
            spawnDelay.OnDelayBreak -= DisableObject;

            OnUnsubscribedAtActions?.Invoke();
        }
        public void DisableObject()
        {
            CancelInvoke(nameof(DisableObject));
            Invoke(nameof(DestroyThis), destroyDelay);
            IsUsing = false;
            UnsubscribeAtActions();
            OnDisabled?.Invoke();
            if (disableRigidbodyOnly)
            {
                rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rigidBody.isKinematic = true;
            }
            else
                gameObject.SetActive(false);
        }
        /// <summary>
        /// Invoke this after getting an object from the object pool
        /// </summary>
        /// <param name="context"></param>
        public void Init(Attack context)
        {
            this.context = context;
            Weapon contextWeapon = context.CurrentWeapon;
            spawnDelay.TryBreakDelaying();
            SubscribeAtActions();

            currentCollisionHits = 0;
            startType = contextWeapon.Type;
            startDamage = contextWeapon.Damage;
            rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigidBody.isKinematic = false;
            rigidBody.velocity = Vector3.zero;

            OnInitialized?.Invoke();
            gameObject.SetActive(false);
            spawnDelay.Activate(delegate
            {
                gameObject.SetActive(true);
                WeaponModel instantiatedModel = contextWeapon.Model.Instantiated;
                transform.position = GetSpawnPosition();
                transform.forward = context.BulletTrajectoryRay.direction;
                rigidBody.AddForce(1000 * (speed) * transform.forward, ForceMode.Acceleration);
                Invoke(nameof(DisableObject), disableDelay);
                OnInitializedSpawned?.Invoke();
            });
        }
        private Vector3 GetSpawnPosition()
        {
            WeaponModel instantiatedModel = Context.CurrentWeapon.Model.Instantiated;
            if (startType != WeaponType.Hand) return instantiatedModel.BulletPosition;

            //hand
            Vector3 raycastStart = instantiatedModel.transform.position;
            Vector3 direction = Context.AimingRay.direction;
            float distance = Vector3.Distance(instantiatedModel.BulletPosition, raycastStart);
            if (Physics.Raycast(raycastStart, direction, out RaycastHit hit, distance + 0.4f, Context.NearShootBlockMask, QueryTriggerInteraction.Ignore))
            {
                return hit.point;
            }
            else if (Physics.Raycast(raycastStart, Context.CharacterController.transform.forward, out hit, distance + 0.3f, Context.NearShootBlockMask, QueryTriggerInteraction.Ignore))
            {
                return hit.point;
            }
            return instantiatedModel.BulletPosition;
        }
        public Bullet InstantiateThis()
        {
            Bullet bullet = Instantiate(this, Vector3.zero, transform.rotation);
            bullet.transform.localScale = this.transform.localScale;
            return bullet;
        }
        public Bullet RevertChangedParams()
        {
            gameObject.SetActive(true);
            CancelInvoke(nameof(DestroyThis));
            return this;
        }
        private void DestroyThis() => Destroy(this.gameObject);
        #endregion methods
    }
}