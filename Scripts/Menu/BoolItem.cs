using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class BoolItem : TextItem<bool>
    {
        #region fields & properties
        [SerializeField] private string trueValue = "+";
        [SerializeField] private string falseValue = "-";
        #endregion fields & properties

        #region methods
        public override void OnListUpdate(bool param)
        {
            value = param;
            Text.text = param ? trueValue : falseValue;
        }
        #endregion methods
    }
}