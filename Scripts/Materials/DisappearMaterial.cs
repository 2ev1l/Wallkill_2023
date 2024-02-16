using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI.Layers;

namespace Materials
{
    public class DisappearMaterial : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Renderer render;
        [SerializeField] private string exposedPropertyName = "_Burn_Amount";
        [SerializeField][Min(0)] private int disappearMaterialIndex = 1;

        [Title("Animations")]
        [SerializeField] private SmoothLayer changeSmoothLayer;

        [Title("Additional Options")]
        [SerializeField] private bool setOnlyMaterialOnFilled = true;
        [SerializeField] private bool revertBurnOnFilled = true;
        [SerializeField][DrawIf(nameof(revertBurnOnFilled), true)][Min(0)] private float revertBurnTime = 0.5f;
        private Material AffectedMaterial
        {
            get
            {
                if (affectedMaterial == null)
                    affectedMaterial = render.materials[disappearMaterialIndex];
                return affectedMaterial;
            }
        }
        private Material affectedMaterial;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">0 = Full Color, 1 = Empty Color</param>
        public void ChangeAmount(float amount, float time)
        {
            changeSmoothLayer.ChangeLayerWeight(GetProperty(), amount, time, SetProperty, delegate { SetProperty(amount); OnChangeEnd(); });
        }
        private void OnChangeEnd()
        {
            if (this == null) return;
            StartCoroutine(DelayOnChangeEnd());
        }
        private IEnumerator DelayOnChangeEnd()
        {
            yield return CustomMath.WaitAFrame();
            bool isFilled = GetProperty() <= 0.001f;
            if (isFilled)
            {
                if (setOnlyMaterialOnFilled)
                {
                    SetOnlyMaterial();
                }
                if (revertBurnOnFilled)
                {
                    changeSmoothLayer.ChangeLayerWeight(GetProperty(), 1, revertBurnTime, SetProperty);
                }
            }
        }
        public void SetOnlyMaterial() => render.materials = new Material[] { AffectedMaterial };
        private float GetProperty() => AffectedMaterial.GetFloat(exposedPropertyName);
        private void SetProperty(float value) => AffectedMaterial.SetFloat(exposedPropertyName, value);
        #endregion methods

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            CheckEditorBugs();
        }
        private void CheckEditorBugs()
        {
            if (Application.isPlaying) return;
            if (render != null)
            {
                if (disappearMaterialIndex > render.sharedMaterials.Length - 1)
                {
                    disappearMaterialIndex = render.sharedMaterials.Length - 1;
                    Debug.LogError("Disappear material index is greater than render materials", this);
                }
            }
        }
#endif
    }
}