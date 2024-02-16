using Data.Settings.PostEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Settings
{
    [System.Serializable]
    public class PostEffectSettings
    {
        #region fields & properties
        public BloomData BloomData
        {
            get => bloomData;
        }
        [SerializeField] private BloomData bloomData = new();
        public VignetteData VignetteData
        {
            get => vignetteData;
        }
        [SerializeField] private VignetteData vignetteData = new();
        public MotionBlurData MotionBlurData
        {
            get => motionBlurData;
        }
        [SerializeField] private MotionBlurData motionBlurData = new();
        public SplitToningData SplitToningData
        {
            get => splitToningData;
        }
        [SerializeField] private SplitToningData splitToningData = new();
        public WhiteBalanceData WhiteBalanceData
        {
            get => whiteBalanceData;
        }
        [SerializeField] private WhiteBalanceData whiteBalanceData = new();
        #endregion fields & properties

        #region methods
        public PostEffectSettings() { }

        public PostEffectSettings(BloomData bloomData, VignetteData vignetteData, MotionBlurData motionBlurData, SplitToningData splitToningData, WhiteBalanceData whiteBalanceData)
        {
            this.bloomData = bloomData;
            this.vignetteData = vignetteData;
            this.motionBlurData = motionBlurData;
            this.splitToningData = splitToningData;
            this.whiteBalanceData = whiteBalanceData;
        }
        #endregion methods
    }
}