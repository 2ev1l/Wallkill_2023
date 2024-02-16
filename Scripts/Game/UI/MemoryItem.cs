using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public class MemoryItem : TextItemChoosable<MemoryItem>
    {
        #region fields & properties
        private Memory memory = null;
        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            memory = DB.Instance.Memories.GetObjectById(param).Data;
            base.OnListUpdate(param);
        }
        protected override string GetDescription()
        {
            memory.TryGetDescription(out string description);
            return description;
        }

        protected override string GetName()
        {
            if (!memory.TryGetName(out string name))
                name = $"{LanguageLoader.GetTextByType(TextType.Help, 40)} #{memory.Id}";
            return name;
        }

        protected override void OnItemChoosed()
        {
            MemoriesUI.Instance.ChooseMemory(memory);
        }
        #endregion methods
    }
}