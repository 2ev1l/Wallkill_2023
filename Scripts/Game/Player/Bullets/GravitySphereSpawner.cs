using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.Player.Bullets
{
    public class GravitySphereSpawner : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Bullet bullet;
        [SerializeField] private ObjectPool<DestroyablePoolableObject> gravitySpheresPool;
        public float SphereScale
        {
            get => sphereScale;
            set => sphereScale = value;
        }
        [Title("Spawn Settings")][SerializeField] private float sphereScale = 0.5f;
        [SerializeField] private bool drawUIOnly = false;

        [Title("Read Only")]
        [SerializeField][ReadOnly][Min(0)] private int currentWeaponState = 0;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Will be called each time bullet was spawned
        /// </summary>
        private void OnEnable()
        {
            bullet.OnCollisionEntered += OnCollisionEntered;
            bullet.OnInitialized += OnBulletInitialized;
        }
        private void OnDisable()
        {
            bullet.OnCollisionEntered -= OnCollisionEntered;
            bullet.OnInitialized -= OnBulletInitialized;
            if (bullet.Context != null)
                bullet.Context.OnWeaponStateChanged -= SaveCurrentWeaponState;
        }
        private void OnBulletInitialized()
        {
            bullet.Context.OnWeaponStateChanged += SaveCurrentWeaponState;
            SaveCurrentWeaponState(bullet.Context.CurrentWeapon.CurrentState);
        }
        private void SaveCurrentWeaponState(int stateId)
        {
            if (bullet.Context.CurrentWeapon.Id != 3) return;
            currentWeaponState = stateId;
        }
        private void OnCollisionEntered(Collision collision)
        {
            SpawnSphere();
        }
        public void SpawnSphere()
        {
            GravitySphere gravitySphere = (GravitySphere)gravitySpheresPool.GetObject();
            gravitySphere.NegateGravity = currentWeaponState != 0;
            gravitySphere.transform.position = bullet.transform.position;
            gravitySphere.SphereRadius = sphereScale;
            gravitySphere.ResetActivations();
            if (!drawUIOnly)
                gravitySphere.ProvideGravity();
            else
                gravitySphere.EnableUI();
        }
        #endregion methods
    }
}