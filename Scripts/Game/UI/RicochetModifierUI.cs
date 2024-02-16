using EditorCustom.Attributes;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    public class RicochetModifierUI : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private bool useBulletDamageReceiver = true;
        [SerializeField][DrawIf(nameof(useBulletDamageReceiver), true)] private BulletDamageReceiver bulletDamageReceiver;
        [SerializeField][DrawIf(nameof(useBulletDamageReceiver), false)][Min(1)] private int count = 1;
        [SerializeField] private bool initializeOnAwake = true;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            if (initializeOnAwake)
                InitializeText();
        }
        public void InitializeText()
        {
            int bounces = useBulletDamageReceiver ? bulletDamageReceiver.RicochetsCountToReceive : count;
            text.text = $"{LanguageLoader.GetTextByType(TextType.Game, 33)} {bounces}";
        }
        #endregion methods
    }
}