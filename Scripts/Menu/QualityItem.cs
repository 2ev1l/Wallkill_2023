using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class QualityItem : IntItem
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            value = param;
            Text.text = value switch
            {
                0 => "Very Low",
                1 => "Low",
                2 => "Medium",
                3 => "High",
                4 => "Optimized",
                5 => "Ultra",
                6 => "Custom",
                _ => "???"
            };
        }
        #endregion methods
    }
}