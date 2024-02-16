using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [CreateAssetMenu(fileName = "MemorySO", menuName = "ScriptableObjects/MemorySO")]
    public class MemorySO : DBScriptableObject<Memory>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods

        #endregion methods
#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField][ReadOnly] private string finalName = "";
        [SerializeField][ReadOnly][TextArea(0, 30)] private string finalDescription = "";
        [SerializeField][ReadOnly] private Sprite finalSprite = null;
        [HelpBox("Start Playing for test", HelpBoxMessageType.Info)]
        [SerializeField] private bool checkToUpdate;
        protected override void OnValidate()
        {
            base.OnValidate();
            if (!Application.isPlaying) return;
            Data.TryGetName(out finalName);
            Data.TryGetDescription(out finalDescription);
            Data.TryGetSprite(out finalSprite);
        }
#endif //UNITY_EDITOR
    }
}