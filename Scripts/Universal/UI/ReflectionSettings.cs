using Data.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI
{
    public class ReflectionSettings : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private ReflectionProbe reflectionProbe;
        [SerializeField] private LightProbeGroup lightProbeGroup;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            SettingsData.Data.OnGraphicsChanged += ChangeReflections;
            ChangeReflections(SettingsData.Data.GraphicsSettings);
        }
        private void OnDisable()
        {
            SettingsData.Data.OnGraphicsChanged -= ChangeReflections;
        }
        private void ChangeReflections(GraphicsSettings gs)
        {
            bool enableReflections = gs.EnableReflections;
            reflectionProbe.enabled = enableReflections;
            lightProbeGroup.enabled = enableReflections;
        }
        #endregion methods
    }
}