using Data.Stored;
using DebugStuff;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Rooms.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.Player
{
    public class ItemsUse : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Attack attack;
        [SerializeField] private Moving moving;

        [SerializeField] private LayerMask playerTeleportIgnoreMask;
        [SerializeField] private ObjectPool<DestroyablePoolableObject> spatialEffectPool;
        [SerializeField][Min(0)] private float checkSphereRadius = 1f;
        #endregion fields & properties

        #region methods
        public void UseItem(int id)
        {
            switch (id)
            {
                case 0: IncreasePlayerHealth(30); break;
                case 1: RefillWeaponAmmo(attack.CurrentWeapon); break;
                case 2: RefillAllWeaponsAmmo(); break;
                case 3: IncreasePlayerStamina(30); break;
                case 4:
                    if (!CanUseByOverlapSphere(10))
                        AddWalletGears(1);
                    else
                        UseByOverlapSphere(10);
                    break;
                case 5: UseItem5(); break;
                case 6: UseByOverlapSphere(6); break;
                case 7: UseItem7(); break;
                case 8: UseByOverlapSphere(8); break;
                case 9: UseByOverlapSphere(9); break;
                case 10: UseByOverlapSphere(10); break;
                case 11: IncreaseMaxHealth(5); break;
                case 12: RefillWeaponAmmo(attack.CurrentWeapon); break;
                case 13: GameData.Data.PlayerData.Wallet.TrySpent(1); UseByOverlapSphere(10); break;
                default: Debug.Log($"No action for item #{id}", this); break;
            }
        }
        public bool CanUseItem(int id) => id switch
        {
            0 => !StatsBehaviour.Stats.Health.IsReachedMaximum(),
            1 => attack.CurrentWeapon.Type != WeaponType.Hand && attack.CurrentWeapon.BulletsInfo.BulletsTotal != attack.CurrentWeapon.BulletsInfo.BulletsMax,
            3 => !StatsBehaviour.Stats.Stamina.IsReachedMaximum(),
            6 => CanUseByOverlapSphere(6),
            7 => attack.CanChangeWeaponState(),
            8 => CanUseByOverlapSphere(8),
            9 => CanUseByOverlapSphere(9),
            10 => CanUseByOverlapSphere(10),
            12 => attack.CurrentWeapon.Type != WeaponType.Hand && attack.CurrentWeapon.BulletsInfo.BulletsTotal != attack.CurrentWeapon.BulletsInfo.BulletsMax,
            13 => GameData.Data.PlayerData.Wallet.CanSpent(1),
            _ => true
        };
        private List<ItemUseReceiver> GetCurrentReceivers()
        {
            Collider[] catchedColliders = Physics.OverlapSphere(transform.position + Vector3.up, checkSphereRadius);
            List<ItemUseReceiver> receivers = new();
            foreach (var el in catchedColliders)
            {
                if (!el.TryGetComponent(out ItemUseReceiver itemUseReceiver)) continue;
                receivers.Add(itemUseReceiver);
            }
            return receivers;
        }
        /// <summary>
        /// Can't use if object has multiple item use receiver components
        /// </summary>
        /// <param name="id"></param>
        private void UseByOverlapSphere(int id)
        {
            GetCurrentReceivers().Exists(x => x.CanUse(id), out ItemUseReceiver exist);
            if (exist == null) return;
            exist.UseThis();
        }
        private bool CanUseByOverlapSphere(int id)
        {
            return GetCurrentReceivers().Exists(x => x.CanUse(id), out _);
        }
        private void IncreaseMaxHealth(int amount) => StatsBehaviour.Stats.Health.ChangeMaxRange(StatsBehaviour.Stats.Health.GetRange().y + amount, false);
        private void UseItem7()
        {
            attack.TryChangeWeaponState();
        }
        private void UseItem5()
        {
            DestroyablePoolableObject effect = spatialEffectPool.GetObject();
            effect.transform.position = attack.CharacterController.transform.position;

            TeleportForward(1.5f);

            effect = spatialEffectPool.GetObject();
            effect.transform.position = attack.CharacterController.transform.position;
        }
        private void TeleportForward(float amount) => moving.TeleportToIgnoreLayer(attack.CharacterController.transform.position + attack.CharacterController.transform.forward * amount, playerTeleportIgnoreMask);
        private void AddWalletGears(int amount) => GameData.Data.PlayerData.Wallet.AddValue(amount);
        private void RefillAllWeaponsAmmo()
        {
            foreach (Weapon weapon in attack.StoredWeapons)
            {
                RefillWeaponAmmo(weapon);
            }
        }
        private void RefillWeaponAmmo(Weapon weapon)
        {
            weapon.BulletsInfo.ResetTotalBulletsToMax();
        }
        private void IncreasePlayerStamina(int amount) => StatsBehaviour.Stats.Stamina.TryIncreaseValue(amount, out _);
        private void IncreasePlayerHealth(int amount) => StatsBehaviour.Stats.Health.TryIncreaseValue(amount, out _);
        private void DecreasePlayerHealth(int amount) => StatsBehaviour.Stats.Health.TryDecreaseValue(amount, out _);
        #endregion methods

#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField] private bool doDebug = true;
        [SerializeField][DrawIf(nameof(doDebug), true)] private bool debugAlways = false;

        private void OnDrawGizmosSelected()
        {
            if (!doDebug) return;
            if (debugAlways) return;
            DebugDraw();
        }
        private void OnDrawGizmos()
        {
            if (!doDebug) return;
            if (!debugAlways) return;
            DebugDraw();
        }

        private void DebugDraw()
        {
            Gizmos.DrawWireSphere(transform.position + Vector3.up, checkSphereRadius);
        }

#endif //UNITY_EDITOR

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private bool ___testBool;
        [SerializeField][Min(0)] private int itemToTest = 5;

        [Button(nameof(TestItemUse))]
        private void TestItemUse()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            if (!CanUseItem(itemToTest))
            {
                print("Cant use this");
                return;
            }
            UseItem(itemToTest);
        }
#endif //UNITY_EDITOR
    }
}