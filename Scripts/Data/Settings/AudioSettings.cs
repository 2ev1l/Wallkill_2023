using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Settings
{
    [System.Serializable]
    public class AudioSettings
    {
        #region fields & properties
        public AudioData SoundData => soundData;
        public AudioData MusicData => musicData;
        public AudioData AudioData => audioData;

        [SerializeField] private AudioData soundData = new();
        [SerializeField] private AudioData musicData = new();
        [SerializeField] private AudioData audioData = new();

        #endregion fields & properties

        #region methods
        public AudioSettings() { }
        public AudioSettings(AudioData soundData, AudioData musicData, AudioData audioData)
        {
            this.soundData = soundData;
            this.musicData = musicData;
            this.audioData = audioData;
        }
        #endregion methods
    }
}