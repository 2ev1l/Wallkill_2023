using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Stored
{
    [System.Serializable]
    public class WorldStats
    {
        #region fields & properties
        public UnityAction<WorldType> OnOpened;
        public UnityAction<WorldType> OnCompleted;
        
        public WorldType World => world;
        [SerializeField] private WorldType world;
        public bool IsOpened => isOpened;
        [SerializeField] private bool isOpened = false;
        public bool IsCompleted => isCompleted;
        [SerializeField] private bool isCompleted = false;
        #endregion fields & properties

        #region methods
        public bool TryClose()
        {
            if (!isOpened) return false;
            isOpened = false;
            return true;
        }
        public bool TryOpen()
        {
            if (isOpened) return false;
            isOpened = true;
            OnOpened?.Invoke(world);
            return true;
        }
        public bool TryComplete()
        {
            if (isCompleted) return false;
            isCompleted = true;
            OnCompleted?.Invoke(world);
            return true;
        }
        public WorldStats(WorldType world, bool isOpened, bool isCompleted)
        {
            this.world = world;
            this.isOpened = isOpened;
            this.isCompleted = isCompleted;
        }
        #endregion methods
    }
}