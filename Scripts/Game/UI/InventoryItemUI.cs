using Data.Settings;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.UI;
using Universal.UI.Layers;

namespace Game.UI
{
    public class InventoryItemUI : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private InventoryItem inventoryItem;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer bgSpriteRenderer;
        [SerializeField] private TextMeshProUGUI textCount;
        [SerializeField] private TextMeshProUGUI textName;
        [SerializeField] private TextMeshProUGUI textKey;
        [SerializeField] private ProgressBar activationProgressBar;

        [SerializeField] private Color defaultColor;
        [SerializeField] private Color blockColor;
        [SerializeField] private ColorLayer blockColorLayer;
        private bool isBlockedToUse;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private KeyCodeInfo keyCodeInfo;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            keyCodeInfo = SettingsData.Data.KeyCodeSettings.GetKeys().Find(x => x.Description == inventoryItem.ActivateCodeDescription);
            SetKeyCodeText();
        }
        private void OnEnable()
        {
            inventoryItem.OnBeforeItemSet += TryUnsubscribe;
            inventoryItem.OnItemSet += TryUpdateUI;
            inventoryItem.OnItemSet += TrySubscribe;
            inventoryItem.OnItemBlockedToUse += SetBlockedColor;
            TryUpdateUI();
            TrySubscribe();
            SetDefaultColor();
        }
        private void OnDisable()
        {
            inventoryItem.OnBeforeItemSet -= TryUnsubscribe;
            inventoryItem.OnItemSet -= TryUpdateUI;
            inventoryItem.OnItemSet -= TrySubscribe;
            inventoryItem.OnItemBlockedToUse -= SetBlockedColor;
            TryUnsubscribe();
        }
        private void SetBlockedColor()
        {
            isBlockedToUse = true;
            blockColorLayer.ChangeColor(bgSpriteRenderer.color, blockColor, 0.3f, x => bgSpriteRenderer.color = x, delegate { isBlockedToUse = false; SetDefaultColor(); }, delegate { return bgSpriteRenderer == null; });
        }
        private void SetDefaultColor()
        {
            blockColorLayer.ChangeColor(bgSpriteRenderer.color, defaultColor, 0.3f, x => bgSpriteRenderer.color = x, null, delegate { return isBlockedToUse || bgSpriteRenderer == null; });
        }
        private void SetKeyCodeText()
        {
            textKey.text = LanguageLoader.GetTextByKeyCode(keyCodeInfo.Key);
        }
        private void TryUnsubscribe()
        {
            if (inventoryItem.PageItem == null) return;
            inventoryItem.PageItem.OnCountChanged -= OnPageItemCountChanged;
            inventoryItem.PageItem.ActivationDelay.OnTimeLasts -= UpdateDelay;
            inventoryItem.PageItem.ActivationDelay.OnDelayReady -= UpdateDelayOnReady;
        }
        private void TrySubscribe()
        {
            if (inventoryItem.PageItem == null) return;
            inventoryItem.PageItem.OnCountChanged += OnPageItemCountChanged;
            inventoryItem.PageItem.ActivationDelay.OnTimeLasts += UpdateDelay;
            inventoryItem.PageItem.ActivationDelay.OnDelayReady += UpdateDelayOnReady;

            activationProgressBar.MinMaxValues = new(0, inventoryItem.PageItem.ActivationDelay.Delay);
            UpdateDelay(inventoryItem.PageItem.ActivationDelay.CanActivate ? activationProgressBar.MinMaxValues.x : activationProgressBar.MinMaxValues.y);
        }
        private void UpdateDelayOnReady() => UpdateDelay(0);
        private void UpdateDelay(float value)
        {
            activationProgressBar.Value = value;
        }
        private void OnPageItemCountChanged(int count) => TryUpdateUI();
        private void TryUpdateUI()
        {
            if (inventoryItem.PageItem == null) return;
            textCount.text = $"x{inventoryItem.PageItem.Count}/{inventoryItem.ItemInfo.CountToUse}";
            textName.text = inventoryItem.ItemInfo.Name.Text;
            spriteRenderer.sprite = inventoryItem.ItemInfo.Sprite512x;
            spriteRenderer.material = inventoryItem.ItemInfo.Material;
        }
        #endregion methods
    }
}