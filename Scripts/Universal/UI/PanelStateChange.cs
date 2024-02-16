using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI
{
    public class PanelStateChange : StateChange
    {
        #region fields & properties
        [SerializeField] private GameObject panel;
        #endregion fields & properties

        #region methods
        public override void SetActive(bool active)
        {
            panel.SetActive(active);
        }
        #endregion methods
    }
}