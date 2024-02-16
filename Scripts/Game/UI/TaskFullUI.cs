using Data.Stored;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game.UI
{
    public class TaskFullUI : SingleSceneInstance<TaskFullUI>
    {
        #region fields & properties
        [Title("Right Panel")]
        [SerializeField] private TextMeshProUGUI rightTaskIdText;
        [SerializeField] private TextMeshProUGUI rightTaskWorldText;
        [SerializeField] private TextMeshProUGUI rightTaskNameText;
        [SerializeField] private TextMeshProUGUI rightTaskDescriptionText;
        [SerializeField] private SpriteRenderer rightTaskSprite;

        private Task choosedTask;

        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (choosedTask == null) ResetRightView();
            else UpdateRightView();
        }
        private void OnDisable()
        {

        }

        public void ChooseTask(int id) => ChooseTask(DB.Instance.Tasks.GetObjectById(id).Data);
        public void ChooseTask(Task task)
        {
            choosedTask = task;
            UpdateRightView();
        }
        private void ResetRightView()
        {
            rightTaskIdText.text = "";
            rightTaskWorldText.text = "";
            rightTaskNameText.text = "";
            rightTaskDescriptionText.text = "";
            rightTaskSprite.gameObject.SetActive(false);
        }
        private void UpdateRightView()
        {
            bool isTaskCompleted = GameData.Data.TasksData.CompletedTasks.Contains(choosedTask.Id);
            rightTaskIdText.text = $"{(isTaskCompleted ? $"{LanguageLoader.GetTextByType(TextType.Help, 38)}" : $"{LanguageLoader.GetTextByType(TextType.Help, 37)}")}";
            rightTaskWorldText.text = $"{LanguageLoader.GetTextByType(TextType.Tasks, 2)}: {DB.Instance.WorldsInfo.Find(x => x.Data.WorldType == choosedTask.World).Data.Language.Text}";
            rightTaskNameText.text = $"{choosedTask.Name.Text}";
            rightTaskDescriptionText.text = $"{(choosedTask.UseDescription ? choosedTask.Description.Text : "")}";
            rightTaskDescriptionText.text += $"{(choosedTask.DoReward ? $"\n{Reward.GetRewardsText(choosedTask.Rewards)}" : "")}";
            rightTaskSprite.gameObject.SetActive(choosedTask.UsePreview);
            if (choosedTask.UsePreview)
                rightTaskSprite.sprite = choosedTask.PreviewSprite;
        }

        #endregion methods
#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField] private int taskToTest = 0;

        [Button(nameof(ChooseTest))]
        private void ChooseTest() => ChooseTask(taskToTest);
#endif //UNITY_EDITOR
    }
}