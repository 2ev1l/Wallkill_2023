using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Bullets
{
    public class RicochetModifier : ModifierBehaviour
    {
        #region fields & properties
        protected override int ReferenceId => 0;
        protected Bullet Bullet => bullet;
        [SerializeField] private Bullet bullet;
        #endregion fields & properties

        #region methods
        protected override void SetDefaultModifierParams()
        {
            Bullet.MaxCollisionHits = 2;
        }
        protected override void SetModifierParams(int rank)
        {
            Bullet.MaxCollisionHits = rank switch
            {
                0 => 3, //2 ricochets
                1 => 4, //3 ricochets
                2 => 5, //4 ricochets
                _ => DebugUnknownModifier(),
            };
        }
        private int DebugUnknownModifier()
        {
            Debug.LogError($"Undefined modifier rank #{ReferenceModifier.Rank} with id #{ReferenceId}", this);
            return 2;
        }
        #endregion methods
    }
}