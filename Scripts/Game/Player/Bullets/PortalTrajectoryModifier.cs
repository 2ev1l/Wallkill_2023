using EditorCustom.Attributes;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.Player.Bullets
{
    public class PortalTrajectoryModifier : ModifierBehaviour
    {
        #region fields & properties
        protected override int ReferenceId => 6;
        [SerializeField] private ObjectPool<Bullet> bullets = new();
        #endregion fields & properties

        #region methods
        protected override void OnDisable()
        {
            base.OnDisable();
            CancelInvoke(nameof(SpawnBullet));
        }
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