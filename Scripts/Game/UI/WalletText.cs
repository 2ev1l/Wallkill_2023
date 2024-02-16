using Data.Stored;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal;
using Universal.UI;
using Universal.UI.Layers;

namespace Game.UI
{
    public class WalletText : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private bool usePlayerWallet = true;
        public Wallet Wallet
        {
            get => usePlayerWallet ? GameData.Data.PlayerData.Wallet : wallet;
            set
            {
                UnsubscribeAtWallet();
                wallet = value;
                SubscribeAtWallet();
            }
        }
        [SerializeField][DrawIf(nameof(usePlayerWallet), false)] private Wallet wallet;

        [SerializeField] private bool useAnimationOnChanged = true;
        [SerializeField][DrawIf(nameof(useAnimationOnChanged), true)] private TextMeshProUGUI spawnText;
        [SerializeField][DrawIf(nameof(useAnimationOnChanged), true)] private Transform parentForSpawn;
        [SerializeField][DrawIf(nameof(useAnimationOnChanged), true)] private Transform positionForSpawn;
        [SerializeField][DrawIf(nameof(useAnimationOnChanged), true)] private Transform endPosition;
        [SerializeField][DrawIf(nameof(useAnimationOnChanged), true)][Min(0)] private float timeToDestroy = 5f;
        private bool isSubscribedAtWallet = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            SubscribeAtWallet();
        }
        private void OnDisable()
        {
            UnsubscribeAtWallet();
        }
        private void SubscribeAtWallet()
        {
            if (isSubscribedAtWallet) return;
            
            Wallet.OnValueChanged += UpdateUI;
            if (useAnimationOnChanged)
                Wallet.OnValueChangedAmount += DoAnimationAmount;

            isSubscribedAtWallet = true;
            UpdateUI(Wallet.Value);
        }
        private void UnsubscribeAtWallet()
        {
            if (!isSubscribedAtWallet) return;
            
            Wallet.OnValueChanged -= UpdateUI;
            Wallet.OnValueChangedAmount -= DoAnimationAmount;
            
            isSubscribedAtWallet = false;
        }
        private void UpdateUI(int value)
        {
            text.text = Wallet.Value.ToString();
        }
        private void DoAnimationAmount(int amount)
        {
            TextMeshProUGUI text = Instantiate(spawnText, parentForSpawn);
            text.transform.SetLocalPositionAndRotation(positionForSpawn.localPosition, positionForSpawn.localRotation);
            string special = amount > 0 ? "+" : "";
            text.text = $"{special}{amount}";
            StartCoroutine(WaitForEndOfLayer(text));
        }
        private IEnumerator WaitForEndOfLayer(TextMeshProUGUI text)
        {
            VectorLayer vectorLayer = new();
            SmoothLayer smoothLayer = new();
            smoothLayer.ChangeLayerWeight(text.alpha, 0, timeToDestroy * 0.9f, (x) => { Color col = text.color; col.a = x; text.color = col; }, null, delegate { return this == null || text == null;}, false);
            yield return vectorLayer.ChangeVectorSmooth(text.transform.localPosition, endPosition.localPosition, timeToDestroy, x => text.transform.localPosition = x, null, delegate { return this == null || text == null; }, false);
            Destroy(text.gameObject);
        }
        #endregion methods
    }
}