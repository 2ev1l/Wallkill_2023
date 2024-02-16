using Data.Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using EditorCustom.Attributes;

namespace Menu
{
    public class QualityPanel : SettingsPanel<Data.QualitySettings>
    {
        #region fields & properties
        [SerializeField] private GameObject qualityChangeRaycastBlock;
        [SerializeField] private IntItemList qualityLevelItemList;

        [SerializeField] private IntItemList msaaItemList; //1-2-4-8x
        [SerializeField] private IntItemList renderScaleItemList; //10%~200% to Float 1:1

        [SerializeField] private IntItemList lightsLimitItemList; //0~8

        [SerializeField] private IntItemList shadowDistanceItemList; //0~200m
        [SerializeField] private IntItemList shadowCascadeItemList; //0~4

        [SerializeField] private List<UniversalRenderPipelineAsset> URPAssets;

        protected override Data.QualitySettings Context => SettingsData.Data.QualitySettings;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            qualityLevelItemList.ItemsList.OnPageSwitched += UpdateListsBasedOnURPRenderer;
        }
        private void OnDisable()
        {
            qualityLevelItemList.ItemsList.OnPageSwitched -= UpdateListsBasedOnURPRenderer;
        }
        protected override Data.QualitySettings GetSettings()
        {
            int msaa = msaaItemList.CurrentPageItems[0].Value;
            float renderScale = renderScaleItemList.CurrentPageItems[0].Value / 100f;
            int lightsLimit = lightsLimitItemList.CurrentPageItems[0].Value;
            float shadowDisance = shadowDistanceItemList.CurrentPageItems[0].Value;
            int shadowCascade = shadowCascadeItemList.CurrentPageItems[0].Value;
            Data.QualitySettings qs = new(msaa, renderScale, lightsLimit, shadowDisance, shadowCascade, IsCustomAsset());
            return qs;
        }
        [SerializedMethod]
        public override void SaveSettings()
        {
            int quality = qualityLevelItemList.CurrentPageItems[0].Value;
            QualitySettings.SetQualityLevel(quality, true);
            if (IsCustomAsset())
                SettingsData.Data.QualitySettings = GetSettings();
            else
                Context.IsCustomAsset = false;
            UpdateListsBasedOnURPRenderer();
        }
        protected override void UpdateAllLists()
        {
            UpdateQualityLevelList();
            UpdateListsBasedOnURPRenderer();
        }
        private void UpdateListsBasedOnURPRenderer()
        {
            ChangeQualityState();
            UpdateQualityLists();
            UpdateLightingLists();
            UpdateShadowsLists();
        }
        private void UpdateQualityLists()
        {
            UniversalRenderPipelineAsset urp = GetURPAsset();
            int msaa = IsCustomAsset() ? Context.MSAA : urp.msaaSampleCount; //Quality - Anti Aliasing;
            List<int> msaaValues = new() { 1, 2, 4, 8 };
            msaaItemList.UpdateListData(msaaValues);
            msaaItemList.ItemsList.ShowAt(msaa);

            int rs = Mathf.RoundToInt((IsCustomAsset() ? Context.RenderScale : urp.renderScale) * 100); //Quality - Render Scale;
            List<int> rsValues = new() { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200 };
            renderScaleItemList.UpdateListData(rsValues);
            renderScaleItemList.ItemsList.ShowAt(rs);
        }
        private void UpdateLightingLists()
        {
            UniversalRenderPipelineAsset urp = GetURPAsset();
            int lightsLimit = IsCustomAsset() ? Context.LightsLimit : urp.maxAdditionalLightsCount; //Lighting - Additional Lights - Per Object Limit
            List<int> lightsLimitValues = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            lightsLimitItemList.UpdateListData(lightsLimitValues);
            lightsLimitItemList.ItemsList.ShowAt(lightsLimit);
        }
        private void UpdateShadowsLists()
        {
            UniversalRenderPipelineAsset urp = GetURPAsset();
            int sDistance = Mathf.RoundToInt(IsCustomAsset() ? Context.ShadowDisance : urp.shadowDistance); //Shadows - Max Disance;
            List<int> sDistanceValues = new() { 0, 10, 25, 30, 40, 50, 60, 75, 80, 90, 100, 110, 125, 130, 140, 150 };
            shadowDistanceItemList.UpdateListData(sDistanceValues);
            shadowDistanceItemList.ItemsList.ShowAt(sDistance);

            int sCascade = IsCustomAsset() ? Context.ShadowCascade : urp.shadowCascadeCount; //Shadows - Cascade Count
            List<int> sCascadeValues = new() { 1, 2, 3, 4 };
            shadowCascadeItemList.UpdateListData(sCascadeValues);
            shadowCascadeItemList.ItemsList.ShowAt(sCascade);
        }

        private UniversalRenderPipelineAsset GetURPAsset() => URPAssets[qualityLevelItemList.CurrentPageItems[0].Value];
        private bool IsCustomAsset() => qualityLevelItemList.CurrentPageItems[0].Value == URPAssets.Count - 1;
        private void ChangeQualityState()
        {
            qualityChangeRaycastBlock.SetActive(!IsCustomAsset());
        }
        private void UpdateQualityLevelList()
        {
            int urpAssetsCount = URPAssets.Count;
            List<int> list = new();
            for (int i = 0; i < urpAssetsCount; ++i)
            {
                list.Add(i);
            }
            qualityLevelItemList.UpdateListData(list);
            qualityLevelItemList.ItemsList.ShowAt(QualitySettings.GetQualityLevel());
        }
        #endregion methods
    }
}