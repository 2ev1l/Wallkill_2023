using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Settings
{
    [System.Serializable]
    public class KeyCodeInfo
    {
        #region fields & properties
        public UnityAction<KeyCode> OnKeyCodeChanged;
        public KeyCode Key
        {
            get => key;
            set
            {
                key = value;
                OnKeyCodeChanged?.Invoke(value);
            }
        }
        [SerializeField] private KeyCode key;
        public KeyCodeDescription Description => description;
        [SerializeField][NonSerialized] private KeyCodeDescription description;
        #endregion fields & properties

        #region methods
        public KeyCodeInfo(KeyCode key, KeyCodeDescription description)
        {
            this.key = key;
            this.description = description;
        }
        #endregion methods
    }
}