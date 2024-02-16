using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.UI.Triggers;

namespace Game.Rooms
{
    public class PickupableItem : Pickupable
    {
        #region fields & properties
        public int Count => count;
        [SerializeField][Min(0)] private int count = 1;
        #endregion fields & properties

        #region methods
        public override void PickUp()
        {
            InstancesProvider.Instance.PlayerInventory.AddItem(Id, count);
            base.PickUp();
        }
        #endregion methods
    }
}