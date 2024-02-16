using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Game.DataBase
{
    [System.Serializable]
    public class Memory
    {
        #region fields & properties
        public int Id => id;
        [SerializeField][Min(0)] private int id;

        public int ReferenceId => referenceId;
        [Title("Reference")][SerializeField][Min(0)] private int referenceId;
        public MemoryType MemoryType => memoryType;
        [SerializeField] private MemoryType memoryType;

        public bool OverrideName => overrideName;
        [SerializeField] private bool overrideName = false;
        public bool OverrideDescription => overrideDescription;
        [SerializeField] private bool overrideDescription = false;
        public bool OverrideSprite => overrideSprite;
        [SerializeField] private bool overrideSprite = false;
        /// <summary>
        /// Probably you want <see cref="TryGetName(out string)"/>
        /// </summary>
        public LanguageInfo Name => name;
        [SerializeField][DrawIf(nameof(overrideName), true)] private LanguageInfo name;
        /// <summary>
        /// Probably you want <see cref="TryGetDescription(out string)"/>
        /// </summary>
        public LanguageInfo Description => description;
        [SerializeField][DrawIf(nameof(overrideDescription), true)] private LanguageInfo description;
        /// <summary>
        /// Probably you want <see cref="TryGetSprite(out Sprite)"/>
        /// </summary>
        public Sprite Sprite => sprite;
        [SerializeField][DrawIf(nameof(overrideSprite), true)] private Sprite sprite;
        #endregion fields & properties

        #region methods
        public bool TryGetName(out string result)
        {
            if (overrideName) result = name.Text;
            else memoryType.GetDataByType(referenceId, out result, out _, out _);
            return result != null;
        }
        public bool TryGetDescription(out string result)
        {
            if (overrideDescription) result = description.Text;
            else memoryType.GetDataByType(referenceId, out _, out result, out _);
            return result != null;
        }
        public bool TryGetSprite(out Sprite result)
        {
            if (overrideSprite) result = sprite;
            else memoryType.GetDataByType(referenceId, out _, out _, out result);
            return result != null;
        }

        #endregion methods
    }
}