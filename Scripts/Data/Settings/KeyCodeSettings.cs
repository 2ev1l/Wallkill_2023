using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Settings
{
    [System.Serializable]
    public class KeyCodeSettings
    {
        #region fields & properties
        public UnityAction OnAnyKeyCodeChanged;

        public bool AlwaysMoveWithCamera
        {
            get => alwaysMoveWithCamera;
            set => alwaysMoveWithCamera = value;
        }
        [SerializeField] private bool alwaysMoveWithCamera = false;

        public CharacterKeyCodeSettings CharacterSettings => characterSettings;
        [SerializeField] private CharacterKeyCodeSettings characterSettings = new();

        //Default
        public KeyCodeInfo OpenSettings => openSettings;
        [SerializeField] private KeyCodeInfo openSettings = new(KeyCode.Escape, KeyCodeDescription.OpenSettings);
        public KeyCodeInfo CameraRotation => cameraRotation;
        [SerializeField] private KeyCodeInfo cameraRotation = new(KeyCode.Mouse2, KeyCodeDescription.CameraRotation);
        public KeyCodeInfo CameraCrop => cameraCrop;
        [SerializeField] private KeyCodeInfo cameraCrop = new(KeyCode.C, KeyCodeDescription.CameraCrop);

        //Inventory
        public KeyCodeInfo Inventory1 => inventory1;
        [SerializeField] private KeyCodeInfo inventory1 = new(KeyCode.Alpha1, KeyCodeDescription.Inventory1);
        public KeyCodeInfo Inventory2 => inventory2;
        [SerializeField] private KeyCodeInfo inventory2 = new(KeyCode.Alpha2, KeyCodeDescription.Inventory2);
        public KeyCodeInfo Inventory3 => inventory3;
        [SerializeField] private KeyCodeInfo inventory3 = new(KeyCode.Alpha3, KeyCodeDescription.Inventory3);
        public KeyCodeInfo Inventory4 => inventory4;
        [SerializeField] private KeyCodeInfo inventory4 = new(KeyCode.Alpha4, KeyCodeDescription.Inventory4);
        public KeyCodeInfo Inventory5 => inventory5;
        [SerializeField] private KeyCodeInfo inventory5 = new(KeyCode.Alpha5, KeyCodeDescription.Inventory5);
        public KeyCodeInfo Inventory6 => inventory6;
        [SerializeField] private KeyCodeInfo inventory6 = new(KeyCode.Alpha6, KeyCodeDescription.Inventory6);
        public KeyCodeInfo Inventory7 => inventory7;
        [SerializeField] private KeyCodeInfo inventory7 = new(KeyCode.Alpha7, KeyCodeDescription.Inventory7);
        public KeyCodeInfo Inventory8 => inventory8;
        [SerializeField] private KeyCodeInfo inventory8 = new(KeyCode.Alpha8, KeyCodeDescription.Inventory8);
        public KeyCodeInfo Inventory9 => inventory9;
        [SerializeField] private KeyCodeInfo inventory9 = new(KeyCode.Alpha9, KeyCodeDescription.Inventory9);

        //Interfaces
        public KeyCodeInfo Tasks => tasks;
        [SerializeField] private KeyCodeInfo tasks = new(KeyCode.T, KeyCodeDescription.Tasks);
        public KeyCodeInfo Memory => memory;
        [SerializeField] private KeyCodeInfo memory = new(KeyCode.H, KeyCodeDescription.Memory);
        #endregion fields & properties

        #region methods
        public List<KeyCodeInfo> GetInventoryKeys()
        {
            return new()
            {
                Inventory1,
                Inventory2,
                Inventory3,
                Inventory4,
                Inventory5,
                Inventory6,
                Inventory7,
                Inventory8,
                Inventory9,
            };
        }
        /// <summary>
        /// Provides original classes.
        /// </summary>
        /// <returns></returns>
        public List<KeyCodeInfo> GetKeys()
        {
            List<KeyCodeInfo> list = new()
            {
                OpenSettings,
                CameraRotation,
                CameraCrop,
                Tasks,
                Memory
            };
            list.AddRange(GetInventoryKeys());
            list.AddRange(characterSettings.GetKeys());
            return list;
        }
        #endregion methods
    }
}