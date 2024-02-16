using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Tutorial
{
    public class TutorialStageUI : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnStageCompleted;
        [SerializeField] private bool disableStageOnComplete;
        #endregion fields & properties

        #region methods
        private void OnDestroy()
        {
            OnStageCompleted = null;
        }
        public bool IsActivated() => gameObject.activeSelf;
        public void DisableStage()
        {
            gameObject.SetActive(false);
        }
        public void ActivateStage()
        {
            gameObject.SetActive(true);
        }
        protected virtual void CompleteStage()
        {
            OnStageCompleted?.Invoke();
            if (disableStageOnComplete)
                DisableStage();
        }
        #endregion methods
    }
}