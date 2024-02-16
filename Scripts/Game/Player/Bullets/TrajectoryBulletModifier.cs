using EditorCustom.Attributes;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.Player.Bullets
{
    public class TrajectoryBulletModifier : ModifierBehaviour
    {
        #region fields & properties
        protected override int ReferenceId => 1;
        [SerializeField] private ObjectPool<Bullet> bullets = new();
        [SerializeField] private bool spawnOnKey = true;
        [SerializeField][DrawIf(nameof(spawnOnKey), false)][Min(0)] private float spawnTime = 0.5f;
        #endregion fields & properties

        #region methods
        protected override void OnDisable()
        {
            base.OnDisable();
            CancelInvoke(nameof(SpawnBullet));
        }
        protected override void OnModifierActivateRequest()
        {
            if (!spawnOnKey) return;
            if (ReferenceModifier == null) return;
            SpawnBullet();
        }
        protected override void SetDefaultModifierParams()
        {
            SetModifierParams(-1);
        }
        protected override void SetModifierParams(int rank)
        {
            if (spawnOnKey) return;
            if (rank >= 0)
                InvokeRepeating(nameof(SpawnBullet), 0.1f, spawnTime);
            else
                CancelInvoke(nameof(SpawnBullet));
        }
        private void SpawnBullet()
        {
            Bullet bullet = bullets.GetObject();
            bullet.Init(InstancesProvider.Instance.PlayerAttack);
            bullet.MaxCollisionHits = ReferenceModifier.Rank switch
            {
                0 => 2,
                1 => 3,
                2 => 4,
                _ => DebugUnknownModifier(1)
            };
        }

        #endregion methods
    }
}