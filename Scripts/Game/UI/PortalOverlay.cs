using Data.Stored;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Labyrinth;
using Game.Rooms;
using Game.Rooms.Mechanics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Universal;
using Universal.UI;

namespace Game.UI
{
    public class PortalOverlay : MonoBehaviour
    {
        #region fields & properties
        public UnityAction<WorldType> OnPortalUsed;
        [SerializeField] private BuyableItem exitBuyer;
        [SerializeField] private SpriteRenderer worldSpriteRenderer;
        [SerializeField] private TextMeshProUGUI worldName;

        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private CustomCheckbox worldSaveCheckbox;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (PortalOverlayData.CurrentInstance == null)
            {
                Debug.Log("Portal Overlay Data Instance is null. UI doesn't update", this);
                return;
            }
            //not necessary to handle other events, like changed <portal overlay data> instance, or player wallet because UI blocks input.
            worldSaveCheckbox.OnStateChanged += OnCheckboxChanged;
            UpdateUI();
        }
        private void OnDisable()
        {
            worldSaveCheckbox.OnStateChanged -= OnCheckboxChanged;
        }
        private void OnCheckboxChanged(bool _) => UpdateUI();
        private void UpdateUI()
        {
            PortalOverlayData data = PortalOverlayData.CurrentInstance;

            WorldType world = data.WorldToLoad;
            WorldInfo worldInfo = DB.Instance.WorldsInfo.Find(x => x.Data.WorldType == world).Data;
            worldSpriteRenderer.sprite = worldInfo.Texture;
            worldName.text = worldInfo.Language.Text;

            int finalPrice = data.DefaultPrice;
            worldSaveCheckbox.gameObject.SetActive(data.CanSaveWorld);
            if (data.CanSaveWorld)
            {
                int checkboxScale = worldSaveCheckbox.CurrentState ? 1 : 0;
                finalPrice += data.WorldSavePriceIncrease * checkboxScale;
            }
            priceText.text = $"{finalPrice}";
            exitBuyer.Price = finalPrice;
        }
        [SerializedMethod]
        public void LoadWorldPortal()
        {
            InstancesProvider.Instance.OverlayStateMachine.ApplyDefaultState();
            PortalOverlayData.CurrentInstance.EventTrigger.SimulateExit();
            
            if (PortalOverlayData.CurrentInstance.CanSaveWorld && worldSaveCheckbox.CurrentState && worldSaveCheckbox.gameObject.activeSelf)
            {
                WorldLoader.Instance.SaveLastWorld = true;
            }
            WorldLoader.Instance.IsLoadingFromPortalObject = true;
            WorldLoader.Instance.LoadWorld(PortalOverlayData.CurrentInstance.WorldToLoad);
            if (PortalOverlayData.CurrentInstance.ForceSave)
            {
                SavingUtils.SaveGameData();
            }
            OnPortalUsed?.Invoke(PortalOverlayData.CurrentInstance.WorldToLoad);
        }
        #endregion methods
    }
}