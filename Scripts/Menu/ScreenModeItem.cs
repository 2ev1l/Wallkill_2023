using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class ScreenModeItem : TextItem<int>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public override void OnListUpdate(int param)
        {
            value = param;
            Text.text = (FullScreenMode)param switch
            {
                FullScreenMode.ExclusiveFullScreen => "Full Screen",
                FullScreenMode.FullScreenWindow => "Maxmized Window",
                FullScreenMode.MaximizedWindow => "Maxmized Window",
                FullScreenMode.Windowed => "Windowed",
                _ => throw new System.NotImplementedException("Full Screen Mode")
            };
        }
        #endregion methods
    }
}