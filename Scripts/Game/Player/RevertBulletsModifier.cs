using EditorCustom.Attributes;
using Game.Player.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.Player
{
    public class RevertBulletsModifier : ModifierBehaviour
    {
        #region fields & properties
        protected override int ReferenceId => 2;
        [SerializeField] private Attack attack;
        
        [Title("Read Only")]
        [SerializeField][ReadOnly] private int chanceToRevert = 0;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            attack.OnShoot += TryRevertBullets;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            attack.OnShoot -= TryRevertBullets;
        }
        private void TryRevertBullets()
        {
            if (chanceToRevert <= 0) return;
            if (!CustomMath.GetRandomChance(chanceToRevert)) return;
            attack.CurrentWeapon.BulletsInfo.AddTotalBullets(1);
        }
        protected override void SetDefaultModifierParams()
        {
            chanceToRevert = 0;
        }

        protected override void SetModifierParams(int rank)
        {
            chanceToRevert = rank switch
            {
                0 => 10,
                1 => 20,
                _ => DebugUnknownModifier(0)
            };
        }
        #endregion methods
    }
}