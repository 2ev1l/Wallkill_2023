using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.DataBase
{
    [System.Serializable]
    public class Item
    {
        #region fields & properties
        public int Id => id;
        [SerializeField][Min(0)] private int id;
        public int CountToUse => countToUse;
        [SerializeField][Min(0)] private int countToUse = 1;
        public Sprite Sprite512x => sprite512x;
        [SerializeField] private Sprite sprite512x;
        public Material Material => material;
        [SerializeField] private Material material;
        /// <summary>
        /// Probably you need <see cref="LanguageInfo.Text"/>
        /// </summary>
        public LanguageInfo Name => name;
        [SerializeField] private LanguageInfo name = new(0, Universal.UI.TextType.Items);
        /// <summary>
        /// Do not modify this property.
        /// </summary>
        public TimeDelay ActivationDelay => activationDelay;
        [SerializeField] private TimeDelay activationDelay;
        public bool IsSkill => isSkill;
        [SerializeField] private bool isSkill = false;
        [SerializeField] private AudioClip useSound;
        #endregion fields & properties

        #region methods
        public bool TryGetUseSound(out AudioClip sound)
        {
            sound = useSound;
            return sound != null;
        }
        #endregion methods
    }
}