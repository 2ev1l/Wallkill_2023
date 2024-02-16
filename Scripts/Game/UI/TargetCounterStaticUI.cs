using Game.Rooms;
using Game.Rooms.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public class TargetCounterStaticUI : RoomMechanicsUI
    {
        #region fields & properties
        [SerializeField] private ProgressBar progressBar;
        #endregion fields & properties

        #region methods
        public override void DisableUI()
        {
            progressBar.gameObject.SetActive(false);
        }
        public void EnableUI(int neededCount, int currentCount)
        {
            progressBar.MinMaxValues = Vector2.up * neededCount;
            progressBar.Value = currentCount;
            progressBar.gameObject.SetActive(true);
        }
        public void UpdateUI(int value)
        {
            progressBar.SetValue(value);
        }
        #endregion methods
    }
}