using System.Collections.Generic;
using UnityEngine;

namespace Data.Settings
{
    [System.Serializable]
    public class LanguageData
    {
        #region fields & properties
        public string[] HelpData => helpData;
        [SerializeField][TextArea(0, 30)] private string[] helpData;
        public string[] MenuData => menuData;
        [SerializeField] [TextArea(0, 30)] private string[] menuData;
        public string[] GameData => gameData;
        [SerializeField][TextArea(0, 30)] private string[] gameData;
        public string[] ItemsData => itemsData;
        [SerializeField][TextArea(0, 30)] private string[] itemsData;
        public string[] TasksData => tasksData;
        [SerializeField][TextArea(0, 30)] private string[] tasksData;
        public string[] MemoriesData => memoriesData;
        [SerializeField][TextArea(0, 30)] private string[] memoriesData;
        #endregion fields & properties
    }
}