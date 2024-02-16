using Data.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Universal.UI
{
    public class PostProcessingSettings : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Volume volume;
        [SerializeField] private bool autoUpdate;
        private static PostEffectSettings Context => SettingsData.Data.PostEffectSettings;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            //It will be easier to prevent user from modifying settings in play mode
            //than trying to implement override with already modified values
            if (autoUpdate)
                SettingsData.Data.OnPostEffectsChanged += SetDefaultValues;
            SetDefaultValues();
        }
        private void OnDisable()
        {
            SettingsData.Data.OnPostEffectsChanged -= SetDefaultValues;
        }
        private void SetDefaultValues(PostEffectSettings value) => SetDefaultValues();
        private void SetDefaultValues()
        {
            TryChangeBloom();
            TryChangeVignette();
            TryChangeMotionBlur();
            TryChangeSplitToning();
            TryChangeWhiteBalance();
        }
        private void TryChangeBloom()
        {
            if (!volume.profile.TryGet(out Bloom bloom)) return;
            bloom.intensity.value = Context.BloomData.Intensity;
            bloom.threshold.value = Context.BloomData.Threshold;
        }
        private void TryChangeVignette()
        {
            if (!volume.profile.TryGet(out Vignette vignette)) return;
            vignette.smoothness.value = Context.VignetteData.Smoothness;
            vignette.intensity.value = Context.VignetteData.Intensity;
        }
        private void TryChangeMotionBlur()
        {
            if (!volume.profile.TryGet(out MotionBlur motionBlur)) return;
            motionBlur.intensity.value = Context.MotionBlurData.Intensity;
        }
        private void TryChangeSplitToning() //gamma
        {
            if (!volume.profile.TryGet(out SplitToning splitToning)) return;
            splitToning.balance.value = Context.SplitToningData.Balance;
        }
        private void TryChangeWhiteBalance()
        {
            if (!volume.profile.TryGet(out WhiteBalance whiteBalance)) return;
            whiteBalance.temperature.value = Context.WhiteBalanceData.Temperature;
            whiteBalance.tint.value = Context.WhiteBalanceData.Tint;
        }
        #endregion methods
    }
}