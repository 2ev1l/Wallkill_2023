using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Rooms
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class DamageProvider : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnDamageProvidedEvent;
        /// <summary>
        /// <see cref="{T0}"/> - clamped damage dealt
        /// </summary>
        public UnityAction<int> OnDamageProvided;
        public virtual int Damage
        {
            get => damage;
            set => damage = value;
        }
        [SerializeField][Min(0)] protected int damage = 1;
        public bool ProvideOnStay => provideOnStay;
        [SerializeField] private bool provideOnStay = false;

        public bool UseHitSound => useHitSound;
        [SerializeField] private bool useHitSound = false;
        public AudioClip HitSound => hitSound;
        [SerializeField][DrawIf(nameof(useHitSound), true)] private AudioClip hitSound;
        public LayerMask HitSoundLayers => hitSoundLayers;
        [SerializeField][DrawIf(nameof(useHitSound), true)] LayerMask hitSoundLayers = Physics.AllLayers;
        #endregion fields & properties

        #region methods
        public virtual void OnDamageProvide(int amount)
        {
            OnDamageProvided?.Invoke(amount);
            OnDamageProvidedEvent?.Invoke();
        }
        public virtual bool CanDealDamageToCollider(Collider collider, DamageReceiver.PhysicsCallback callback) => callback switch
        {
            DamageReceiver.PhysicsCallback.TriggerStay => provideOnStay,
            DamageReceiver.PhysicsCallback.CollisionStay => provideOnStay,
            _ => true,
        };
        #endregion methods
    }
}