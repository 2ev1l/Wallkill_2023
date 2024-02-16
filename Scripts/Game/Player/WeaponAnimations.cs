using Game.DataBase;
using Game.Player.Bullets;
using System.Collections.Generic;
using EditorCustom.Attributes;
using UnityEngine;
using Universal;
using Universal.UI;
using Universal.UI.Layers;

namespace Game.Player
{
    public class WeaponAnimations : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private WeaponModel model;

        [Title("Reload")]
        [SerializeField] private bool enableReloadAnimation = true;
        [DrawIf(nameof(enableReloadAnimation), true, DisablingType.ReadOnly)][SerializeField] private List<Renderer> reloadRenderers;
        [DrawIf(nameof(enableReloadAnimation), true, DisablingType.DontDraw)][SerializeField] private Material reloadMaterial;

        [Title("Recoil")]
        [SerializeField] private bool enableRecoil = true;
        [DrawIf(nameof(enableRecoil), true)][SerializeField][Min(0)] private float recoilTimeStart = 0.1f;
        [DrawIf(nameof(enableRecoil), true)][SerializeField][Min(0)] private float recoilTimeRevert = 0.1f;
        [DrawIf(nameof(enableRecoil), true)][SerializeField] private Transform recoilPivot;
        [DrawIf(nameof(enableRecoil), true)][SerializeField] private Transform recoilStart;
        [DrawIf(nameof(enableRecoil), true)][SerializeField] private Transform recoilEnd;
        [DrawIf(nameof(enableRecoil), true)][SerializeField] private VectorLayer recoilPositionLayer;
        [DrawIf(nameof(enableRecoil), true)][SerializeField] private QuaternionLayer recoilRotationLayer;
        private bool isRecoilStarted = false;

        [Title("Catrtridge Case")]
        [SerializeField] private bool enableCartridgeCaseAtShooting = false;
        [DrawIf(nameof(enableCartridgeCaseAtShooting), true)][SerializeField] private ObjectPool<DestroyablePoolableObject> cartridgeCaseShootPool;
        [DrawIf(nameof(enableCartridgeCaseAtShooting), true)][SerializeField] private Transform cartridgeCaseShootStart;
        [DrawIf(nameof(enableCartridgeCaseAtShooting), true)][SerializeField] private Transform cartridgeCaseShootDirection;
        [DrawIf(nameof(enableCartridgeCaseAtShooting), true)][SerializeField][Min(0)] private float cartridgeCaseShootForce = 1f;

        [Space(10)]
        [SerializeField] private bool enableCartridgeCaseAtReloading = false;
        [DrawIf(nameof(enableCartridgeCaseAtReloading), true)][SerializeField] private ObjectPool<DestroyablePoolableObject> cartridgeCaseReloadPool;
        [DrawIf(nameof(enableCartridgeCaseAtReloading), true, DisablingType.ReadOnly)][SerializeField] private List<Transform> cartridgeCaseReloadStart;
        [DrawIf(nameof(enableCartridgeCaseAtReloading), true)][SerializeField] private Transform cartridgeCaseReloadDirection;
        [DrawIf(nameof(enableCartridgeCaseAtReloading), true)][SerializeField][Min(0)] private float cartridgeCaseReloadForce = 1f;

        [Title("Shoot Fire")]
        [SerializeField] private bool enableShootFire = false;
        [DrawIf(nameof(enableShootFire), true)][SerializeField] private ObjectPool<DestroyablePoolableObject> shootFirePool;
        [DrawIf(nameof(enableShootFire), true)][SerializeField] private Transform shootFireStart;

