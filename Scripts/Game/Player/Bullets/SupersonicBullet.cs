using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Bullets
{
    public class SupersonicBullet : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Bullet bullet;
        [SerializeField] private PortalReceiver portalReceiver;

        [Title("Read Only")]
        [SerializeField][ReadOnly][Min(0)] private int currentWeaponState = 0;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            bullet.OnInitialized += OnBulletInitialized;
        }
        private void OnDisable()
        {
            bullet.OnInitialized -= OnBulletInitialized;
            if (bullet.Context != null)
                bullet.Context.OnWeaponStateChanged -= SaveCurrentWeaponState;
        }
        private void OnBulletInitialized()
        {
            bullet.Context.OnWeaponStateChanged += SaveCurrentWeaponState;
            SaveCurrentWeaponState(bullet.Context.CurrentWeapon.CurrentState);
            if (currentWeaponState == 0)
            {
                portalReceiver.enabled = true;
                bullet.ModifiersDamageScale = 1;
                bullet.MaxCollisionHits = 2;
            }
            else
            {
                portalReceiver.enabled = false;
                bullet.ModifiersDamageScale = 100;
                bullet.MaxCollisionHits = 1;
            }
        }
        private void SaveCurrentWeaponState(int stateId)
        {
            if (bullet.Context.CurrentWeapon.Id != 4) return;
            currentWeaponState = stateId;
        }
        #endregion methods
    }
}