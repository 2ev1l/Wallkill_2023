using Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class IntPercentItem : TextItem<int>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            value = param;
            Text.text = $"{param}%";
        }
        #endregion methods
    }
}