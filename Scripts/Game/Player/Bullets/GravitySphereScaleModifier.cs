using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Bullets
{
    public class GravitySphereScaleModifier : ModifierBehaviour
    {
        #region fields & properties
        protected override int ReferenceId => 3;
        [SerializeField] private GravitySphereSpawner gravitySphereSpawner;
        private float currentScale = 1f;
        #endregion fields & properties

        #region methods
        protected override void SetDefaultModifierParams()
        {
            currentScale = 1f;
        }

        protected override void SetModifierParams(int rank)
        {
            currentScale = rank switch
            {
                0 => 1.5f,
                1 => 2f,
                _ => DebugUnknownModifier(1f)
            };
            gravitySphereSpawner.SphereScale = currentScale;
        }
        #endregion methods
    }
}