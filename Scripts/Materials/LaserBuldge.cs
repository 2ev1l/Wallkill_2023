using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;
using Universal.UI.Layers;

namespace Materials
{
    public class LaserBuldge : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Renderer render;
        [SerializeField] private int materialId;
        [SerializeField] private Vector2 pannerValues = new(-2, 2);
        [SerializeField][Min(0f)] private float speed = 1f;
        [SerializeField] private SmoothLayer smoothLayer;

        private Material Material
        {
            get
            {
                if (material == null)
                    material = render.materials[materialId];
                return material;
            }
        }
        private Material material;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            BuldgeUp();
        }
        private void BuldgeUp()
        {
            smoothLayer.ChangeLayerWeight(pannerValues.x, pannerValues.y, 1f / speed, SetPannerValue, delegate { BuldgeDown(); }, delegate { try { return !gameObject.activeInHierarchy || !enabled; } catch { return true; } });
        }
        private void BuldgeDown()
        {
            smoothLayer.ChangeLayerWeight(pannerValues.y, pannerValues.x, 1f / speed, SetPannerValue, delegate { BuldgeUp(); }, delegate { try { return !gameObject.activeInHierarchy || !enabled; } catch { return true; } });
        }
        private void SetPannerValue(float value) => Material.SetFloat("_Panner", value);
        #endregion methods
    }
}