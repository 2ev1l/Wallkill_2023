using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.Player.Bullets
{
    public class GravitySpherePreviewModifier : ModifierBehaviour
    {
        #region fields & properties
        protected override int ReferenceId => 4;
        [SerializeField] private ObjectPool<Bullet> bullets = new();
        #endregion fields & properties

        #region methods
        protected override void OnModifierActivateRequest()
        {
            if (ReferenceModifier == null) return;
            SpawnBullet();
        }
        protected override void SetDefaultModifierParams() { }

        protected override void SetModifierParams(int rank) { }
        private void SpawnBullet()
        {
            Bullet bullet = bullets.GetObject();
            bullet.Init(InstancesProvider.Instance.PlayerAttack);
        }
        #endregion methods
    }
}