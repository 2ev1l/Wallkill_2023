using UnityEngine;
using EditorCustom.Attributes;

namespace Menu
{
    public abstract class SettingsPanel<T> : MonoBehaviour where T : class
    {
        #region fields & properties
        protected abstract T Context { get; }
        #endregion fields & properties

        #region methods
        protected virtual void Awake()
        {
            UpdateAllLists();
        }
        protected abstract T GetSettings();
        [SerializedMethod]
        public abstract void SaveSettings();
        protected abstract void UpdateAllLists();
        #endregion methods
    }
}