using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI.Audio;

namespace Game.Rooms
{
    public class RoomUI : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Room room;

        [Title("Audio")]
        [SerializeField] private bool playSoundOnStart = true;
        [SerializeField] private bool playSoundOnComplete = true;
        [SerializeField][DrawIf(nameof(playSoundOnStart), true)] private AudioClip startClip;
        [SerializeField][DrawIf(nameof(playSoundOnComplete), true)] private AudioClip completedClip;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            room.OnStart += OnRoomStart;
            room.OnCompleted += OnRoomCompleted;
        }
        private void OnDisable()
        {
            room.OnStart -= OnRoomStart;
            room.OnCompleted -= OnRoomCompleted;
        }
        private void OnRoomCompleted()
        {
            if (playSoundOnComplete)
            {
                PlayClip(completedClip);
            }
        }
        private void OnRoomStart()
        {
            if (playSoundOnStart)
            {
                PlayClip(startClip);
            }
        }
        private void PlayClip(AudioClip clip)
        {
            AudioManager.PlayClip(clip, Universal.UI.Audio.AudioType.Sound, out AudioSource playableSource);
            playableSource.spatialBlend = 0;
        }
        #endregion methods
    }
}