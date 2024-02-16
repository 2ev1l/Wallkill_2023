using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Game.Rooms.Mechanics
{
    public class CheckTask : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnTaskCompletedEvent;
        public UnityEvent OnTaskStartedEvent;
        public UnityEvent OnTaskNotInitializedEvent;

        [SerializeField][Min(0)] private int taskId;
        [SerializeField] private bool checkOnStart = true;
        [SerializeField] private bool subscribeAtTasks = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (!subscribeAtTasks) return;
            GameData.Data.TasksData.OnTaskCompleted += CheckTaskAction;
            GameData.Data.TasksData.OnTaskStarted += CheckTaskAction;
            Check();
        }
        private void OnDisable()
        {
            GameData.Data.TasksData.OnTaskCompleted -= CheckTaskAction;
            GameData.Data.TasksData.OnTaskStarted -= CheckTaskAction;
        }
        private void CheckTaskAction(int taskId)
        {
            if (taskId != this.taskId) return;
            Check();
        }
        private void Start()
        {
            if (!checkOnStart) return;
            Check();
        }
        public void Check()
        {
            if (GameData.Data.TasksData.CurrentTasks.Exists(x => x == taskId, out _))
            {
                OnTaskStartedEvent?.Invoke();
                return;
            }
            if (GameData.Data.TasksData.CompletedTasks.Exists(x => x == taskId, out _))
            {
                OnTaskCompletedEvent?.Invoke();
                return;
            }
            OnTaskNotInitializedEvent?.Invoke();
        }
        #endregion methods
    }
}