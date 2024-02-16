using Data.Stored;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game.UI
{
    public class WorldChooseState : StateChange
    {
        #region fields & properties
        public WorldType WorldType => worldType;
        [SerializeField] private WorldType worldType;
        public Sprite MainSprite => imagePreviewBase.sprite;
        [SerializeField] private SpriteRenderer imagePreviewBase;
        [SerializeField] private SpriteRenderer imagePreviewOutline;
        [SerializeField] private CustomButton customButton;
        [SerializeField] private List<MaterialRaycastChanger> materialRaycastChangers;
        [SerializeField] private Material materialWhenChoosed;
        [SerializeField] private Material materialDefault;
        public WorldInfo WorldInfo
        {
            get
            {
                worldInfo ??= DB.Instance.WorldsInfo.Find(x => x.Data.WorldType == WorldType).Data;
                return worldInfo;
            }
        }
        private WorldInfo worldInfo = null;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            imagePreviewBase.sprite = WorldInfo.Texture;
        }
        public override void SetActive(bool active)
        {
            customButton.enabled = !active;
            materialRaycastChangers.ForEach(x => x.enabled = !active);
            imagePreviewBase.material = active ? materialWhenChoosed : materialDefault;
            imagePreviewOutline.material = active ? materialWhenChoosed : materialDefault;
        }
        public override void SetInteraction(bool isInteractive)
        {
            gameObject.SetActive(isInteractive);
        }
        #endregion methods
    }
}