using TMPro;
using UnityEngine;
using UnityEngine.Events;
using EditorCustom.Attributes;
using Universal.UI.Layers;

namespace Universal.UI
{
    public class ProgressBar : MonoBehaviour
    {
        #region fields & properties
        public UnityAction<float> OnValueChanged;
        [Title("UI")]
        [Required][SerializeField] private Renderer barRenderer;
        [SerializeField] private bool useText = true;
        [DrawIf(nameof(useText), true)][SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private bool useDelay = false;
        [DrawIf(nameof(useDelay), true)][SerializeField] private Renderer delayedBar;

        [Title("Shader")]
        [SerializeField] private string shaderFillName = "_ClipUvRight";
        [SerializeField] private Vector2 minMaxShaderValues = new(1f, 0f);

        public Vector2 MinMaxValues
        {
            get => minMaxValues;
            set
            {
                minMaxValues = value;
                SetValue(Value);
            }
        }
        [Title("Initialization")]
        [SerializeField] private Vector2 minMaxValues = new(0, 1);
        [SerializeField] private float defaultValue = 0f;
        [SerializeField] private bool clampDefaultValue = false;
        [DrawIf(nameof(useText), true)][SerializeField] private ProgressTextFormat textFormat = ProgressTextFormat.PercentDefault;
        [DrawIf(nameof(useText), true)][SerializeField] private string additionalText;
        [DrawIf(nameof(useDelay), true)][SerializeField][Min(0.01f)] private float delay = 0.1f;
        [DrawIf(nameof(useDelay), true)][SerializeField] private SmoothLayer delayedBarLayer = new();
        [SerializeField] private bool updateOnAwake = true;

        /// <summary>
        /// [minMaxValues.x .. minMaxValues.y]
        /// </summary>
        public float Value
        {
            get => value;
            set => SetValue(value);
        }
        [Title("Read Only")]
        [SerializeField][ReadOnly] private float value;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            if (!updateOnAwake) return;
            if (clampDefaultValue)
                defaultValue = Mathf.Clamp(defaultValue, minMaxValues.x, minMaxValues.y);
            Value = defaultValue;
        }

        public void SetValue(float value)
        {
            this.value = value;
            UpdateShaderValue(barRenderer, Value);
            TryUpdateText();
            TryUpdateDelayedBar();
            OnValueChanged?.Invoke(Value);
        }
        private void TryUpdateDelayedBar()
        {
            if (!useDelay) return;
            if (delayedBar == null)
            {
                Debug.LogWarning("Delayed Bar is null");
                return;
            }
            float currentLayerWeight = delayedBar.material.GetFloat(shaderFillName);
            float finalLayerWeight = CustomMath.ConvertValueFromTo(minMaxValues, value, minMaxShaderValues);
            delayedBarLayer.ChangeLayerWeight(currentLayerWeight, finalLayerWeight, delay, x => UpdateShaderValueRaw(delayedBar, x));
        }
        private void TryUpdateText()
        {
            if (!useText) return;
            progressText.text = GetText(textFormat, Value, MinMaxValues.x, MinMaxValues.y) + additionalText;
        }
        private void UpdateShaderValueRaw(Renderer renderer, float value)
        {
            if (renderer == null)
            {
                Debug.LogWarning("Renderer is null");
                return;
            }
            renderer.material.SetFloat(shaderFillName, value);
        }
        private void UpdateShaderValue(Renderer renderer, float unscaledValue)
        {
            if (renderer == null)
            {
                Debug.LogWarning("Renderer is null");
                return;
            }
            UpdateShaderValueRaw(renderer, CustomMath.ConvertValueFromTo(minMaxValues, unscaledValue, minMaxShaderValues));
        }
        #endregion methods

        private enum ProgressTextFormat
        {
            [InspectorName("Value * 100 [Percent]")] PercentDefault,
            [InspectorName("Value \\ MaxValue")] MaxDefault,
            [InspectorName("MinValue \\ Value")] MinDefault,
            [InspectorName("Value \\ MaxValue : F1")] MaxF1,
            [InspectorName("MinValue \\ Value : F1")] MinF1,
        }
        private static string GetText(ProgressTextFormat ptf, float value, float min, float max) => ptf switch
        {
            ProgressTextFormat.PercentDefault => $"{CustomMath.InverseLerpVector(new(min, max), value) * 100:F0}%",
            ProgressTextFormat.MaxDefault => $"{value:F0}/{max:F0}",
            ProgressTextFormat.MinDefault => $"{min:F0}/{value:F0}",
            ProgressTextFormat.MaxF1 => $"{value:F1}/{max:F1}",
            ProgressTextFormat.MinF1 => $"{min:F1}/{value:F1}",
            _ => "Undefined"
        };
    }
}