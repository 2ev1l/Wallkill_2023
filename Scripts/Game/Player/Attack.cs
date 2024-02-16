using Data.Stored;
using Game.DataBase;
using Game.Player.Bullets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI;
using EditorCustom.Attributes;
using Game.Labyrinth;

namespace Game.Player
{
    public class Attack : MonoBehaviour
    {
        #region fields & properties
        public UnityAction<Weapon> OnWeaponChanged;
        public UnityAction<Weapon> OnBeforeWeaponChanged;

        public UnityAction<Weapon> OnBeforeWeaponRemoved;

        public UnityAction OnAimingStart;
        public UnityAction OnAimingEnd;

        public UnityAction OnShoot;
        public UnityAction OnReloadStart;
        public UnityAction OnOutOfAmmo;
        /// <summary>
        /// <see cref="{T0}"/> - stateId
        /// </summary>
        public UnityAction<int> OnWeaponStateChanged;

        public CharacterController CharacterController => characterController;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Moving moving;
        [SerializeField] private Jumping jumping;

        public TimeDelayContinuous SwitchWeaponDelay => switchWeaponDelay;
        [SerializeField] private TimeDelayContinuous switchWeaponDelay = new();
        [SerializeField] private Transform weaponOffset;
        [SerializeField] private Transform characterNeck;
        [SerializeField] private Transform aimingPosition;
        public LayerMask NearShootBlockMask => nearShootBlockMask;
        [SerializeField] private LayerMask nearShootBlockMask;
        [SerializeField] private StatChangeLayer staminaOnHandsAttack;
        public Weapon CurrentWeapon
        {
            get
            {
                if (currentWeapon == null)
                {
                    currentWeapon = Weapons[0];
                    OnWeaponChanged?.Invoke(currentWeapon);
                }
                return currentWeapon;
            }
        }
        private Weapon currentWeapon = null;
        public Weapon NextWeapon => GetWeapon(true);
        public Weapon PrevWeapon => GetWeapon(false);
        public bool IsAiming => isAiming;
        [Title("Read Only")][SerializeField][ReadOnly] private bool isAiming = false;
        public Ray AimingRay => new(CurrentWeapon.Model.Instantiated.BulletPosition, aimingPosition.position - CurrentWeapon.Model.Instantiated.BulletPosition);
        public Ray BulletTrajectoryRay => new(CurrentWeapon.Model.Instantiated.BulletPosition, CurrentWeapon.Model.Instantiated.BulletTransform.forward);
        public IReadOnlyList<Weapon> StoredWeapons => Weapons;
        private List<Weapon> Weapons
        {
            get
            {
                if (weapons == null || weapons.Count == 0)
                {
                    weapons = new();
                    GetNewWeapons();
                }
                return weapons;
            }
        }
        private List<Weapon> weapons;

        private List<WeaponModel> instantiatedWeapons = new();
        private int weaponAtJumped = 0;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            InitLayers();
        }
        private void InitLayers()
        {
            staminaOnHandsAttack.Value = StatsBehaviour.Stats.Stamina;
        }
        private void OnEnable()
        {
            CursorSettings.OnMouseWheelDirectionChanged += ChangeWeaponFromStack;

            GameData.Data.PlayerData.OpenedWeapons.OnItemOpened += GetNewWeapon;
            GameData.Data.PlayerData.OpenedWeapons.OnItemClosed += RemoveOldWeapon;

            jumping.OnJumped += CheckWeaponAtJumped;
            jumping.OnFalled += CheckAimingAtFalled;

            GameData.Data.WorldsData.OnCurrentWorldChanged += OnWorldChanged;
            _ = CurrentWeapon;
        }

