using Data.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Menu
{
    public class SettingsKeyCatcher : KeyCatcher
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void CatchThisKey(KeyCode thisKey)
        {
            base.CatchThisKey(thisKey);
            SettingsData.Data.KeyCodeSettings.OnAnyKeyCodeChanged?.Invoke();
        }
        #endregion methods
    }
}