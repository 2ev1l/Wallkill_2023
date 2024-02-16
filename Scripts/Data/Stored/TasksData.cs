using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Data.Stored
{
    [System.Serializable]
    public class TasksData
    {
        #region fields & properties
        public UnityAction<int> OnTaskCompleted;
        public UnityAction<int> OnTaskStarted;
        public IReadOnlyList<int> CompletedTasks => completedTasks.ItemsId;
        [SerializeField] private OpenedItems completedTasks = new();
        public IReadOnlyList<int> CurrentTasks => currentTasks.ItemsId;
        [SerializeField] private OpenedItems currentTasks = new();
        #endregion fields & properties

        #region methods
        private bool CanStartTask(int taskId)
        {
            if (completedTasks.IsOpened(taskId)) return false;
            if (!currentTasks.TryOpenItem(taskId)) return false;
            return true;
        }
        public bool TryStartTask(int taskId)
        {
            if (!CanStartTask(taskId)) return false;
            OnTaskStarted?.Invoke(taskId);
            return true;
        }
        private bool CanCompleteTask(int taskId)
        {
            if (!currentTasks.TryCloseItem(taskId)) return false;
            if (!completedTasks.TryOpenItem(taskId)) return false;
            return true;
        }
        public bool TryCompleteTask(int taskId)
        {
            if (!CanCompleteTask(taskId)) return false;
            Task task = DB.Instance.Tasks.GetObjectById(taskId).Data;
            foreach (var reward in task.Rewards)
            {
                reward.AddReward();
            }
            OnTaskCompleted?.Invoke(taskId);
            return true;
        }
        public bool IsTaskCompleted(int taskId) => CompletedTasks.Contains(taskId);
        public bool IsTaskStarted(int taskId) => CurrentTasks.Contains(taskId);
        public bool IsTaskCanBeStarted(int taskId)
        {
            if (IsTaskStarted(taskId)) return false;
            if (IsTaskCompleted(taskId)) return false;
            return true;
        }
        #endregion methods
    }
}