using Data.Interfaces;
using Data.Stored;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using UnityEngine;
using UnityEngine.Events;
using Universal.UI;
using Game.UI;
using Game.Labyrinth;
using Universal;

namespace Game.Player
{
    /// <summary>
    /// Needed to runtime <see cref="PlayerStats"/> observation.
    /// </summary>
    public class StatsBehaviour : MonoBehaviour, IInitializable
    {
        #region fields & properties
        public static PlayerStats Stats => GameData.Data.PlayerData.Stats;
        [SerializeField] private Moving moving;
        [SerializeField] private DamageReceiver damageReceiver;

        [Title("Settings")]
        [SerializeField] private StatChangeLayer healthRegen = new();
        [SerializeField] private StatChangeLayer staminaRegen = new();

        public bool IsDead => isDead;
        [Title("Read Only")][SerializeField][ReadOnly] private bool isDead = false;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            ResetStats();
            Init();
        }
        private void OnEnable()
        {
            Stats.Health.OnValueReachedMinimum += OnDead;
            WorldLoader.Instance.OnWorldLoaded += CheckWorldLoaded;
        }
        private void OnDisable()
        {
            Stats.Health.OnValueReachedMinimum -= OnDead;
            WorldLoader.Instance.OnWorldLoaded -= CheckWorldLoaded;
        }
        private void OnDead()
        {
            isDead = true;

            if (!GameData.Data.TutorialData.IsCompleted) return;

            int walletAmount = GameData.Data.PlayerData.Wallet.Value;
            GameData.Data.PlayerData.Wallet.TrySpent(CustomMath.Multiply(walletAmount, 20f));

            int maxHealthReduceAmount = 1;
            int newMaxHealth = Mathf.Max(Stats.Health.GetRange().y - maxHealthReduceAmount, 1);
            Stats.Health.ChangeMaxRange(newMaxHealth, false);
        }
        public void Init()
        {
            healthRegen.Value = Stats.Health;
            staminaRegen.Value = Stats.Stamina;
            damageReceiver.Init(Stats.Health);
        }
        private void CheckWorldLoaded(WorldType worldType)
        {
            if (IsDead) return;
            if (worldType == WorldType.Portal)
                ResetStats();
        }
        public void ResetStats()
        {
            Stats.Health.SetToMax();
            Stats.Stamina.SetToMax();
        }

        private void RegenerateStat(StatChangeLayer layer)
        {
            layer.IncreaseChangedAmountByTimeSpeed();
            layer.TryIncreaseStat(true, true);
        }
        private void Update()
        {
            if (IsDead) return;
            RegenerateStat(healthRegen);
            float staminaRegenScale = 1;
            staminaRegenScale *= moving.LastFrameMoved ? 0.5f : 1f;
            staminaRegen.ChangeSpeedScale = staminaRegenScale;
            RegenerateStat(staminaRegen);
        }

        #endregion methods
    }
}