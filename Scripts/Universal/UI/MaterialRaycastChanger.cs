using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using EditorCustom.Attributes;

namespace Universal
{
    public class MaterialRaycastChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> isDefault;
        /// </summary>
        public UnityAction<bool> OnMaterialChanged;
        public Renderer Renderer => render;
        [SerializeField] private Renderer render;
        public Material ChangedMaterial => changedMaterial;
        [SerializeField] private Material changedMaterial;
        public Material DefaultMaterial => defaultMaterial;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private bool raycastActive = true;
        #endregion fields & properties

        #region methods
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!raycastActive) return;
            SetChangedMaterial();
        }
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!raycastActive) return;
            SetDefaultMaterial();
        }
        private void OnDisable()
        {
            if (!raycastActive) return;
            SetDefaultMaterial();
        }
        public void UpdateChangedMaterial(Material newMaterial) => changedMaterial = newMaterial;

        [SerializedMethod]
        public void SetDefaultMaterial()
        {
            Renderer.material = defaultMaterial;
            OnMaterialChanged?.Invoke(true);
        }
        [SerializedMethod]
        public void SetChangedMaterial()
        {
            Renderer.material = changedMaterial;
            OnMaterialChanged?.Invoke(false);
        }
        #endregion methods
    }
}