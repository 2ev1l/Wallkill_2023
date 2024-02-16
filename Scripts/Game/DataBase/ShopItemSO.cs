using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [CreateAssetMenu(fileName = "ShopItemSO", menuName = "ScriptableObjects/ShopItemSO")]
    public class ShopItemSO : DBScriptableObject<ShopItem>
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
            Data.GetInfo(out finalName, out finalDescription, out finalSprite);
        }
#endif //UNITY_EDITOR
    }
}