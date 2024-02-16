using Data.Interfaces;
using Game.Player;
using Game.Player.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Game.DataBase
{
    [System.Serializable]
    public class BulletsInfo : ICloneable<BulletsInfo>
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> bulletsCount
        /// </summary>
        public UnityAction<int> OnCurrentBulletsChanged;
        /// <summary>
        /// <see cref="{T0}"/> bulletsCount
        /// </summary>
        public UnityAction<int> OnTotalBulletsChanged;

        public int BulletsPerShoot => bulletsPerShoot;
        [SerializeField][Min(0)] private int bulletsPerShoot = 1;
        /// <summary>
        /// -1 Equals Infinity
        /// </summary>
        public int BulletsAtClip => bulletsAtClip;
        [SerializeField][Min(-1)] private int bulletsAtClip = 0;
        /// <summary>
        /// -1 Equals Infinity
        /// </summary>
        public int BulletsTotal => bulletsTotal;
        [SerializeField][Min(-1)] private int bulletsTotal = 0;
        /// <summary>
        /// -1 Equals Infinity
        /// </summary>
        public int BulletsMax => bulletsMax;
        [SerializeField][Min(-1)] private int bulletsMax = 0;

        /// <summary>
        /// -1 Equals Infinity
        /// </summary>
        public int BulletsCurrent => bulletsCurrent;
        [SerializeField][Min(-1)] private int bulletsCurrent = 0;

        public ObjectPool<Bullet> Bullets => bullets;
        [SerializeField] private ObjectPool<Bullet> bullets = new();

        [SerializeField] private bool allowChanges = true;
        public bool NeedReloading => bulletsCurrent < bulletsPerShoot;
        #endregion fields & properties

        #region methods
        public void ResetTotalBulletsToMax()
        {
            if (!allowChanges) return;
            if (bulletsTotal == -1) return;
            bulletsTotal = bulletsMax;
            OnTotalBulletsChanged?.Invoke(bulletsTotal);
        }
        public void ResetCurrentBulletsToClip()
        {
            if (!allowChanges) return;
            if (bulletsCurrent == -1 || bulletsAtClip == -1) return;
            bulletsCurrent = bulletsAtClip;
            OnCurrentBulletsChanged?.Invoke(bulletsCurrent);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">>0</param>
        public void AddTotalBullets(int amount)
        {
            if (!allowChanges) return;
            if (amount <= 0 || bulletsTotal == -1) return;
            bulletsTotal += amount;
            bulletsTotal = Mathf.Min(bulletsTotal, bulletsMax);
            OnTotalBulletsChanged?.Invoke(bulletsTotal);
        }
        private void DecreaseTotalBullets(int amount)
        {
            if (amount <= 0 || bulletsTotal == -1) return;
            bulletsTotal -= amount;
            OnTotalBulletsChanged?.Invoke(bulletsTotal);
        }

        public bool IsReloadDisabled() => bulletsCurrent == -1 || bulletsTotal == 0 || (bulletsCurrent >= bulletsAtClip && bulletsAtClip > -1);
        /// <summary>
        /// Probably you need <see cref="Weapon.TryReload"/> instead
        /// </summary>
        /// <returns></returns>
        public bool TryReload()
        {
            if (!allowChanges) return false;
            if (IsReloadDisabled()) return false;

            int neededToReload = bulletsAtClip == -1 ? bulletsTotal : bulletsAtClip - bulletsCurrent;
            if (bulletsTotal > -1)
            {
                neededToReload = Mathf.Min(neededToReload, bulletsTotal);
                DecreaseTotalBullets(neededToReload);
            }
            IncreaseCurrentBullets(neededToReload);
            return true;
        }
        private void IncreaseCurrentBullets(int amount)
        {
            if (amount <= 0 || bulletsCurrent == -1) return;
            bulletsCurrent += amount;
            OnCurrentBulletsChanged?.Invoke(bulletsCurrent);
        }
        /// <summary>
        /// Probably you need <see cref="Weapon.TryShoot"/> instead
        /// </summary>
        /// <returns></returns>
        public bool TryShoot()
        {
            if (!CanShoot()) return false;
            if (bulletsCurrent == -1) return true;
            bulletsCurrent -= bulletsPerShoot;
            OnCurrentBulletsChanged?.Invoke(bulletsCurrent);
            return true;
        }
        public bool CanShoot()
        {
            if (!allowChanges) return false;
            if (bulletsCurrent == -1) return true;
            if (NeedReloading) return false;
            return true;
        }
        public BulletsInfo Clone()
        {
            return new()
            {
                bulletsAtClip = bulletsAtClip,
                bulletsTotal = bulletsTotal,
                bulletsCurrent = bulletsCurrent,
                bulletsPerShoot = bulletsPerShoot,
                bulletsMax = bulletsMax,
                bullets = Bullets.Clone(),
                allowChanges = true
            };
        }

        public BulletsInfo() { }

        #endregion methods
    }
}