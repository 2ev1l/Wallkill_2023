using Data.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    [System.Serializable]
    public class KeyCodeItemData
    {
        #region fields & properties
        public KeyCodeItem Item => item;
        [SerializeField] private KeyCodeItem item;
        public KeyCodeDescription Description => description;
        [SerializeField] private KeyCodeDescription description;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}