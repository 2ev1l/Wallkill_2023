using UnityEngine;

namespace Universal.UI.Audio
{
    [System.Serializable]
    public class AudioClipData
    {
        #region fields & properties
        public AudioClip Clip
        {
            get => clip;
            set => clip = value;
        }
        [SerializeField] private AudioClip clip;
        public Transform PositionToPlay
        {
            get => positionToPlay;
            set => positionToPlay = value;
        }
        [SerializeField] private Transform positionToPlay;
        [SerializeField] private AudioType audioType = AudioType.Sound;
        [SerializeField][Min(0)] private float soundScale = 1f;
        #endregion fields & properties

        #region methods
        public void Play()
        {
            AudioManager.NextSoundScale = soundScale;
            if (positionToPlay != null)
            {
                AudioManager.PlayClipAtPoint(clip, audioType, positionToPlay.position);
            }
            else
            {
                AudioManager.PlayClip(clip, audioType);
            }
        }
        public AudioClipData() { }
        public AudioClipData(AudioClip clip, Transform positionToPlay)
        {
            this.clip = clip;
            this.positionToPlay = positionToPlay;
        }
        #endregion methods
    }
}