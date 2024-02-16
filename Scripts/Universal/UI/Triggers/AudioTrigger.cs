using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI.Audio;

namespace Universal.UI.Triggers
{
    public class AudioTrigger : DefaultTrigger
    {
        #region fields & properties
        [SerializeField] private bool playOnce = false;
        [SerializeField] private List<AudioClipData> clipsOnEnter;
        [SerializeField] private List<AudioClipData> clipsOnExit;
        private int playedCountEnter = 0;
        private int playedCountExit = 0;
        #endregion fields & properties

        #region methods
        protected override void OnEnterTriggered()
        {
            if (playOnce && playedCountEnter > 0) return;
            playedCountEnter++;
            foreach (var el in clipsOnEnter)
                el.Play();
        }

        protected override void OnExitTriggered()
        {
            if (playOnce && playedCountExit > 0) return;
            playedCountExit++;
            foreach (var el in clipsOnExit)
                el.Play();
        }
        #endregion methods
    }
}