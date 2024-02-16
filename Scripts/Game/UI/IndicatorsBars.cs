using Data.Interfaces;
using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using TMPro;
using UnityEngine;
using Universal;
using Universal.UI;
using Game.Player;

namespace Game.UI
{
    public class IndicatorsBars : MonoBehaviour, IInitializable
    {
        #region fields & properties
        [SerializeField] private StatBar healthBar;
        [SerializeField] private StatBar staminaBar;
        [SerializeField] private TextMeshProUGUI healthPerSecondText;
        [SerializeField] private TextMeshProUGUI staminaPerSecondText;
        [SerializeField] private List<SpriteRenderer> fullDetails;
        [SerializeField] private TimeAverage healthAverageValue;
        [SerializeField] private TimeAverage staminaAverageValue;
        private List<float> fullDetailsStartAlpha = new();
        
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            Init();
            foreach (var el in fullDetails)
            {
                fullDetailsStartAlpha.Add(el.color.a);
            }
        }
        public void Init()
        {
            PlayerStats Context = StatsBehaviour.Stats;
            healthBar.Stat = Context.Health;
            staminaBar.Stat = Context.Stamina;

            healthAverageValue.Init();
            staminaAverageValue.Init();
        }
        private void OnEnable()
        {
            healthBar.ProgressBar.OnValueChanged += SetDetails;
            healthBar.Stat.OnValueChanged += SetHealthPerSecond;
            staminaBar.Stat.OnValueChanged += SetStaminaPerSecond;

            ResetValuePerSecond(healthPerSecondText);
            ResetValuePerSecond(staminaPerSecondText);
        }
        private void OnDisable()
        {
            healthBar.ProgressBar.OnValueChanged -= SetDetails;
            healthBar.Stat.OnValueChanged -= SetHealthPerSecond;
            staminaBar.Stat.OnValueChanged -= SetStaminaPerSecond;
        }
        private void SetValuePerSecond(StatBar bar, TimeAverage average, TextMeshProUGUI text)
        {
            if (bar.Stat.IsReachedMaximum())
            {
                ResetValuePerSecond(text);
                return;
            }
            float statValue = bar.Stat.Value;
            float speed = average.GetAverageSpeed(statValue);
            string speedSign = speed > 0 ? "+" : "";
            text.text = $"{speedSign}{speed:F1}/s";
            average.Value = statValue;
        }
        private void ResetValuePerSecond(TextMeshProUGUI text) => text.text = $"{0f:F1}/s";
        private void SetHealthPerSecond(int value)
        {
            SetValuePerSecond(healthBar, healthAverageValue, healthPerSecondText);
        }
        private void SetStaminaPerSecond(int value)
        {
            SetValuePerSecond(staminaBar, staminaAverageValue, staminaPerSecondText);
        }
        private void SetDetails(float health)
        {
            float percentFullFilled = CustomMath.InverseLerpVector(healthBar.ProgressBar.MinMaxValues, health);
            int totalCount = fullDetails.Count;
            for (int i = 0; i < totalCount; ++i)
            {
                var el = fullDetails[i];
                Color col = el.color;
                float alphaScale = percentFullFilled;
                col.a = alphaScale * fullDetailsStartAlpha[i];
                el.color = col;
            }
        }
        #endregion methods
    }
}