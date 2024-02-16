using Data.Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EditorCustom.Attributes;

namespace Universal.UI
{
    public class KeyCatcher : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnCatchStart;
        public UnityAction OnKeyReturns;
        public UnityAction<KeyCodeInfo> OnKeyCatched;
        public static List<KeyCode> AllowedKeys
        {
            get
            {
                allowedKeys ??= GetAllowedKeys();
                return allowedKeys;
            }
        }
        private static List<KeyCode> allowedKeys = null;
        public bool CanCatch => canCatch;
        [Header("Read Only")]
        [SerializeField][ReadOnly] private bool canCatch;
        [SerializeField][ReadOnly] private KeyCodeInfo lastKeyInfo;
        #endregion fields & properties

        #region methods
        private void OnDisable()
        {
            DisableCatch();
        }

        public void CatchKey(KeyCodeInfo keyInfo)
        {
            lastKeyInfo = keyInfo;
            canCatch = true;
            OnCatchStart?.Invoke();
        }
        [SerializedMethod]
        public void DisableCatch()
        {
            canCatch = false;
            OnKeyReturns?.Invoke();
        }
        private static List<KeyCode> GetAllowedKeys()
        {
            List<KeyCode> list = new();
            for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z; ++i)
                list.Add((KeyCode)i);

            for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; ++i)
                list.Add((KeyCode)i);

            list.Add(KeyCode.LeftShift);
            list.Add(KeyCode.RightShift);
            list.Add(KeyCode.LeftAlt);
            list.Add(KeyCode.RightAlt);
            list.Add(KeyCode.LeftControl);
            list.Add(KeyCode.RightControl);
            list.Add(KeyCode.Tab);
            list.Add(KeyCode.Space);
            list.Add(KeyCode.Backspace);
            list.Add(KeyCode.DownArrow);
            list.Add(KeyCode.LeftArrow);
            list.Add(KeyCode.RightArrow);
            list.Add(KeyCode.UpArrow);
            list.Add(KeyCode.Mouse0);
            list.Add(KeyCode.Mouse1);
            list.Add(KeyCode.Mouse2);
            list.Add(KeyCode.Mouse3);
            list.Add(KeyCode.Mouse4);
            return list;
        }

        private void Update()
        {
            if (!canCatch) return;
            int keysCount = AllowedKeys.Count;
            for (int i = 0; i < keysCount; ++i)
                if (Input.GetKeyDown(AllowedKeys[i]))
                    CatchThisKey(AllowedKeys[i]);

            if (Input.GetKeyDown(KeyCode.Escape))
                DisableCatch();
        }
        protected virtual void CatchThisKey(KeyCode @thisKey)
        {
            lastKeyInfo.Key = @thisKey;
            OnKeyCatched?.Invoke(lastKeyInfo);
            DisableCatch();
        }
        #endregion methods
    }
}