        private void OnDisable()
        {
            CursorSettings.OnMouseWheelDirectionChanged -= ChangeWeaponFromStack;

            GameData.Data.PlayerData.OpenedWeapons.OnItemOpened -= GetNewWeapon;
            GameData.Data.PlayerData.OpenedWeapons.OnItemClosed -= RemoveOldWeapon;

            jumping.OnJumped -= CheckWeaponAtJumped;
            jumping.OnFalled -= CheckAimingAtFalled;

            GameData.Data.WorldsData.OnCurrentWorldChanged -= OnWorldChanged;

            if (isAiming)
                TryEndAiming();
        }
        private void OnWorldChanged(WorldType _) => ResetAllBullets();
        private void ResetAllBullets()
        {
            foreach (Weapon el in Weapons)
            {
                el.BulletsInfo.ResetTotalBulletsToMax();
                el.BulletsInfo.ResetCurrentBulletsToClip();
            }
        }
        private void CheckAimingAtFalled()
        {
            Weapon oldWeapon = Weapons.Find(x => x.Id == weaponAtJumped);
            if (oldWeapon == null) return;
            if (!isAiming) return;
            if (weaponAtJumped == 0) return;
            ChangeWeapon(oldWeapon);
        }
        private void CheckWeaponAtJumped()
        {
            if (!isAiming) return;
            weaponAtJumped = CurrentWeapon.Id;
            if (weaponAtJumped == 0) return;
            ChangeWeapon(Weapons.Find(x => x.Id == 0));
        }
        private void SpawnBullet()
        {
            if (CurrentWeapon.BulletsInfo.Bullets.OriginalPrefab == null) return;
            Bullet bullet = CurrentWeapon.BulletsInfo.Bullets.GetObject();
            bullet.Init(this);
        }

        private void ShowWeapon() => ShowWeapon(CurrentWeapon);
        private void ShowWeapon(Weapon newWeapon)
        {
            WeaponModel model = null;
            if (!TryInstantiateWeapon(newWeapon, out model))
                TryFindInstantiatedWeaponModel(newWeapon, out model);
            instantiatedWeapons.ForEach(x => x.ChangeModelState(x == model));
        }
        private void HideWeapons()
        {
            instantiatedWeapons.ForEach(x => x.DisableModel());
        }
        private bool TryInstantiateWeapon(Weapon weapon, out WeaponModel model)
        {
            if (!weapon.Model.TryInstantiate(weaponOffset, out model)) return false;
            model.Init(this);
            instantiatedWeapons.Add(model);
            return true;
        }
        private void ClearInstantiatedModel(Weapon oldWeapon)
        {
            if (!TryFindInstantiatedWeaponModel(oldWeapon, out WeaponModel model)) return;
            instantiatedWeapons.Remove(model);
            Destroy(model);
        }
        private bool TryFindInstantiatedWeaponModel(Weapon weapon, out WeaponModel model)
        {
            model = instantiatedWeapons.Find(x => x == weapon.Model.Instantiated);
            if (model == null) return false;
            return true;
        }

