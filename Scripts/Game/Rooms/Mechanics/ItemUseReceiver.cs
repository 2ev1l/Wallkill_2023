using DebugStuff;
using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Rooms.Mechanics
{
    public class ItemUseReceiver : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnItemUsedEvent;
        public int ItemId => itemId;
        [SerializeField][Min(0)] private int itemId;
        #endregion fields & properties

        #region methods
        public bool CanUse(int itemId) => itemId == this.itemId;
        public void UseThis()
        {
            OnItemUsedEvent?.Invoke();
        }
        #endregion methods


#if UNITY_EDITOR
        private void Awake()
        {
            if (DB.Instance.Items.GetObjectById(itemId) == null)
                Debug.LogError($"Can't get item #{itemId} in {gameObject.name}", this);
        }
#endif //UNITY_EDITOR

    }
}