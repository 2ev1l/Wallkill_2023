using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Game.Player.Bullets
{
    public abstract class ModifierBehaviour : MonoBehaviour
    {
        #region fields & properties
        public static UnityAction OnModifierActivate;
        protected abstract int ReferenceId { get; }
        protected Modifier ReferenceModifier 
        {
            get
            {
                return referenceModifier;
            }
        }
        private Modifier referenceModifier;
        private bool isSubscribedAtModifierChange = false;
        private bool isSubscribedAtModifierRankChange = false;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            SubscribeAtModifierChange();
            TrySetModifier();
        }
        private void OnDestroy()
        {
            UnSubscribeAtModifierChange();
        }
        protected virtual void OnEnable()
        {
            OnModifierActivate += OnModifierActivateRequest;
            if (ReferenceModifier == null)
                SetDefaultModifierParams();
            else
            {
                SetModifierParams();
                SubscribeAtModifierRankChange();
            }
        }
        protected virtual void OnDisable()
        {
            OnModifierActivate -= OnModifierActivateRequest;
            if (ReferenceModifier != null)
                UnSubscribeAtModifierRankChange();
        }
        /// <summary>
        /// Invokes if <see cref="OnModifierActivate"/> was triggered. Subscribed on enable/disable. Does nothing at base.
        /// </summary>
        protected virtual void OnModifierActivateRequest() { }
        private void SubscribeAtModifierChange()
        {
            if (isSubscribedAtModifierChange) return;
            GameData.Data.PlayerData.OnModifierOpened += TrySetModifier;
            isSubscribedAtModifierChange = true;
        }
        private void UnSubscribeAtModifierChange()
        {
            if (!isSubscribedAtModifierChange) return;
            GameData.Data.PlayerData.OnModifierOpened -= TrySetModifier;
            isSubscribedAtModifierChange = false;
        }
        private void SubscribeAtModifierRankChange()
        {
            if (isSubscribedAtModifierRankChange) return;
            ReferenceModifier.OnRankIncreased += SetModifierParams;
            isSubscribedAtModifierRankChange = true;
        }
        private void UnSubscribeAtModifierRankChange()
        {
            if (!isSubscribedAtModifierRankChange) return;
            ReferenceModifier.OnRankIncreased -= SetModifierParams;
            isSubscribedAtModifierRankChange = false;
        }

        private void TrySetModifier(int modifierId)
        {
            if (modifierId != ReferenceId) return;
            TrySetModifier();
        }
        private void TrySetModifier()
        {
            if (referenceModifier != null) return;
            if (GameData.Data.PlayerData.OpenedModifiers.Exists(x => x.Id == ReferenceId, out Modifier modifier))
            {
                referenceModifier = modifier;
                UnSubscribeAtModifierChange();
                SubscribeAtModifierRankChange();
                SetModifierParams();
            }
        }
        /// <summary>
        /// This method invokes on enable if reference modifier is null
        /// </summary>
        protected abstract void SetDefaultModifierParams();
        protected void SetModifierParams() => SetModifierParams(ReferenceModifier.Rank);
        /// <summary>
        /// This method invokes on enable if reference modifier is not null (or when modifier is set)
        /// </summary>
        protected abstract void SetModifierParams(int rank);
        protected T DebugUnknownModifier<T>(T returns)
        {
            Debug.LogError($"Undefined modifier rank #{ReferenceModifier.Rank} with id #{ReferenceId}", this);
            return returns;
        }
        #endregion methods
    }
}