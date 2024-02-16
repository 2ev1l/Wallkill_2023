using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Materials
{
    public class ShaderValueChanger : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Renderer render;
        [SerializeField] private string exposedName = "_Glow";
        [SerializeField] private float minValue = 5f;
        [SerializeField] private float maxValue = 11f;
        [SerializeField] private float time = 5f;
        private Material Material
        {
            get
            {
                if (material == null)
                    material = render.material;
                return material;
            }
        }
        private Material material;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            StartCoroutine(Loop(true));
        }
        private IEnumerator Loop(bool toMax)
        {
            float value = Material.GetFloat(exposedName);
            ValueTimeChanger vtc = new(value, toMax ? maxValue : minValue, time);
            while (!vtc.IsEnded)
            {
                Material.SetFloat(exposedName, vtc.Value);
                yield return CustomMath.WaitAFrame();
            }
            StartCoroutine(Loop(!toMax));
        }
        #endregion methods
    }
}