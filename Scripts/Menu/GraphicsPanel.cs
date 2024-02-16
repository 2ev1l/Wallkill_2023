using Data.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EditorCustom.Attributes;

namespace Menu
{
    public class GraphicsPanel : SettingsPanel<GraphicsSettings>
    {
        #region fields & properties
        [SerializeField] private ResolutionsItemList resolutionsList;
        [SerializeField] private IntItemList refreshRateList;
        [SerializeField] private IntItemList screenModeList;
        [SerializeField] private IntItemList cameraSensitivitiesList;
        [SerializeField] private BoolItemList enableLoadingScreenList;
        [SerializeField] private BoolItemList enableReflectionsList;

        private List<SimpleResolution> Resolutions
        {
            get
            {
                resolutions ??= GetResolutions();
                return resolutions;
            }
        }
        private List<SimpleResolution> resolutions = null;
        private List<int> RefreshRates
        {
            get
            {
                refreshRates ??= GetRefreshRates();
                return refreshRates;
            }
        }
        private List<int> refreshRates = null;
        private List<int> ScreenModes
        {
            get
            {
                screenModes ??= new() { (int)FullScreenMode.ExclusiveFullScreen, (int)FullScreenMode.FullScreenWindow, (int)FullScreenMode.Windowed };
                return screenModes;
            }
        }
        private List<int> screenModes = null;
        private List<int> Sensitivities
        {
            get
            {
                sensitivities ??= new() { 1, 2, 3, 4, 6, 8, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 140, 170, 200, 250, 300, 400, 500, 600, 700, 800, 900, 1000 };
                return sensitivities;
            }
        }
        private List<int> sensitivities = null;
        protected override GraphicsSettings Context => SettingsData.Data.GraphicsSettings;
        #endregion fields & properties

        #region methods
        private static List<int> GetRefreshRates()
        {
            List<int> rr = new();
            foreach (Resolution el in Screen.resolutions)
            {
                int value = (int)el.refreshRateRatio.value;
                if (rr.FindIndex(x => x == value) > -1) continue;
                rr.Add(value);
            }
            rr.Add(999);
            rr = rr.OrderBy(x => x).ToList();
            return rr;
        }
        private static List<SimpleResolution> GetResolutions()
        {
            List<SimpleResolution> resolutions = new();
            foreach (Resolution el in Screen.resolutions)
            {
                if (resolutions.FindIndex(x => x.width == el.width && x.height == el.height) > -1) continue;
                SimpleResolution res = new();
                res.width = el.width;
                res.height = el.height;
                resolutions.Add(res);
            }
            return resolutions;
        }
        
        [SerializedMethod]
        public override void SaveSettings()
        {
            SettingsData.Data.GraphicsSettings = GetSettings();
        }
        protected override GraphicsSettings GetSettings()
        {
            SimpleResolution res = resolutionsList.CurrentPageItems[0].Value;
            int rr = refreshRateList.CurrentPageItems[0].Value;
            FullScreenMode sm = (FullScreenMode)screenModeList.CurrentPageItems[0].Value;
            int cs = cameraSensitivitiesList.CurrentPageItems[0].Value;
            bool els = enableLoadingScreenList.CurrentPageItems[0].Value;
            bool er = enableReflectionsList.CurrentPageItems[0].Value;

            return new()
            {
                Resolution = res,
                RefreshRate = rr,
                ScreenMode = sm,
                CameraSensitvity = cs,
                EnableLoadingScreen = els,
                EnableReflections = er,
            };
        }

        protected override void UpdateAllLists()
        {
            UpdateResolution();
            UpdateRefreshRate();
            UpdateScreenMode();
            UpdateCameraSensitivity();
            UpdateLoadingScreen();
            UpdateReflections();
        }
        private void UpdateResolution()
        {
            resolutionsList.UpdateListData(Resolutions);
            resolutionsList.ItemsList.ShowAt(Context.Resolution);
        }
        private void UpdateRefreshRate()
        {
            refreshRateList.UpdateListData(RefreshRates);
            refreshRateList.ItemsList.ShowAt(Context.RefreshRate);
        }
        private void UpdateScreenMode()
        {
            screenModeList.UpdateListData(ScreenModes);
            screenModeList.ItemsList.ShowAt((int)Context.ScreenMode);
        }
        private void UpdateCameraSensitivity()
        {
            cameraSensitivitiesList.UpdateListData(Sensitivities);
            cameraSensitivitiesList.ItemsList.ShowAt(Context.CameraSensitvity);
        }
        private void UpdateLoadingScreen()
        {
            enableLoadingScreenList.UpdateListData();
            enableLoadingScreenList.ItemsList.ShowAt(Context.EnableLoadingScreen);
        }
        private void UpdateReflections()
        {
            enableReflectionsList.UpdateListData();
            enableReflectionsList.ItemsList.ShowAt(Context.EnableReflections);
        }
        #endregion methods
    }
}