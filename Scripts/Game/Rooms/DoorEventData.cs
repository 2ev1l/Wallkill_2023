using System.Collections.Generic;
using UnityEngine;
using EditorCustom.Attributes;

namespace Game.Rooms
{
    [System.Serializable]
    public class DoorEventData
    {
        #region fields & properties
        public Door Door => door;
        [SerializeField] private Door door;
        #endregion fields & properties

        #region methods
        public void SetAdjacentDoor(Door door, Room adjacentRoom)
        {
            door.SetAdjacentRoom(adjacentRoom);
        }
        #endregion methods
    }
}