using System.Collections;
using UnityEngine;
using Universal;
using Universal.UI;
using EditorCustom.Attributes;

namespace Menu
{
    public class SceneInit : SingleSceneInstance<SceneInit>
    {
        #region fields & properties
        [SerializeField] private Material shadowMaterial;
        [SerializeField] private Material glowMaterial;
        private bool disableChangeMaterial = false;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void SetShadowMaterial(Renderer render)
        {
            StartCoroutine(ChangeGlowToValue(render, 2f, true));
            disableChangeMaterial = false;
        }

        [SerializedMethod]
        public void SetGlowMaterial(Renderer render)
        {
            render.material = new(glowMaterial);
            StartCoroutine(ChangeGlowToValue(render, 8f, false));
            disableChangeMaterial = true;
        }
        private IEnumerator ChangeGlowToValue(Renderer render, float value, bool changeMaterial)
        {
            string paramName = "_Glow";
            ValueTimeChanger vtc = new(render.material.GetFloat(paramName), value, 0.5f, x => render.material.SetFloat(paramName, x));
            while (!vtc.IsEnded)
                yield return CustomMath.WaitAFrame();
            if(changeMaterial && !disableChangeMaterial)
                render.material = new(shadowMaterial);

        }
        #endregion methods
    }
}