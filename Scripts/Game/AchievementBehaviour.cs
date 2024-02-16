using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AchievementBehaviour : MonoBehaviour
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void SetAchievement(string name)
        {
            AchievementsObserver.SetAchievement(name);
        }
        #endregion methods
    }
}