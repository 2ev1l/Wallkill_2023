using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Stored
{
    [System.Serializable]
    public class TutorialData
    {
        #region fields & properties
        public UnityAction OnCompleted;
        public UnityAction<TutorialStage> OnStageChanged;
        public bool IsCompleted => isCompleted;
        [SerializeField] private bool isCompleted = false;

        public TutorialStage CurrentStage
        {
            get => currentStage;
            set => SetCurrentStage(value);
        }
        [SerializeField] private TutorialStage currentStage = TutorialStage.WakeUp;
        #endregion fields & properties

        #region methods
        private void SetCurrentStage(TutorialStage value)
        {
            currentStage = value;
            OnStageChanged?.Invoke(currentStage);
        }
        public void TryComplete()
        {
            if (isCompleted) return;
            isCompleted = true;
            OnCompleted?.Invoke();
        }
        #endregion methods
    }
}