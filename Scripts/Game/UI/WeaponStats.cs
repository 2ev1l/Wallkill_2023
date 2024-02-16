using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class WeaponStats : MonoBehaviour
    {
        #region fields & properties
        public Weapon Weapon
        {
            get => weapon;
            set
            {
                Unsubscribe();
                weapon = value;
                Subscribe();
            }
        }
        private Weapon weapon;

        [SerializeField] private TextMeshProUGUI weaponName;
        [SerializeField] private TextMeshProUGUI modifiers;
        [SerializeField] private TextMeshProUGUI currentBullets;
        [SerializeField] private TextMeshProUGUI totalBullets;
        [SerializeField] private TextMeshProUGUI damage;
        private static readonly LanguageInfo outOfAmmoText = new(6, Universal.UI.TextType.Game);
        private bool isSubscribed = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            Subscribe();
        }
        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (isSubscribed || Weapon == null) return;
            Weapon.BulletsInfo.OnCurrentBulletsChanged += SetCurrentBullets;
            Weapon.BulletsInfo.OnTotalBulletsChanged += SetTotalBullets;
            SetValues();
            isSubscribed = true;
        }
        private void Unsubscribe()
        {
            if (!isSubscribed || Weapon == null) return;
            Weapon.BulletsInfo.OnCurrentBulletsChanged -= SetCurrentBullets;
            Weapon.BulletsInfo.OnTotalBulletsChanged -= SetTotalBullets;
            isSubscribed = false;
        }
        private void SetValues()
        {
            SetCurrentBullets(Weapon.BulletsInfo.BulletsCurrent);
            SetTotalBullets(Weapon.BulletsInfo.BulletsTotal);
            SetDamage();
            SetName();
            SetModifiers();
        }
        private void SetModifiers() => modifiers.text = "";
        private void SetName() => weaponName.text = Weapon.Name.Text;
        private void SetDamage() => damage.text = Weapon.Damage.ToString();
        private void SetCurrentBullets(int value)
        {
            if (Weapon.BulletsInfo.BulletsTotal == 0 && value == 0)
            {
                currentBullets.text = outOfAmmoText.Text;
                return;
            }
            currentBullets.text = $"{IntToString(value)}/{IntToString(Weapon.BulletsInfo.BulletsAtClip)}";
        }
        private void SetTotalBullets(int value)
        {
            if (value == 0)
            {
                totalBullets.text = outOfAmmoText.Text;
                return;
            }
            totalBullets.text = $"{IntToString(value)}/{IntToString(Weapon.BulletsInfo.BulletsMax)}";
        }
        private string IntToString(int value) => value switch
        {
            -1 => "~",
            _ => value.ToString()
        };
        #endregion methods
    }
}