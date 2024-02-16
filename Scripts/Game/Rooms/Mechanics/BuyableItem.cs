using Data.Stored;
using EditorCustom.Attributes;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Rooms.Mechanics
{
    public class BuyableItem : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnCanBuyItemEvent;
        public UnityEvent OnCanNotBuyItemEvent;

        public UnityEvent OnItemBoughtEvent;
        private Wallet PlayerWallet => GameData.Data.PlayerData.Wallet;
        public int Price
        {
            get => price;
            set
            {
                price = value;
                CheckPrice();
            }
        }
        [SerializeField] private int price;
        [SerializeField] private bool checkOnEnable = true;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (checkOnEnable) CheckPrice();
            PlayerWallet.OnValueChanged += OnPlayerWalletChanged;
        }
        private void OnDisable()
        {
            PlayerWallet.OnValueChanged -= OnPlayerWalletChanged;
        }

        private void OnPlayerWalletChanged(int amount) => CheckPrice();
        public void CheckPrice()
        {
            if (PlayerWallet.CanSpent(price))
                OnCanBuyItemEvent?.Invoke();
            else
                OnCanNotBuyItemEvent?.Invoke();
        }
        [SerializedMethod]
        public void TryBoughtItem()
        {
            if (!PlayerWallet.TrySpent(Price)) return;
            OnItemBoughtEvent?.Invoke();
        }
        #endregion methods
    }
}