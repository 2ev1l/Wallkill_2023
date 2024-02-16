using Data.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EditorCustom.Attributes;

namespace Game.Rooms
{
    [DisallowMultipleComponent]
    public class Door : WallBlock
    {
        #region fields & properties
        public UnityAction OnDoorOpened;
        public UnityAction OnDoorClosed;
        public UnityAction OnDoorClosedUIOnly;
        public UnityAction OnCurrentRoomStart;

        public bool CanOpen => canOpen;
        [SerializeField] private bool canOpen = true;
        public TransformDirection Direction => direction;
        [SerializeField] private TransformDirection direction;
        public Room AdjacentRoom => adjacentRoom;
        [SerializeField] private Room adjacentRoom;
        public bool IsOpened => isOpened;
        [Title("Read Only")]
        [SerializeField][ReadOnly] private bool isOpened;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (canOpen)
            {
                SubscribeAtRoomOpen(Room);
                SubscribeAtRoomStart();
            }
            else
                SubscribeAtRoomClose(Room);

            if (adjacentRoom != null)
                SetAdjacentRoom(adjacentRoom);
        }
        private void OnDisable()
        {
            UnSubscribeAtRoomOpen(Room);
            UnSubscribeAtRoomClose(Room);
            UnSubscribeAtRoomStart();
            if (AdjacentRoom != null)
            {
                UnSubscribeAtRoomOpen(AdjacentRoom);
                UnSubscribeAtRoomClose(AdjacentRoom);
            }
        }
        public void SetAdjacentRoom(Room room)
        {
            //unsubscribe
            if (AdjacentRoom != null)
            {
                UnSubscribeAtRoomOpen(AdjacentRoom);
                UnSubscribeAtRoomClose(AdjacentRoom);
            }

            adjacentRoom = room;
            if (canOpen)
                SubscribeAtRoomOpen(AdjacentRoom);
            else
                SubscribeAtRoomClose(AdjacentRoom);
        }
        private void InvokeAtCurrentRoomStart() => OnCurrentRoomStart?.Invoke();
        private void SubscribeAtRoomStart()
        {
            Room.OnStart += InvokeAtCurrentRoomStart;
            Room.OnStart += TryClose;
        }
        private void UnSubscribeAtRoomStart()
        {
            Room.OnStart -= InvokeAtCurrentRoomStart;
            Room.OnStart -= TryClose;
        }

        private void SubscribeAtRoomOpen(Room room)
        {
            room.OnCompleted += TryOpen;
        }
        private void UnSubscribeAtRoomOpen(Room room)
        {
            room.OnCompleted -= TryOpen;
        }
        private void SubscribeAtRoomClose(Room room)
        {
            room.OnCompleted += TryClose;
        }
        private void UnSubscribeAtRoomClose(Room room)
        {
            room.OnCompleted -= TryClose;
        }
        [SerializedMethod]
        public void DisableOpening()
        {
            if (!canOpen) return;
            canOpen = false;
            UnSubscribeAtRoomOpen(Room);
            UnSubscribeAtRoomStart();
            SubscribeAtRoomClose(Room);
            if (AdjacentRoom != null)
            {
                UnSubscribeAtRoomOpen(AdjacentRoom);
                SubscribeAtRoomClose(AdjacentRoom);
            }
        }
        [SerializedMethod]
        public void TryOpen()
        {
            if (!canOpen || isOpened) return;
            Open();
            return;
        }
        public void TryClose()
        {
            if (!isOpened)
            {
                OnDoorClosedUIOnly?.Invoke();
                return;
            }
            Close();
            return;
        }

        [SerializedMethod]
        public void Open()
        {
            isOpened = true;
            OnDoorOpened?.Invoke();
        }

        public void Close()
        {
            isOpened = false;
            OnDoorClosed?.Invoke();
        }
        #endregion methods
    }
}