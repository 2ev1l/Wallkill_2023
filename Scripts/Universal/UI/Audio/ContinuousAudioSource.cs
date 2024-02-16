using Data.Stored;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI.Audio
{
    public class ContinuousAudioSource : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioType audioType = AudioType.Sound;

        [Title("Settings")]
        [SerializeField][Range(0, 2)] float finalVolumeScale = 1f;
        [SerializeField] private bool playOnEnable = true;
        [SerializeField][DrawIf(nameof(playOnEnable), true)] private bool playFromLastSecond = true;
        [SerializeField][DrawIf(nameof(playOnEnable), true)][DrawIf(nameof(playFromLastSecond), true)][Range(0, 100)] private float savingPrecision = 50;

        [Title("Read Only")]
        [SerializeField][ReadOnly][DrawIf(nameof(playOnEnable), true)][DrawIf(nameof(playFromLastSecond), true)] private float lastTimeSaved = 0f;
        [SerializeField][ReadOnly][DrawIf(nameof(playOnEnable), true)][DrawIf(nameof(playFromLastSecond), true)] private float savingTimeDelay;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            AudioManager.Instance.OnAnyVolumeChanged += UpdateVolume;
            UpdateVolume();
            if (playOnEnable)
            {
                if (playFromLastSecond)
                {
                    PlayFromLastSecond();

                    CalculateSavingTimeDelay();
                    CancelInvoke(nameof(SaveCurrentDelay));
                    InvokeRepeating(nameof(SaveCurrentDelay), 0, savingTimeDelay);
                }
                else
                    PlayFromStart();
            }

        }
        private void OnDisable()
        {
            AudioManager.Instance.OnAnyVolumeChanged -= UpdateVolume;
            CancelInvoke(nameof(SaveCurrentDelay));
        }
        private void CalculateSavingTimeDelay()
        {
            savingTimeDelay = CustomMath.ConvertValueFromTo(new(0, 100), savingPrecision, new(audioSource.clip.length, Mathf.Min(audioSource.clip.length, 0.5f)));
        }
        public void SaveCurrentDelay()
        {
            lastTimeSaved = audioSource.time;
        }
        private void UpdateVolume()
        {
            audioSource.volume = AudioManager.GetScaledVolume(audioType) * finalVolumeScale;
        }
        public void PlayFromStart() => audioSource.Play();
        public void PlayFromLastSecond()
        {
            if (lastTimeSaved > audioSource.clip.length)
                lastTimeSaved = 0;
            PlayFromStart();
            audioSource.time = lastTimeSaved;
        }
        #endregion methods
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (audioSource == null || audioSource.clip == null) return;
            CalculateSavingTimeDelay();
        }
#endif //UNITY_EDITOR
    }
}