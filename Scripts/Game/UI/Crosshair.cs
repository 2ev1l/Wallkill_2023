using EditorCustom.Attributes;
using UnityEngine;
using Universal;

namespace Game.UI
{
    public class Crosshair : SingleSceneInstance<Crosshair>
    {
        #region fields & properties
        [Required][SerializeField] private SpriteRenderer crosshairRenderer;
        
        [Title("Settings")]
        [SerializeField] private Color redColor;
        [SerializeField] private Material redMaterial;

        [SerializeField] private Color defaultColor;
        [SerializeField] private Material defaultMaterial;
        #endregion fields & properties

        #region methods
        protected override void OnAwake()
        {
            ResetColor();
        }

        public void BurstColorToRed()
        {
            CancelInvoke(nameof(ResetColor));
            crosshairRenderer.color = redColor;
            crosshairRenderer.material = redMaterial;
            Invoke(nameof(ResetColor), 0.2f);
        }
        public void ResetColor()
        {
            crosshairRenderer.color = defaultColor;
            crosshairRenderer.material = defaultMaterial;
        }
        #endregion methods
    }
}