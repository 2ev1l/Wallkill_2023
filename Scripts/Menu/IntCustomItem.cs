using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class IntCustomItem : IntItem
    {
        #region fields & properties
        [SerializeField] private string textAfterValue = "";
        [SerializeField] private string textBeforeValue = "";
        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            value = param;
            Text.text = $"{textBeforeValue}{param}{textAfterValue}";
        }
        #endregion methods
    }
}