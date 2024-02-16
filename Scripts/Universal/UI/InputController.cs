using Data.Interfaces;
using Data.Settings;
using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Universal
{
    public class InputController : MonoBehaviour, IInitializable
    {
        #region fields & properties
        public UnityAction OnKeyUpdates;
        public static InputController Instance { get; private set; }

        public static UnityAction<KeyCode> OnKeyDown;
        public static UnityAction<KeyCode> OnKeyHold;
        public static IReadOnlyList<KeyCode> CheckCodes
        {
            get
            {
                checkCodes ??= GetKeyCodes();
                return checkCodes;
            }
        }
        private static IReadOnlyList<KeyCode> checkCodes = null;
        public static IEnumerable<KeyCodeInfo> CurrentKeys
        {
            get
            {
                currentKeys ??= GetKeys();
                return currentKeys;
            }
        }
        private static IEnumerable<KeyCodeInfo> currentKeys;

        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
            GameData.Data.KeyCodesData.FixNewOpenedKeys();
        }
        private void OnEnable()
        {
            KeyCodeSettings keyCodeSettings = SettingsData.Data.KeyCodeSettings;
            keyCodeSettings.OnAnyKeyCodeChanged += SetKeys;
            keyCodeSettings.OnAnyKeyCodeChanged += SetKeyCodes;

            KeyCodesData keyCodesData = GameData.Data.KeyCodesData;
            keyCodesData.OnAnyKeyOpened += SetKeys;
            keyCodesData.OnAnyKeyOpened += SetKeyCodes;
            SetKeys();
            SetKeyCodes();
        }
        private void OnDisable()
        {
            KeyCodeSettings keyCodeSettings = SettingsData.Data.KeyCodeSettings;
            keyCodeSettings.OnAnyKeyCodeChanged -= SetKeys;
            keyCodeSettings.OnAnyKeyCodeChanged -= SetKeyCodes;

            KeyCodesData keyCodesData = GameData.Data.KeyCodesData;
            keyCodesData.OnAnyKeyOpened -= SetKeys;
            keyCodesData.OnAnyKeyOpened -= SetKeyCodes;
        }
        private static void SetKeyCodes()
        {
            checkCodes = GetKeyCodes();
            Instance.OnKeyUpdates?.Invoke();
        }
        private static List<KeyCode> GetKeyCodes()
        {
            return CurrentKeys.Select(x => x.Key).ToList();
        }

        private static void SetKeys() => currentKeys = GetKeys();
        private static IEnumerable<KeyCodeInfo> GetKeys()
        {
            IEnumerable<KeyCodeInfo> allKeys = SettingsData.Data.KeyCodeSettings.GetKeys();
            KeyCodesData keyCodesData = GameData.Data.KeyCodesData;
            IEnumerable<KeyCodeInfo> sortedKeys = allKeys.Where(x => keyCodesData.IsKeyOpened(x.Description));
            return sortedKeys;
        }

        public static KeyCode FindKeyByDescription(KeyCodeDescription description)
        {
            foreach (var el in CurrentKeys)
            {
                if (el.Description == description)
                    return el.Key;
            }
            return KeyCode.None;
        }

        private void Update()
        {
            int count = CheckCodes.Count;
            for (int i = 0; i < count; ++i)
            {
                KeyCode currentKey = CheckCodes[i];
                if (Input.GetKeyDown(currentKey))
                    OnKeyDown?.Invoke(currentKey);
                if (Input.GetKey(currentKey))
                    OnKeyHold?.Invoke(currentKey);
            }
        }
        #endregion methods
    }
}