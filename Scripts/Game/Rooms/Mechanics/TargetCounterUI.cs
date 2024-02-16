using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.UI;

namespace Game.Rooms.Mechanics
{
    public class TargetCounterUI : MonoBehaviour
    {
        #region fields & properties
        protected TargetCounter TargetCounter => targetCounter;
        [HelpBox("You need to manualy invoke EnableUI & DisableUI methods", HelpBoxMessageType.Info)]
        [SerializeField] private TargetCounter targetCounter;
        private static TargetCounter CurrentController;
        #endregion fields & properties

        #region methods
        protected virtual void OnEnable()
        {
            targetCounter.OnCountChanged += UpdateUI;
        }
        protected virtual void OnDisable()
        {
            targetCounter.OnCountChanged -= UpdateUI;
        }
        protected virtual void UpdateUI()
        {
            InstancesProvider.Instance.TargetsCounterUI.UpdateUI(targetCounter.CurrentCount);
        }
        [SerializedMethod]
        public void EnableUI()
        {
            if (targetCounter.IsCountReached) return;
            CurrentController = targetCounter;
            InstancesProvider.Instance.TargetsCounterUI.EnableUI(targetCounter.GetNeededCount(), targetCounter.CurrentCount);
        }
        [SerializedMethod]
        public void DisableUI()
        {
            if (CurrentController != targetCounter) return;
            InstancesProvider.Instance.TargetsCounterUI.DisableUI();
        }
        #endregion methods
    }
}