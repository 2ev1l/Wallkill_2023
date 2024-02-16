using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms
{
    public class PickupableWeapon : Pickupable
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void PickUp()
        {
            GameData.Data.PlayerData.OpenedWeapons.TryOpenItem(Id);
            base.PickUp();
        }
        #endregion methods
    }
}