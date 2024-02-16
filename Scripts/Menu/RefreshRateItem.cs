using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class RefreshRateItem : TextItem<int>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            value = param;
            Text.text = $"{param} FPS";
        }
        #endregion methods
    }
}