        private void RemoveOldWeapon(int oldItemid)
        {
            Weapon removed = weapons.Find(x => x.Id == oldItemid);
            OnBeforeWeaponRemoved?.Invoke(removed);
            ClearInstantiatedModel(removed);
            weapons.Remove(removed);
            ChangeWeaponFromStack(false);
        }
        private void GetNewWeapon(int newItemId)
        {
            weapons.Add(DB.Instance.Weapons.GetObjectById(newItemId).Data.Clone());
        }
        private void GetNewWeapons()
        {
            OpenedItems openedWeapons = GameData.Data.PlayerData.OpenedWeapons;
            foreach (int weaponId in openedWeapons.ItemsId)
            {
                if (weapons.Exists(x => x.Id == weaponId)) continue;
                GetNewWeapon(weaponId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction">Is greater than 0 => next weapon, else => prev</param>
        public void ChangeWeaponFromStack(float direction)
        {
            if (switchWeaponDelay.CanActivate && isAiming && Weapons.Count > 1)
            {
                ChangeWeaponFromStack(direction > 0);
                switchWeaponDelay.Activate();
            }
        }
        private void ChangeWeaponFromStack(bool scrollUp)
        {
            Weapon next = GetWeapon(scrollUp);
            TryChangeWeapon(next);
        }
        private Weapon GetWeapon(bool positionUp)
        {
            int currentWeaponStackId = Weapons.FindIndex(x => x.Id == currentWeapon.Id);
            int totalWeapons = Weapons.Count;
            int nextWeaponStackId = (currentWeaponStackId + (positionUp ? 1 : -1)) % totalWeapons;
            if (nextWeaponStackId < 0) nextWeaponStackId = totalWeapons - 1;
            return weapons[nextWeaponStackId];
        }
        /// <summary>
        /// Changes to clone of original scriptable object
        /// </summary>
        /// <param name="id"></param>
        public void ChangeWeapon(int id) => ChangeWeapon(DB.Instance.Weapons.GetObjectById(id));
        /// <summary>
        /// Changes to clone of original scriptable object
        /// </summary>
        /// <param name="weaponOriginal"></param>
        public void ChangeWeapon(WeaponSO weaponOriginal)
        {
            TryChangeWeapon(weaponOriginal.Data.Clone());
        }
        public void TryChangeWeapon(Weapon clone)
        {
            if (jumping.IsJumping || moving.IsMoveControlled) return;
            ChangeWeapon(clone);
        }
        private void ChangeWeapon(Weapon clone)
        {
            TryBreakReloading();
            OnBeforeWeaponChanged?.Invoke(currentWeapon);
            HideWeapons();
            currentWeapon = clone;
            ShowWeapon();
            OnWeaponChanged?.Invoke(currentWeapon);
        }
        private bool IsSpaceBetweenGunFree()
        {
            if (CurrentWeapon.Type == WeaponType.Hand) return true;
            WeaponModel instantiated = CurrentWeapon.Model.Instantiated;
            if (instantiated == null) return true;
            Ray ray = new(characterNeck.position, instantiated.BulletPosition - characterNeck.position);
            float distance = Vector3.Distance(characterNeck.position, instantiated.BulletPosition);
            bool isCasted = Physics.Raycast(ray, out _, distance, nearShootBlockMask, QueryTriggerInteraction.Ignore);
            return !isCasted;
        }

        public void TryStartAiming()
        {
            if (moving.IsMoveControlled || isAiming || jumping.IsJumping) return;
            isAiming = true;
            ShowWeapon();
            OnAimingStart?.Invoke();
        }
        public void TryEndAiming()
        {
            if (!isAiming) return;
            isAiming = false;
            TryBreakReloading();
            OnAimingEnd?.Invoke();
            HideWeapons();
        }
        public bool TryShoot()
        {
            if (!IsAiming) return false;
            if (moving.IsMoveControlled) return false;
            if (jumping.IsJumping) return false;
            if (!IsSpaceBetweenGunFree()) return false;

            if (currentWeapon.Type == WeaponType.Hand && currentWeapon.CanShoot())
            {
                staminaOnHandsAttack.SetChangedAmountToSpeed();
                bool result = staminaOnHandsAttack.TryDecreaseStat(false, false);
                if (!result) return false;
            }

            if (!currentWeapon.TryShoot())
            {
                if (currentWeapon.BulletsInfo.NeedReloading) TryReload();
                return false;
            }
            SpawnBullet();
            OnShoot?.Invoke();
            return true;
        }
        public bool TryReload()
        {
            if (!IsAiming) return false;
            if (moving.IsMoveControlled) return false;
            if (jumping.IsJumping) return false;
            if (!currentWeapon.TryReload())
            {
                if (currentWeapon.BulletsInfo.BulletsTotal == 0)
                    OnOutOfAmmo?.Invoke();
                return false;
            }
            OnReloadStart?.Invoke();
            return true;
        }
        public bool TryBreakReloading()
        {
            if (currentWeapon == null) return false;
            if (!currentWeapon.ReloadDelay.TryBreakDelaying()) return false;
            return true;
        }
        public bool CanChangeWeaponState()
        {
            if (currentWeapon == null) return false;
            if (!IsAiming) return false;
            if (!currentWeapon.CanChangeWeaponState()) return false;
            return true;
        }
        public bool TryChangeWeaponState()
        {
            if (!CanChangeWeaponState()) return false;
            if (!currentWeapon.TryChangeState()) return false;
            OnWeaponStateChanged?.Invoke(currentWeapon.CurrentState);
            return true;
        }
        private void OnDrawGizmos()
        {
            if (characterController == null) return;
            try
            {
                Debug.DrawRay(AimingRay.origin, AimingRay.direction * 10f, Color.cyan);
                Debug.DrawRay(BulletTrajectoryRay.origin, BulletTrajectoryRay.direction * 10f, Color.blue);
            }
            catch { }
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField] private int weaponToTest = 0;
        [Button(nameof(TestAddWeapon))]
        private void TestAddWeapon() => GameData.Data.PlayerData.OpenedWeapons.TryOpenItem(weaponToTest);

        [SerializeField] private int modifierToTest = 0;
        [Button(nameof(TestAddModifier))]
        private void TestAddModifier() => GameData.Data.PlayerData.TryOpenModifier(modifierToTest, out _);
        [Button(nameof(TestIncreaseModifierRank))]
        private void TestIncreaseModifierRank()
        {
            GameData.Data.PlayerData.TryIncreaseModifierRank(modifierToTest);
        }
#endif //UNITY_EDITOR
    }
}