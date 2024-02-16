using Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class BoolItemList : TextItemList<bool>
{
        #region fields & properties

        #endregion fields & properties

        #region methods
        /// <summary>
        /// Triggers the same base method with "true" and "false" list
        /// </summary>
        public void UpdateListData() => UpdateListData(new() { false, true });
        #endregion methods
    }
}