using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public class TaskChoosableItemList : ItemListBase<TaskItemChoosable, int>
    {
        #region fields & properties
        [SerializeField] private bool useCurrentTasks = true;
        [SerializeField] private Transform referenceRotation;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            GameData.Data.TasksData.OnTaskStarted += UpdateListData;
            GameData.Data.TasksData.OnTaskCompleted += UpdateListData;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            GameData.Data.TasksData.OnTaskStarted -= UpdateListData;
            GameData.Data.TasksData.OnTaskCompleted -= UpdateListData;
            base.OnDisable();
        }
        private void UpdateListData(int _) => UpdateListData();
        public override void UpdateListData()
        {
            ItemList.UpdateListDefault(useCurrentTasks ? GameData.Data.TasksData.CurrentTasks : GameData.Data.TasksData.CompletedTasks, x => x);
            foreach (var el in ItemList.Items)
            {
                el.transform.localRotation = referenceRotation.localRotation;
            }
        }
        #endregion methods
    }
}