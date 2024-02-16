using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Settings
{
    [System.Serializable]
    public class GraphicsSettings
    {
        #region fields & properties
        public SimpleResolution Resolution
        {
            get
            {
                if (resolution.width == 0 || resolution.height == 0)
                {
                    resolution.width = Screen.currentResolution.width;
                    resolution.height = Screen.currentResolution.height;
                }

                return resolution;
            }
            set => SetResolution(value);
        }
        [SerializeField] private SimpleResolution resolution;
        public FullScreenMode ScreenMode
        {
            get => screenMode;
            set => SetScreenMode(value);
        }
        [SerializeField] private FullScreenMode screenMode = FullScreenMode.FullScreenWindow;
        public bool Vsync
        {
            get => vsync;
            set => SetVsync(value);
        }
        [SerializeField] private bool vsync = false;
        /// <summary>
        /// 10..inf
        /// </summary>
        public int RefreshRate
        {
            get => refreshRate;
            set => SetRefreshRate(value);
        }
        [SerializeField] private int refreshRate = 60;
        /// <summary>
        /// 1..inf%
        /// </summary>
        public int CameraSensitvity
        {
            get => cameraSensitivity;
            set => SetSensitivity(value);
        }
        [SerializeField] private int cameraSensitivity = 100;

        public bool EnableLoadingScreen
        {
            get => enableLoadingScreen;
            set => enableLoadingScreen = value;
        }
        [SerializeField] private bool enableLoadingScreen = true;
        public bool EnableReflections
        {
            get => enableReflections;
            set => enableReflections = value;
        }
        [SerializeField] private bool enableReflections = true;
        #endregion fields & properties

        #region methods
        private void SetSensitivity(int value)
        {
            value = Mathf.Max(value, 1);
            cameraSensitivity = value;
        }
        private void SetResolution(SimpleResolution value)
        {
            resolution = value;
        }
        private void SetScreenMode(FullScreenMode value)
        {
            screenMode = value;
        }
        private void SetVsync(bool value)
        {
            vsync = value;
        }
        private void SetRefreshRate(int value)
        {
            value = Mathf.Max(value, 10);
            refreshRate = value;
        }

        public GraphicsSettings() { }
        public GraphicsSettings(SimpleResolution resolution, FullScreenMode screenMode, bool vsync, int refreshRate, int cameraSensitivity, bool enableLoadingScreen)
        {
            this.resolution = resolution;
            this.screenMode = screenMode;
            this.vsync = vsync;
            this.refreshRate = refreshRate;
            this.cameraSensitivity = cameraSensitivity;
            this.enableLoadingScreen = enableLoadingScreen;
        }
        #endregion methods
    }
}