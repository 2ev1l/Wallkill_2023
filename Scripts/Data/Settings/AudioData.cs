using UnityEngine;
using UnityEngine.Events;

namespace Data.Settings
{
    [System.Serializable]
    public class AudioData
    {
        #region fields & properties
        public UnityAction<float> OnVolumeChanged;

        /// <summary>
        /// 0..1f
        /// </summary>
        public float Volume
        {
            get => volume;
            set => SetVolume(value);
        }
        [SerializeField] private float volume = 0.5f;

        #endregion fields & properties

        #region methods
        private void SetVolume(float value)
        {
            value = Mathf.Clamp01(value);
            volume = value;
            OnVolumeChanged?.Invoke(value);
        }
        public AudioData() { }
        public AudioData(float volume)
        {
            this.volume = volume;
        }
        #endregion methods
    }
}