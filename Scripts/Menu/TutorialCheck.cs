using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Menu
{
    public class TutorialCheck : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnCompletedEvent;
        public UnityEvent OnNotCompletedEvent;
        [SerializeField] private bool checkOnEnable = true;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (!checkOnEnable) return;
            Check();
        }
        public void Check()
        {
            if (GameData.Data.TutorialData.IsCompleted)
                OnCompletedEvent?.Invoke();
            else
                OnNotCompletedEvent?.Invoke();
        }
        #endregion methods
    }
}