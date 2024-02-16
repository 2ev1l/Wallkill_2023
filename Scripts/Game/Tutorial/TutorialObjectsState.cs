using Data.Stored;
using EditorCustom.Attributes;
using Game.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialObjectsState : ObjectsState
    {
        #region fields & properties
        [SerializeField] private bool doNotEnable;
        [SerializeField][DrawIf(nameof(doNotEnable), false)] private TutorialStage stageToEnableObjects;
        [SerializeField] private bool disableInstantly;
        [SerializeField][DrawIf(nameof(disableInstantly), false)] private TutorialStage stageToDisableObjects;
        [SerializeField] private bool stateOnTutorialEnd = true;
        private TutorialData Context => GameData.Data.TutorialData;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (Context.IsCompleted)
            {
                EnableObjects();
                return;
            }

            Context.OnStageChanged += CheckCurrentTutorialStage;
            if (disableInstantly)
                DisableObjects();
            else
                CheckCurrentTutorialStage(Context.CurrentStage);
        }
        private void OnDisable()
        {
            Context.OnStageChanged -= CheckCurrentTutorialStage;
        }
        private void CheckCurrentTutorialStage(TutorialStage currentStage)
        {
            if (Context.IsCompleted) return;
            if (currentStage == stageToEnableObjects)
            {
                if (!doNotEnable)
                    EnableObjects();
            }
            if (currentStage == stageToDisableObjects) DisableObjects();
        }
        #endregion methods
    }
}