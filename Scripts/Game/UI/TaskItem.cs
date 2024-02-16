using EditorCustom.Attributes;
using Game.DataBase;
using Menu;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class TaskItem : TextItemDescription
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
        #endregion methods
    }
}