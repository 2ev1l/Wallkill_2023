using Data.Stored;
using Game.Labyrinth;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public abstract class RoomMechanicsUI : MonoBehaviour
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            WorldLoader.Instance.OnWorldLoaded += CheckWorldLoaded;
        }
        private void OnDisable()
        {
            WorldLoader.Instance.OnWorldLoaded -= CheckWorldLoaded;
        }
        private void CheckWorldLoaded(WorldType worldType)
        {
            if (worldType != WorldType.Portal) return;
            DisableUI();
        }
        public abstract void DisableUI();
        #endregion methods
    }
}