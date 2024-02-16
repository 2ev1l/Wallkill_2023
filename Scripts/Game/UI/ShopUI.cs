using Data.Stored;
using Game.DataBase;
using Game.Shop;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game.UI
{
    public class ShopUI : SingleSceneInstance<ShopUI>
    {
        #region fields & properties
        [SerializeField] private ShopBehaviour shopBehaviour;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDescription;
        [SerializeField] private CustomButton buttonBuy;
        [SerializeField] private WalletText priceText;
        [SerializeField] private GameObject textureObject;
        [SerializeField] private SpriteRenderer textureRenderer;

        private ShopItemChoosable currentItemReference;
        private ShopItem currentItem;
        private int shopTimeUpdatedUI = int.MinValue;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            shopBehaviour.ShopData.OnItemsGenerated += UpdateUI;
            buttonBuy.OnClicked += BuyItem;
            UpdateUI();
        }
        private void OnDisable()
        {
            shopBehaviour.ShopData.OnItemsGenerated -= UpdateUI;
            buttonBuy.OnClicked -= BuyItem;
        }
        public void BuyItem()
        {
            if (currentItemReference == null) return;
            shopBehaviour.ShopData.BuyItem(currentItem.Id);
            currentItemReference = null;
            currentItem = null;
            UpdateUI();
        }
        private void ResetUI()
        {
            itemName.text = "";
            itemDescription.text = "";
            buttonBuy.gameObject.SetActive(false);
            priceText.gameObject.SetActive(false);
            textureObject.SetActive(false);
            shopTimeUpdatedUI = shopBehaviour.ShopData.LastTimeGenerated;
            
            if (currentItemReference != null)
            {
                currentItemReference.DeselectItem();
                currentItemReference = null;
                currentItem = null;
            }
        }
        private void UpdateUI()
        {
            if (currentItemReference == null || currentItem == null || currentItemReference.Item != currentItem || shopTimeUpdatedUI != shopBehaviour.ShopData.LastTimeGenerated)
            {
                ResetUI();
                return;
            }
            currentItem.GetInfo(out string name, out string description, out Sprite sprite);
            itemName.text = name;
            itemDescription.text = description;
            textureRenderer.sprite = sprite;
            buttonBuy.gameObject.SetActive(GameData.Data.PlayerData.Wallet.CanSpent(currentItem.Price.Value));
            priceText.Wallet = currentItem.Price;
            priceText.gameObject.SetActive(true);
            textureObject.SetActive(sprite != null);
            shopTimeUpdatedUI = shopBehaviour.ShopData.LastTimeGenerated;
        }
        public void ChooseItem(ShopItemChoosable shopItemChoosable)
        {
            currentItemReference = shopItemChoosable;
            currentItem = currentItemReference.Item;
            UpdateUI();
        }
        #endregion methods
    }
}