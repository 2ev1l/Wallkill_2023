using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms
{
    [DisallowMultipleComponent]
    public class WallBlock : MonoBehaviour
    {
        #region fields & properties
        public Room Room => room;
        [SerializeField] private Room room;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}