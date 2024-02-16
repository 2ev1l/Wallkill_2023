using Data.Stored;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI.Triggers;

namespace Game.UI
{
    public class PortalOverlayData : MonoBehaviour
    {
        #region fields & properties
        public static PortalOverlayData CurrentInstance => currentInstance;
        private static PortalOverlayData currentInstance;

        [SerializeField] private TriggerCatcher playerTriggerCatcher;
        public int DefaultPrice => defaultPrice;
        [SerializeField][Min(0)] private int defaultPrice = 0;
        public bool CanSaveWorld => canSaveWorld && worldToLoad == WorldType.Portal;
        [SerializeField][DrawIf(nameof(worldToLoad), WorldType.Portal)] private bool canSaveWorld = false;
        public int WorldSavePriceIncrease => worldSavePriceIncrease;
        [SerializeField][DrawIf(nameof(canSaveWorld), true)][DrawIf(nameof(worldToLoad), WorldType.Portal)][Min(0)] private int worldSavePriceIncrease = 0;
        public EventTrigger EventTrigger => eventTrigger;
        [SerializeField] private EventTrigger eventTrigger;
        public WorldType WorldToLoad => worldToLoad;
        [SerializeField] private WorldType worldToLoad = WorldType.Portal;
        public bool ForceSave => forceSave;
        [SerializeField] private bool forceSave = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            playerTriggerCatcher.OnEnterTagSimple += SetInstance;
        }
        private void OnDisable()
        {
            playerTriggerCatcher.OnEnterTagSimple -= SetInstance;
        }
        private void SetInstance()
        {
            currentInstance = this;
        }
        #endregion methods
    }
}