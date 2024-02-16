using Data.Stored;
using EditorCustom.Attributes;
using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game.UI
{
    public class MemoriesUI : SingleSceneInstance<MemoriesUI>
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Memory choosedMemory;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (choosedMemory == null) ResetUI();
            else UpdateUI();
        }
        private void OnDisable()
        {

        }
        public void ChooseMemory(int id) => ChooseMemory(DB.Instance.Memories.GetObjectById(id).Data);
        public void ChooseMemory(Memory memory)
        {
            choosedMemory = memory;
            UpdateUI();
        }
        private void ResetUI()
        {
            ResetNameText();
            ResetDescriptionText();
            ResetSprite();
        }
        private void UpdateUI()
        {
            if (choosedMemory.TryGetName(out string name)) nameText.text = name;
            else nameText.text = $"{LanguageLoader.GetTextByType(TextType.Help, 40)} #{choosedMemory.Id}";

            if (choosedMemory.TryGetDescription(out string description)) descriptionText.text = description;
            else ResetDescriptionText();

            if (choosedMemory.TryGetSprite(out Sprite sprite))
            {
                spriteRenderer.gameObject.SetActive(true);
                spriteRenderer.sprite = sprite;
            }
            else ResetSprite();
        }
        private void ResetNameText() => nameText.text = "";
        private void ResetDescriptionText() => descriptionText.text = "";
        private void ResetSprite() => spriteRenderer.gameObject.SetActive(false);
        #endregion methods
#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField] private int memoryToTest = 0;

        [Button(nameof(ChooseTest))]
        private void ChooseTest() => ChooseMemory(memoryToTest);
        [Button(nameof(AddTest))]
        private void AddTest() => GameData.Data.StatisticData.OpenedMemories.TryOpenItem(memoryToTest);
#endif //UNITY_EDITOR
    }
}