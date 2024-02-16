using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Materials
{
    public class GPUInstancingEnable : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Renderer render;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            MaterialPropertyBlock materialPropertyBlock = new();
            render.SetPropertyBlock(materialPropertyBlock);
        }
        [ContextMenu("Get Current Renderer")]
        private void GetCurrentRenderer()
        {
            render = GetComponent<Renderer>();
        }
        #endregion methods
    }
}