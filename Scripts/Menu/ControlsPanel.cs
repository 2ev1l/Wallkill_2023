using Data.Settings;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Menu
{
    public class ControlsPanel : SingleSceneInstance<ControlsPanel>
    {
        #region fields & properties
        public KeyCatcher KeyCatcher => keyCatcher;
        [SerializeField] private KeyCatcher keyCatcher;
        [SerializeField] private List<KeyCodeItemData> items;
        private KeyCodeSettings Context => SettingsData.Data.KeyCodeSettings;
        public IReadOnlyList<KeyCodeInfo> CurrentKeys => currentKeys;
        private List<KeyCodeInfo> currentKeys;
        [SerializeField] private CustomCheckbox alwaysMoveWithCameraCheckbox;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            KeyCatcher.OnKeyCatched += UpdateKeys;
            alwaysMoveWithCameraCheckbox.OnStateChanged += UpdateAlwaysMoveBool;
            UpdateCheckboxes();
            UpdateKeys();
        }
        private void OnDisable()
        {
            KeyCatcher.OnKeyCatched -= UpdateKeys;
            alwaysMoveWithCameraCheckbox.OnStateChanged -= UpdateAlwaysMoveBool;
        }
        private void UpdateAlwaysMoveBool(bool value)
        {
            SettingsData.Data.KeyCodeSettings.AlwaysMoveWithCamera = value;
        }
        private void UpdateCheckboxes()
        {
            alwaysMoveWithCameraCheckbox.CurrentState = SettingsData.Data.KeyCodeSettings.AlwaysMoveWithCamera;
        }
        private void UpdateKeys(KeyCodeInfo info) => UpdateKeys();
        public void UpdateKeys()
        {
            currentKeys = Context.GetKeys();

            foreach (var el in items)
            {
                KeyCodeInfo info = currentKeys.Find(x => x.Description == el.Description);
                el.Item.Init(info);
            }
        }

        #endregion methods
    }
}