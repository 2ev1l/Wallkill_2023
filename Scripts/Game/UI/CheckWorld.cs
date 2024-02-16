using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class CheckWorld : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnWorldCorrectEvent;
        public UnityEvent OnWorldIncorrectEvent;

        [SerializeField] private WorldType worldToCheck = WorldType.Portal;
        [SerializeField] private bool checkOnEnable = true;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (checkOnEnable)
                Check();
        }
        private void Check()
        {
            if (IsCorrect())
                OnWorldCorrectEvent?.Invoke();
            else
                OnWorldIncorrectEvent?.Invoke();
        }
        public bool IsCorrect() => GameData.Data.WorldsData.CurrentWorld == worldToCheck;
        #endregion methods
    }
}