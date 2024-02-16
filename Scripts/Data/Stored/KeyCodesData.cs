using Data.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Stored
{
    [System.Serializable]
    public class KeyCodesData
    {
        #region fields & properties
        public UnityAction<KeyCodeDescription> OnKeyOpened;
        public UnityAction OnAnyKeyOpened;
        public IReadOnlyList<KeyCodeDescription> OpenedKeys => openedKeys;
        [SerializeField] private List<KeyCodeDescription> openedKeys = new();
        #endregion fields & properties

        #region methods
        public bool IsKeyOpened(KeyCodeDescription description) => openedKeys.Exists(x => x == description);
        public bool TryOpenKey(KeyCodeDescription description)
        {
            if (IsKeyOpened(description)) return false;
            openedKeys.Add(description);
            OnKeyOpened?.Invoke(description);
            OnAnyKeyOpened?.Invoke();
            return true;
        }

        private List<KeyCodeDescription> GetDefaultOpenedKeys() 
        {
            List<KeyCodeDescription> result = new()
            {
                KeyCodeDescription.OpenSettings,
                KeyCodeDescription.Modifier,
            };
            return result;
        }
        public void FixNewOpenedKeys()
        {
            List<KeyCodeDescription> newKeys = GetDefaultOpenedKeys();
            foreach (var el in newKeys)
            {
                if (TryOpenKey(el))
                    Debug.Log($"Updated key opened: {el}");
            }
        }
        public KeyCodesData()
        {
            openedKeys = GetDefaultOpenedKeys();
        }
        #endregion methods
    }
}