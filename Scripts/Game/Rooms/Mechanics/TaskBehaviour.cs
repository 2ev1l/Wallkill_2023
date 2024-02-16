using Data.Stored;
using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms.Mechanics
{
    public class TaskBehaviour : MonoBehaviour
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void TryStartTask(int id) => GameData.Data.TasksData.TryStartTask(id);
        [SerializedMethod]
        public void TryCompleteTask(int id) => GameData.Data.TasksData.TryCompleteTask(id);
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][Min(0)] private int taskToTest = 0;

        [Button(nameof(TestStartTask))]
        private void TestStartTask()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            TryStartTask(taskToTest);
        }
        [Button(nameof(TestCompleteTask))]
        private void TestCompleteTask()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            TryCompleteTask(taskToTest);
        }
#endif //UNITY_EDITOR
    }
}