        [Title("States")]
        [SerializeField] private bool enableStates = false;
        [SerializeField][DrawIf(nameof(enableStates), true, DisablingType.ReadOnly)] private StateAnimation[] statesAnimation;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private bool isSubscribed = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            model.OnInitialized += Subscribe;
            model.OnEnable += Subscribe;
            model.OnDisable += UnSubscribe;
            Subscribe();
        }
        private void OnDisable()
        {
            model.OnInitialized -= Subscribe;
            model.OnEnable -= Subscribe;
            model.OnDisable -= UnSubscribe;
            UnSubscribe();
        }
        private void Subscribe()
        {
            if (isSubscribed) return;
            if (!model.IsInitialized) return;
            Weapon currentWeapon = model.Context.CurrentWeapon;
            if (enableReloadAnimation)
                currentWeapon.ReloadDelay.OnActivated += StartReloadAnimation;
            if (enableCartridgeCaseAtReloading)
                currentWeapon.ReloadDelay.OnActivated += SpawnCartridgeCaseAtReload;
            if (enableCartridgeCaseAtShooting)
                currentWeapon.ShootDelay.OnActivated += SpawnCartridgeCaseAtShoot;
            if (enableRecoil)
                currentWeapon.ShootDelay.OnActivated += StartRecoil;
            if (enableShootFire)
                currentWeapon.ShootDelay.OnActivated += SpawnShootFire;
            if (enableStates)
            {
                currentWeapon.OnStateChanged += StartStateAnimation;
                StartStateAnimation(currentWeapon.CurrentState);
            }
            isSubscribed = true;
        }
        private void UnSubscribe()
        {
            if (!isSubscribed) return;
            if (!model.IsInitialized) return;
            Weapon currentWeapon = model.Context.CurrentWeapon;
            currentWeapon.ReloadDelay.OnActivated -= StartReloadAnimation;
            currentWeapon.ReloadDelay.OnActivated -= SpawnCartridgeCaseAtReload;
            currentWeapon.ShootDelay.OnActivated -= SpawnCartridgeCaseAtShoot;
            currentWeapon.ShootDelay.OnActivated -= StartRecoil;
            currentWeapon.ShootDelay.OnActivated -= SpawnShootFire;
            currentWeapon.OnStateChanged -= StartStateAnimation;
            isSubscribed = false;
        }
        private void StartStateAnimation(int newState)
        {
            statesAnimation[newState].ApplyState();
        }
        private void SpawnShootFire()
        {
            DestroyablePoolableObject obj = shootFirePool.GetObject();
            obj.transform.SetParent(shootFireStart.parent);
            obj.transform.localPosition = shootFireStart.localPosition;
            obj.transform.forward = shootFireStart.forward;
        }
        private void SpawnCartridgeCaseAtReload()
        {
            BulletsInfo bullets = model.Context.CurrentWeapon.BulletsInfo;
            int neededReloadAmount = bullets.BulletsAtClip - bullets.BulletsCurrent;
            int possibleProvidedBullets = Mathf.Min(neededReloadAmount, bullets.BulletsTotal);
            int reloadBulletsCount = Mathf.Max(possibleProvidedBullets, -1);
            int bulletCounter = 0;
            foreach (var startTransform in cartridgeCaseReloadStart)
            {
                if (reloadBulletsCount > -1 && bulletCounter >= reloadBulletsCount) break;
                CartridgeCasePoolable cartridgeCase = (CartridgeCasePoolable)cartridgeCaseReloadPool.GetObject();
                cartridgeCase.transform.SetPositionAndRotation(startTransform.position, startTransform.rotation);
                Vector3 forceDirection = cartridgeCaseReloadDirection.position - startTransform.position;
                cartridgeCase.Rigidbody.AddForce(forceDirection * cartridgeCaseReloadForce, ForceMode.Acceleration);
                bulletCounter++;
            }
        }
        private void SpawnCartridgeCaseAtShoot()
        {
            CartridgeCasePoolable cartridgeCase = (CartridgeCasePoolable)cartridgeCaseShootPool.GetObject();
            cartridgeCase.transform.SetPositionAndRotation(cartridgeCaseShootStart.position, cartridgeCaseShootStart.rotation);
            Vector3 forceDirection = cartridgeCaseShootDirection.position - cartridgeCaseShootStart.position;
            cartridgeCase.Rigidbody.AddForce(forceDirection * cartridgeCaseShootForce, ForceMode.Acceleration);
        }
        private void StartRecoil()
        {
            isRecoilStarted = true;
            Vector3 startPosition = recoilPivot.localPosition;
            Vector3 endPosition = recoilEnd.localPosition;

            Quaternion startRotation = recoilPivot.localRotation;
            Quaternion endRotation = recoilEnd.localRotation;

            recoilPositionLayer.ChangeVector(startPosition, endPosition, recoilTimeStart, x => recoilPivot.localPosition = x, null, null);
            recoilRotationLayer.ChangeQuaternion(startRotation, endRotation, recoilTimeStart, x => recoilPivot.localRotation = x, delegate { RevertRecoil(); }, null);
        }
        private void RevertRecoil()
        {
            isRecoilStarted = false;
            Vector3 startPosition = recoilPivot.localPosition;
            Vector3 endPosition = recoilStart.localPosition;

            Quaternion startRotation = recoilPivot.localRotation;
            Quaternion endRotation = recoilStart.localRotation;

            recoilPositionLayer.ChangeVector(startPosition, endPosition, recoilTimeRevert, x => recoilPivot.localPosition = x, null, delegate { return isRecoilStarted; });
            recoilRotationLayer.ChangeQuaternion(startRotation, endRotation, recoilTimeRevert, x => recoilPivot.localRotation = x, null, delegate { return isRecoilStarted; });
        }
        private void StartReloadAnimation()
        {
            List<Material> storedMaterials = new();
            foreach (var el in reloadRenderers)
            {
                storedMaterials.Add(el.material);
                el.material = reloadMaterial;
            }
            Weapon weapon = model.Context.CurrentWeapon;
            ValueTimeChanger vtc = new(0, 1, weapon.ReloadDelay.Delay, ValueTimeChanger.DefaultCurve,
                setValue: null,
                onEnd: delegate
                {
                    int rrCount = reloadRenderers.Count;
                    for (int i = 0; i < rrCount; ++i)
                    {
                        reloadRenderers[i].material = storedMaterials[i];
                    }
                },
                breakCondition: delegate { return !weapon.ReloadDelay.IsDelaying; },
                invokeEndAtBreak: true);
        }
        #endregion methods
        [System.Serializable]
        private class StateAnimation
        {
            [SerializeField] private List<Renderer> renderers;
            [SerializeField] private Material newMaterial;

            public void ApplyState()
            {
                foreach (var el in renderers)
                {
                    el.material = newMaterial;
                }
            }
        }
    }
}