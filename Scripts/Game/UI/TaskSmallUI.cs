using Data.Stored;
using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public class TaskSmallUI : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private MessageReceiver horizontalMessages;
        [SerializeField] private GameObject newTaskInfo;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            GameData.Data.TasksData.OnTaskStarted += OnNewTaskStarted;
            GameData.Data.TasksData.OnTaskCompleted += OnTaskCompleted;
        }
        private void OnDisable()
        {
            GameData.Data.TasksData.OnTaskStarted -= OnNewTaskStarted;
            GameData.Data.TasksData.OnTaskCompleted -= OnTaskCompleted;
        }
        private void OnTaskCompleted(int taskId)
        {
            Task task = DB.Instance.Tasks.GetObjectById(taskId).Data;
            horizontalMessages.ReceiveMessage($"{LanguageLoader.GetTextByType(TextType.Help, 38)}\n{task.Name.Text}");
        }
        private void OnNewTaskStarted(int taskId)
        {
            EnableNewTaskInfo();
            Task task = DB.Instance.Tasks.GetObjectById(taskId).Data;
            horizontalMessages.ReceiveMessage($"{LanguageLoader.GetTextByType(TextType.Help, 37)}\n{task.Name.Text}");
        }
        [SerializedMethod]
        public void DisableNewTaskInfo()
        {
            newTaskInfo.SetActive(false);
        }
        public void EnableNewTaskInfo()
        {
            newTaskInfo.SetActive(true);
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField] private int taskToCheck = 0;
        [Button(nameof(AddTask))]
        private void AddTask()
        {
            GameData.Data.TasksData.TryStartTask(taskToCheck);
        }
        [Button(nameof(CompleteTask))]
        private void CompleteTask()
        {
            GameData.Data.TasksData.TryCompleteTask(taskToCheck);
        }
#endif //UNITY_EDITOR
    }
}