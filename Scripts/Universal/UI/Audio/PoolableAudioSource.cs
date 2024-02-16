using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Universal.UI.Audio
{
    public class PoolableAudioSource : DestroyablePoolableObject
    {
        #region fields & properties
        public AudioSource AudioSource => audioSource;
        [SerializeField] private AudioSource audioSource;
        #endregion fields & properties

        #region methods
        public void Init(AudioClip clip, Vector3 position, float volume)
        {
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            audioSource.Play();
            transform.position = position;
            LiveTime = clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale);
        }
        #endregion methods
    }
}