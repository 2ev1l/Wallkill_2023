using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorial
{
    public class StageMoving : TutorialStageUI
    {
        #region fields & properties
        [SerializeField] private List<KeyProgressBar> progressBars;
        private int progressBarsFillCount = 0;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            progressBars.ForEach(x => x.OnKeyFilled += IncreaseFillCount);
        }
        private void OnDisable()
        {
            progressBars.ForEach(x => x.OnKeyFilled -= IncreaseFillCount);
        }
        private void IncreaseFillCount()
        {
            progressBarsFillCount++;
            if (progressBarsFillCount == progressBars.Count)
            {
                CompleteStage();
            }
        }
        #endregion methods
    }
}