using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI;
using Universal.UI.Audio;

namespace Game.Rooms.Mechanics
{
    public class TargetCounterOrderedUI : TargetCounterUI
    {
        #region fields & properties
        private TargetCounterOrdered TargetCounterOrdered => (TargetCounterOrdered)TargetCounter;
        [SerializeField] private bool playSoundOnWrongCount = true;
        [SerializeField][DrawIf(nameof(playSoundOnWrongCount), true)] private AudioClipData wrongSound;
        [SerializeField][DrawIf(nameof(playSoundOnWrongCount), true)] private TimeDelay soundDelay = new(0.5f);
        [SerializeField] private bool ignoreVisualUI = false;
        #endregion fields & properties

        #region methods
        protected override void OnEnable()
        {
            base.OnEnable();
            if (playSoundOnWrongCount)
                TargetCounterOrdered.OnWrongOrder += PlaySoundOnWrong;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            TargetCounterOrdered.OnWrongOrder -= PlaySoundOnWrong;
        }
        protected override void UpdateUI()
        {
            if (ignoreVisualUI) return;
            base.UpdateUI();
        }
        private void PlaySoundOnWrong()
        {
            if (!soundDelay.CanActivate) return;
            soundDelay.Activate();
            wrongSound.Play();
        }
        #endregion methods

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (TargetCounter == null) return;
            if (TargetCounter.GetType() != typeof(TargetCounterOrdered))
            {
                Debug.LogError("Type of target counter is not 'ordered'", this);
            }
        }
#endif //UNITY_EDITOR
    }
}