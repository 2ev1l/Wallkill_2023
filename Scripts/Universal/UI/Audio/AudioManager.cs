using Data;
using Data.Interfaces;
using Data.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using EditorCustom.Attributes;
using Data.Stored;
using UnityEngine.Events;

namespace Universal.UI.Audio
{
    public class AudioManager : MonoBehaviour, IInitializable, IStartInitializable
    {
        #region fields & properties
        public UnityAction OnAnyVolumeChanged;
        public static AudioManager Instance { get; private set; }
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private ObjectPool<DestroyablePoolableObject> oneShotClips;
        private static float musicScale = 1f;
        public static float NextSoundScale { get; set; } = 1f;

        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private AudioClip gameTutorialMusic;
        [SerializeField] private AudioClip anySceneMusic;

        [Title("Read Only")]
        [SerializeField] [ReadOnly] private AudioListener activeAudioListener = null;
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
        }
        private void OnEnable()
        {
            Data.Settings.AudioSettings audioSettings = SettingsData.Data.AudioSettings;
            audioSettings.MusicData.OnVolumeChanged += UpdateMusicVolumes;
            audioSettings.AudioData.OnVolumeChanged += UpdateMusicVolumes;
            audioSettings.SoundData.OnVolumeChanged += UpdateSoundVolume;
        }
        private void OnDisable()
        {
            Data.Settings.AudioSettings audioSettings = SettingsData.Data.AudioSettings;
            audioSettings.MusicData.OnVolumeChanged -= UpdateMusicVolumes;
            audioSettings.AudioData.OnVolumeChanged -= UpdateMusicVolumes;
            audioSettings.SoundData.OnVolumeChanged -= UpdateSoundVolume;
        }
        public void Start()
        {
            FindActiveAudioListener();
            PlayMusicDependOnScene();
        }
        private void FindActiveAudioListener()
        {
            activeAudioListener = FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(x => x.enabled).FirstOrDefault();
        }
        private void PlayMusicDependOnScene()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName)
            {
                case "Game": PlayMusic(GameData.Data.TutorialData.IsCompleted ? gameMusic : gameTutorialMusic, false); break;
                default: PlayMusic(anySceneMusic, false); break;
            }
        }
        /// <summary>
        /// Plays clip at audio listener
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="type"></param>
        public static void PlayClip(AudioClip clip, AudioType type)
        {
            PlayClipAtPoint(clip, type, Instance.activeAudioListener == null ? Camera.main.transform.position : Instance.activeAudioListener.transform.position);
        }
        /// <summary>
        /// Plays clip at audio listener
        /// </summary>
        public static void PlayClip(AudioClip clip, AudioType type, out AudioSource playableSource)
        {
            PlayClipAtPoint(clip, type, GetActiveAudioListener().position, out playableSource);
        }
        public static Transform GetActiveAudioListener()=> Instance.activeAudioListener == null ? Camera.main.transform : Instance.activeAudioListener.transform;
        public static void PlayClipAtPoint(AudioClip clip, AudioType type, Vector3 position) => PlayClipAtPoint(clip, type, position, out _);
        public static void PlayClipAtPoint(AudioClip clip, AudioType type, Vector3 position, out AudioSource playableSource)
        {
            float volume = 1f * GetValueByType(type) * GetValueByType(AudioType.Audio) * NextSoundScale;
            NextSoundScale = 1f;
            PlayClipAtPoint(clip, position, volume, out playableSource);
        }
        private static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, out AudioSource audioSource)
        {
            PoolableAudioSource poolableAudioSource = (PoolableAudioSource)Instance.oneShotClips.GetObject();
            poolableAudioSource.Init(clip, position, volume);
            audioSource = poolableAudioSource.AudioSource;
        }
        public static void AddAmbient(AudioClip clip, bool force = false, float defaultMusicScale = 0.8f)
        {
            if (force || Instance.ambienceSource.clip != clip)
            {
                Instance.ambienceSource.clip = clip;
            }
            musicScale = defaultMusicScale;
            Instance.UpdateCurrentMusicVolume();
            Instance.UpdateAmbientVolume();
            Instance.ambienceSource.Play();
        }
        public static void RemoveAmbient()
        {
            musicScale = 1f;
            Instance.ambienceSource.clip = null;
            Instance.ambienceSource.Stop();
            Instance.UpdateCurrentMusicVolume();
        }
        public static void PlayMusic(AudioClip clip, [Optional] bool force)
        {
            if (force || Instance.musicSource.clip != clip)
            {
                Instance.musicSource.clip = clip;
                Instance.UpdateCurrentMusicVolume();
                Instance.musicSource.Play();
            }
        }
        private void UpdateMusicVolumes(float _)
        {
            UpdateCurrentMusicVolume();
            UpdateAmbientVolume();
            OnAnyVolumeChanged?.Invoke();
        }
        private void UpdateSoundVolume(float soundVolume)
        {
            OnAnyVolumeChanged?.Invoke();
        }
        public void UpdateCurrentMusicVolume()
        {
            UpdateVolume(musicSource, AudioType.Music);
        }
        public void UpdateAmbientVolume()
        {
            ambienceSource.volume = 0.5f * GetValueByType(AudioType.Music) * GetValueByType(AudioType.Audio);
        }
        public static float GetScaledVolume(AudioType type) => GetValueByType(type) * GetValueByType(AudioType.Audio);
        private static void UpdateVolume(AudioSource audioSource, AudioType type) => audioSource.volume = musicScale * GetScaledVolume(type);
        public static float GetValueByType(AudioType type) => (type) switch
        {
            AudioType.Music => SettingsData.Data.AudioSettings.MusicData.Volume,
            AudioType.Sound => SettingsData.Data.AudioSettings.SoundData.Volume,
            AudioType.Audio => SettingsData.Data.AudioSettings.AudioData.Volume,
            _ => throw new System.NotImplementedException()
        };
        #endregion methods
    }
}