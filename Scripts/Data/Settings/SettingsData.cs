using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.UI;

namespace Data.Settings
{
    [System.Serializable]
    internal class SettingsData : ISaveable
    {
        #region fields & properties
        public static readonly string SaveName = "settings";
        public static readonly string SaveExtension = ".json";
        public static SettingsData Data
        {
            get => data;
            set => SetData(value);
        }
        private static SettingsData data;

        public UnityAction<GraphicsSettings> OnGraphicsChanged;
        public UnityAction<LanguageSettings> OnLanguageChanged;
        public UnityAction<PostEffectSettings> OnPostEffectsChanged;
        public UnityAction<QualitySettings> OnQualityChanged;

        [SerializeField] private LanguageSettings languageSettings = new();
        [SerializeField] private GraphicsSettings graphicsSettings = new();
        [SerializeField] private AudioSettings audioSettings = new();
        [SerializeField] private PostEffectSettings postEffectSettings = new();
        [SerializeField] private KeyCodeSettings keyCodeSettings = new();
        [SerializeField] private Data.QualitySettings qualitySettings = new();
        [SerializeField] private bool isFirstLaunch = true;

        public LanguageSettings LanguageSettings
        {
            get => languageSettings;
            set => SetLanguage(value);
        }
        public GraphicsSettings GraphicsSettings
        {
            get => graphicsSettings;
            set => SetGraphics(value);
        }
        public AudioSettings AudioSettings
        {
            get => audioSettings;
        }
        public PostEffectSettings PostEffectSettings
        {
            get => postEffectSettings;
            set => SetPostEffects(value);
        }
        public KeyCodeSettings KeyCodeSettings
        {
            get => keyCodeSettings;
        }
        public Data.QualitySettings QualitySettings
        {
            get => qualitySettings;
            set => SetQuality(value);
        }
        public bool IsFirstLaunch
        {
            get => isFirstLaunch;
            set => isFirstLaunch = value;
        }
        #endregion fields & properties

        #region methods
        private void SetQuality(Data.QualitySettings value)
        {
            qualitySettings = value;
            OnQualityChanged?.Invoke(value);
        }
        private void SetPostEffects(PostEffectSettings value)
        {
            postEffectSettings = value;
            OnPostEffectsChanged?.Invoke(value);
        }
        private void SetLanguage(LanguageSettings value)
        {
            languageSettings = value;
            OnLanguageChanged?.Invoke(value);
        }
        private void SetGraphics(GraphicsSettings value)
        {
            graphicsSettings = value;
            OnGraphicsChanged?.Invoke(value);

        }
        private static void SetData(SettingsData value) => data = value ?? new SettingsData();
        #endregion methods
    }
}