using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class MeshRendererActions : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material[] newMaterials;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void ChangeMaterials()
        {
            meshRenderer.materials = newMaterials;
        }
        #endregion methods
    }
}