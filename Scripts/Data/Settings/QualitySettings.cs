using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class QualitySettings
    {
        #region fields & properties
        public int MSAA => msaa;
        [SerializeField] private int msaa;
        public float RenderScale => renderScale;
        [SerializeField] private float renderScale;
        public int LightsLimit => lightsLimit;
        [SerializeField] private int lightsLimit;
        public float ShadowDisance => shadowDisance;
        [SerializeField] private float shadowDisance;
        public int ShadowCascade => shadowCascade;
        [SerializeField] private int shadowCascade;
        public bool IsCustomAsset
        {
            get => isCustomAsset;
            set => isCustomAsset = value;
        }
        [SerializeField] private bool isCustomAsset;
        #endregion fields & properties

        #region methods
        public QualitySettings() { }

        public QualitySettings(int msaa, float renderScale, int lightsLimit, float shadowDisance, int shadowCascade, bool isCustomAsset)
        {
            this.msaa = msaa;
            this.renderScale = renderScale;
            this.lightsLimit = lightsLimit;
            this.shadowDisance = shadowDisance;
            this.shadowCascade = shadowCascade;
            this.isCustomAsset = isCustomAsset;
        }
        #endregion methods
    }
}