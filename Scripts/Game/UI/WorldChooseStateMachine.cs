using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public class WorldChooseStateMachine : DefaultStateMachine
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            CheckInteraction();
        }
        private void CheckInteraction()
        {
            WorldsData worldsData = GameData.Data.WorldsData;
            foreach (WorldChooseState state in Context.States.Cast<WorldChooseState>())
            {
                WorldStats worldStats = worldsData.GetWorldStats(state.WorldType);
                state.SetInteraction(worldStats.IsOpened);
            }
        }
        #endregion methods
    }
}