using Data.Settings;
using Game.DataBase;
using Game.Player;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using TMPro;
using UnityEngine;
using Universal;
using Universal.UI;
using Universal.UI.Layers;

namespace Game.UI
{
    public class WeaponBars : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Attack playerAttack;
        private Weapon CurrentWeapon => playerAttack.CurrentWeapon;

        [Title("Center")]
        [SerializeField] private Transform crosshairCenter;
        [SerializeField] private List<Transform> additionalCenterFollowedPositions;
        [SerializeField] private float centerMoveScale = 10f;
        [SerializeField] private float additionalMoveScale = 2f;
        [SerializeField] private float rotateScale = 5f;

        [Title("Bars")]
        [SerializeField] private ProgressBar shootBar;
        [SerializeField] private ProgressBar reloadBar;

        [Title("Weapons UI")]
        [SerializeField] private WeaponStats additionalCurrentStats;
        [SerializeField] private WeaponStats currentWeaponStats;
        [SerializeField] private WeaponStats nextWeaponStats;
        [SerializeField] private WeaponStats prevWeaponStats;
        [SerializeField] private Transform endWeaponPosition;
        [SerializeField] private MessageReceiver messageReceiver;

        private Vector3 nextWeaponPosition;
        private Vector3 prevWeaponPosition;
        private Vector3 currentWeaponPosition;
        private bool isPositionsInitialized = false;
        private SmoothLayers<VectorLayer> weaponMoveLayers = new();
        private bool isSubscribedAtWeapon = false;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            StorePositions();
            isPositionsInitialized = true;
        }
        private void StorePositions()
        {
            Transform next = nextWeaponStats.gameObject.transform;
            Transform curr = currentWeaponStats.gameObject.transform;
            Transform prev = prevWeaponStats.gameObject.transform;

            currentWeaponPosition = curr.localPosition;
            prevWeaponPosition = prev.localPosition;
            nextWeaponPosition = next.localPosition;
        }
        private void ResetPositions()
        {
            Transform next = nextWeaponStats.gameObject.transform;
            Transform curr = currentWeaponStats.gameObject.transform;
            Transform prev = prevWeaponStats.gameObject.transform;

            prev.localPosition = prevWeaponPosition;
            next.localPosition = nextWeaponPosition;
            curr.localPosition = currentWeaponPosition;
        }
        private void OnEnable()
        {
            playerAttack.OnWeaponChanged += MoveWeaponsUI;
            playerAttack.OnWeaponChanged += SubscribeAtWeapon;
            playerAttack.OnBeforeWeaponChanged += UnSubscribeAtWeapon;
            SubscribeAtWeapon(playerAttack.CurrentWeapon);
            SetWeaponStats();
            if (isPositionsInitialized)
                StorePositions();
        }
        private void OnDisable()
        {
            playerAttack.OnWeaponChanged -= MoveWeaponsUI;
            playerAttack.OnWeaponChanged -= SubscribeAtWeapon;
            playerAttack.OnBeforeWeaponChanged -= UnSubscribeAtWeapon;
            UnSubscribeAtWeapon(playerAttack.CurrentWeapon);
            ResetPositions();
        }
        private void HideBars() => ChangeBarsState(false);
        private void ShowBars() => ChangeBarsState(true);
        private void ChangeBarsState(bool isActive)
        {
            shootBar.gameObject.SetActive(isActive);
            reloadBar.gameObject.SetActive(isActive);
        }
        private void SubscribeAtWeapon(Weapon newWeapon)
        {
            if (isSubscribedAtWeapon || newWeapon == null) return;

            CurrentWeapon.ShootDelay.OnTimeLasts += OnShootDelaying;
            CurrentWeapon.ReloadDelay.OnTimeLasts += OnReloadDelaying;
            CurrentWeapon.ReloadDelay.OnDelayBreak += ResetBars;
            playerAttack.OnOutOfAmmo += CheckCurrentBullets;
            ResetBars();
            isSubscribedAtWeapon = true;
        }
        private void UnSubscribeAtWeapon(Weapon oldWeapon)
        {
            if (!isSubscribedAtWeapon || oldWeapon == null) return;

            CurrentWeapon.ShootDelay.OnTimeLasts -= OnShootDelaying;
            CurrentWeapon.ReloadDelay.OnTimeLasts -= OnReloadDelaying;
            CurrentWeapon.ReloadDelay.OnDelayBreak -= ResetBars;
            playerAttack.OnOutOfAmmo -= CheckCurrentBullets;

            isSubscribedAtWeapon = false;
        }
        private void CheckCurrentBullets()
        {
                SendOutOfAmmoMessage();
        }
        private void SendOutOfAmmoMessage() => messageReceiver.ReceiveMessage(MessageType.OutOfAmmo.GetMessage());
        private void MoveWeaponsUI(Weapon newWeapon)
        {
            MoveWeaponsUI(nextWeaponStats.Weapon == newWeapon);
        }
        private void MoveWeaponsUI(bool direction) => StartCoroutine(WeaponsUIMove(direction));
        private IEnumerator WeaponsUIMove(bool direction)
        {
            Transform next = nextWeaponStats.gameObject.transform;
            Transform curr = currentWeaponStats.gameObject.transform;
            Transform prev = prevWeaponStats.gameObject.transform;

            VectorLayer prevLayer = weaponMoveLayers.GetLayer(0);
            VectorLayer currentLayer = weaponMoveLayers.GetLayer(1);
            VectorLayer nextLayer = weaponMoveLayers.GetLayer(2);
            float time = playerAttack.SwitchWeaponDelay.Delay / 2f;
            
            //move to positions
            if (direction)
            {
                nextLayer.ChangeVector(nextWeaponPosition, currentWeaponPosition, time, x => next.localPosition = x, delegate { }, delegate { return !enabled; });
                currentLayer.ChangeVector(currentWeaponPosition, prevWeaponPosition, time, x => curr.localPosition = x, delegate { }, delegate { return !enabled; });
                prevLayer.ChangeVector(prevWeaponPosition, endWeaponPosition.localPosition, time, x => prev.localPosition = x, delegate { ResetPositions(); }, delegate { return !enabled; });
            }
            else
            {
                nextLayer.ChangeVector(nextWeaponPosition, endWeaponPosition.localPosition, time, x => next.localPosition = x, delegate { }, delegate { return !enabled; });
                currentLayer.ChangeVector(currentWeaponPosition, nextWeaponPosition, time, x => curr.localPosition = x, delegate { }, delegate { return !enabled; });
                prevLayer.ChangeVector(prevWeaponPosition, currentWeaponPosition, time, x => prev.localPosition = x, delegate { ResetPositions(); }, delegate { return !enabled; });
            }

            yield return new WaitForSeconds(time);
            //reset all positions and events
            SetWeaponStats();
        }
        private void SetWeaponStats()
        {
            currentWeaponStats.Weapon = CurrentWeapon;
            nextWeaponStats.Weapon = playerAttack.NextWeapon;
            prevWeaponStats.Weapon = playerAttack.PrevWeapon;
        }
        private void ResetBars()
        {
            additionalCurrentStats.Weapon = CurrentWeapon;
            shootBar.MinMaxValues = new(0, CurrentWeapon.ShootDelay.Delay);
            reloadBar.MinMaxValues = new(0, CurrentWeapon.ReloadDelay.Delay);
            OnShootDelaying(CurrentWeapon.ShootDelay.CanActivate ? 0 : CurrentWeapon.ShootDelay.Delay);
            OnReloadDelaying(CurrentWeapon.BulletsInfo.NeedReloading ? reloadBar.MinMaxValues.y : 0);
        }
        private void OnShootDelaying(float timeLasts)
        {
            shootBar.Value = shootBar.MinMaxValues.y - timeLasts;
        }
        private void OnReloadDelaying(float timeLasts)
        {
            reloadBar.Value = reloadBar.MinMaxValues.y - timeLasts;
        }
        private void MoveCenterToCursor()
        {
            ///0..1
            Vector2 pos = CursorSettings.LastMousePointOnScreenScaled;

            ///-1..1
            Vector2 scaledPos = CustomMath.ConvertVectorFromTo(Vector2.up, pos, new(-1, 1));
            SetFlatPosition(crosshairCenter, scaledPos, centerMoveScale);

            Vector2 scaledAngle = new(scaledPos.y, -scaledPos.x);
            Quaternion newRotation = Quaternion.Euler(scaledAngle.x * rotateScale, scaledAngle.y * rotateScale, 0);
            crosshairCenter.localRotation = newRotation;

            for(int i =0; i < additionalCenterFollowedPositions.Count; ++i)
            {
                SetFlatPosition(additionalCenterFollowedPositions[i], scaledPos, additionalMoveScale);
            }
        }
        private void SetFlatPosition(Transform transform, Vector2 direction, float scale)
        {
            Vector3 newPosition = transform.localPosition;
            newPosition.x = direction.x * scale;
            newPosition.y = direction.y * scale;
            newPosition.z = 0;
            transform.localPosition = newPosition;
        }
        private void Update()
        {
            MoveCenterToCursor();
        }
        #endregion methods
    }
}