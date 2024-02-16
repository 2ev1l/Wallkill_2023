using EditorCustom.Attributes;
using Game.DataBase;
using Menu;
using TMPro;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public class TaskItemChoosable : TextItemChoosable<TaskItemChoosable>
    {
        #region fields & properties
        private Task task = null;
        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            task = DB.Instance.Tasks.GetObjectById(param).Data;
            base.OnListUpdate(param);
        }
        protected override string GetName() => task.Name.Text;
        protected override string GetDescription() => task.UseDescription ? task.Description.Text : "";
        protected override void OnItemChoosed()
        {
            TaskFullUI.Instance.ChooseTask(task);
        }
        #endregion methods
    }
}