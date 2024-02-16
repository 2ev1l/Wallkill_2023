using Data.Settings;
using Data.Settings.PostEffects;
using System.Collections.Generic;
using UnityEngine;
using EditorCustom.Attributes;

namespace Menu
{
    public class PostEffectsPanel : SettingsPanel<PostEffectSettings>
    {
        #region fields & properties
        [SerializeField] private IntItemList bloomIntensityList; //percent-float [1f=100%] 80-120%
        [SerializeField] private IntItemList vignetteIntensityList; //percent-float [1f=100%] 0-25%
        [SerializeField] private IntItemList motionBlurIntensityList; //percent-float [1f=100%] 0-40%
        [SerializeField] private IntItemList whiteBalanceTemperatureList; //percent-float [1:1] -50-50
        [SerializeField] private IntItemList whiteBalanceTintList; //percent-float [1:1] -50-50
        protected override PostEffectSettings Context => SettingsData.Data.PostEffectSettings;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public override void SaveSettings()
        {
            SettingsData.Data.PostEffectSettings = GetSettings();
        }
        protected override PostEffectSettings GetSettings()
        {
            float bloomIntensity = GetFirstItemListValue(bloomIntensityList) / 100f;
            float vignetteIntensity = GetFirstItemListValue(vignetteIntensityList) / 100f;
            float motionBlurIntensity = GetFirstItemListValue(motionBlurIntensityList) / 100f;
            float wbTemperature = GetFirstItemListValue(whiteBalanceTemperatureList);
            float wbTint = GetFirstItemListValue(whiteBalanceTintList);
            BloomData bloom = new(Context.BloomData.Threshold, bloomIntensity);
            VignetteData vignette = new(vignetteIntensity, Context.VignetteData.Smoothness);
            MotionBlurData motionBlur = new(motionBlurIntensity);
            WhiteBalanceData whiteBalance = new(wbTemperature, wbTint);
            SplitToningData splitToning = new(Context.SplitToningData.Balance);
            PostEffectSettings s = new(bloom, vignette, motionBlur, splitToning, whiteBalance);
            return s;
        }

        protected override void UpdateAllLists()
        {
            UpdateBloomLists();
            UpdateVignetteLists();
            UpdateMotionBlurLists();
            UpdateWhiteBalanceLists();
        }
        private void UpdateBloomLists()
        {
            List<int> list = new() { 80, 90, 100, 110, 120 };
            bloomIntensityList.UpdateListData(list);
            bloomIntensityList.ItemsList.ShowAt(Mathf.RoundToInt(Context.BloomData.Intensity * 100));
        }
        private void UpdateVignetteLists()
        {
            List<int> list = new() { 0, 5, 10, 15, 20, 25 };
            vignetteIntensityList.UpdateListData(list);
            vignetteIntensityList.ItemsList.ShowAt(Mathf.RoundToInt(Context.VignetteData.Intensity * 100));
        }
        private void UpdateMotionBlurLists()
        {
            List<int> list = new() { 0, 10, 20, 30, 40 };
            motionBlurIntensityList.UpdateListData(list);
            motionBlurIntensityList.ItemsList.ShowAt(Mathf.RoundToInt(Context.MotionBlurData.Intensity * 100));
        }
        private void UpdateWhiteBalanceLists()
        {
            List<int> list = new() { -50, -40, -30, -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 40, 50 };
            
            whiteBalanceTemperatureList.UpdateListData(list);
            whiteBalanceTemperatureList.ItemsList.ShowAt(Mathf.RoundToInt(Context.WhiteBalanceData.Temperature));

            whiteBalanceTintList.UpdateListData(list);
            whiteBalanceTintList.ItemsList.ShowAt(Mathf.RoundToInt(Context.WhiteBalanceData.Tint));
        }
        private int GetFirstItemListValue(IntItemList list) => list.CurrentPageItems[0].Value;
        #endregion methods
    }
}