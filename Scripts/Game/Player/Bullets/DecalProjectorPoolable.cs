using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Universal;

namespace Game.Player.Bullets
{
    public class DecalProjectorPoolable : DestroyablePoolableObject
{
        #region fields & properties
        public DecalProjector DecalProjector => decalProjector;
        [SerializeField] private DecalProjector decalProjector;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}