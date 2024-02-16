using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Rooms
{
    public class RoomEvents : MonoBehaviour
    {
        #region fields & properties
        [HelpBox("Events will be called only when component is active", HelpBoxMessageType.Info)]
        public UnityEvent OnRoomStartEvents;
        public UnityEvent OnRoomCompleteEvents;
        [SerializeField][Required] private Room room;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            room.OnStart += InvokeRoomStart;
            room.OnCompleted += InvokeRoomComplete;
        }
        private void OnDisable()
        {
            room.OnStart -= InvokeRoomStart;
            room.OnCompleted -= InvokeRoomComplete;
        }
        private void InvokeRoomStart()
        {
            OnRoomStartEvents?.Invoke();
        }
        private void InvokeRoomComplete()
        {
            OnRoomCompleteEvents?.Invoke();
        }
        #endregion methods
    }
}