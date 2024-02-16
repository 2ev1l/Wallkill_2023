using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms
{
    public class DoorUI : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Door door;
        [SerializeField] private BoxCollider doorCollider;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            door.OnDoorOpened += OnOpened;
            door.OnDoorClosed += OnClosed;
        }
        private void OnDisable()
        {
            door.OnDoorOpened -= OnOpened;
            door.OnDoorClosed -= OnClosed;
        }
        private void OnOpened()
        {
            doorCollider.enabled = false;

        }
        private void OnClosed()
        {
            doorCollider.enabled = true;
        
        }
        #endregion methods
    }
}