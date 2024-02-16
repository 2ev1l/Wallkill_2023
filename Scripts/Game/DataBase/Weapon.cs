using Data.Interfaces;
using EditorCustom.Attributes;
using Game.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI;

namespace Game.DataBase
{
    [System.Serializable]
    public class Weapon : ICloneable<Weapon>
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - stateId
        /// </summary>
        public UnityAction<int> OnStateChanged;
        public int Id => id;
        [SerializeField][Min(0)] private int id;
        public WeaponSpawn Model => model;
        [SerializeField] private WeaponSpawn model;

        public WeaponType Type => type;
        [SerializeField] private WeaponType type;
        public TimeDelayContinuous ReloadDelay => reloadDelay;
        [SerializeField] private TimeDelayContinuous reloadDelay = new();
        public TimeDelay ShootDelay => shootDelay;
        [SerializeField] private TimeDelay shootDelay = new();
        public int Damage => damage;
        [SerializeField][Min(0)] private int damage = 1;
        public bool HasStates => hasStates;
        [SerializeField] private bool hasStates = false;
        [SerializeField][DrawIf(nameof(hasStates), true)][Min(1)] private int maxState = 1;
        public int CurrentState => currentState;
        [SerializeField][DrawIf(nameof(hasStates), true)][Min(0)] private int currentState = 0;
        public BulletsInfo BulletsInfo => bulletsInfo;
        [SerializeField] private BulletsInfo bulletsInfo;
        /// <summary>
        /// Probably you need <see cref="LanguageInfo.Text"/>
        /// </summary>
        public LanguageInfo Name => name;
        [SerializeField] private LanguageInfo name = new(0, TextType.Items);

        [SerializeField] private List<AudioClip> shootClips = new();
        [SerializeField] private List<AudioClip> reloadClips = new();

        [SerializeField] private bool allowChanges = true;
        #endregion fields & properties

        #region methods
        public bool TryGetShootClip(out AudioClip clip) => TryGetRandomClip(shootClips, out clip);
        public bool TryGetReloadClip(out AudioClip clip) => TryGetRandomClip(reloadClips, out clip);
        private bool TryGetRandomClip(List<AudioClip> clips, out AudioClip clip)
        {
            clip = null;
            if (clips.Count == 0) return false;
            clip = clips[Random.Range(0, clips.Count)];
            return true;
        }
        public bool CanShoot()
        {
            if (!allowChanges) return false;
            if (!shootDelay.CanActivate) return false;
            if (ReloadDelay.IsDelaying) return false;
            if (!BulletsInfo.CanShoot()) return false;
            return true;
        }
        public bool TryShoot()
        {
            if (!CanShoot()) return false;
            if (!BulletsInfo.TryShoot()) return false;
            shootDelay.Activate();
            return true;
        }
        public bool TryReload()
        {
            if (!allowChanges) return false;
            if (!ReloadDelay.CanActivate) return false;
            if (BulletsInfo.IsReloadDisabled()) return false;
            ReloadDelay.Activate(delegate { BulletsInfo.TryReload(); });
            return true;
        }
        public bool CanChangeWeaponState()
        {
            if (!allowChanges) return false;
            if (!HasStates) return false;
            if (reloadDelay.IsDelaying) return false;
            return true;
        }
        public bool TryChangeState()
        {
            if (!CanChangeWeaponState()) return false;
            currentState++;
            currentState %= (maxState + 1);
            OnStateChanged?.Invoke(currentState);
            return true;
        }

        public Weapon Clone()
        {
            return new()
            {
                id = id,
                model = Model.Clone(),
                type = type,

                reloadDelay = reloadDelay.Clone(),
                shootDelay = shootDelay.Clone(),
                damage = damage,
                hasStates = hasStates,
                maxState = maxState,
                currentState = currentState,
                bulletsInfo = BulletsInfo.Clone(),
                name = name,
                shootClips = shootClips,
                reloadClips = reloadClips,

                allowChanges = true,
            };
        }
        #endregion methods
    }
}