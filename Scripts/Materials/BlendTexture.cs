using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI.Triggers;
using Universal.UI;

namespace Materials
{
    public class BlendTexture : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private List<Renderer> renders;
        [SerializeField] private int materialIndex = 0;
        [SerializeField] private string blendShaderName = "_Blend";
        [Header("Settings")]
        [SerializeField][Range(0, 10)] private float secondsToChange = 1f;

        [Header("Trigger")]
        [SerializeField] private bool changeOnTrigger;
        [SerializeField] private bool blendUpOnEnter = true;
        [SerializeField] private TriggerCatcher triggerCatcher;

        [SerializeField] private ValueTimeChanger valueTimeChanger;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (changeOnTrigger)
            {
                triggerCatcher.OnEnterTagSimple += blendUpOnEnter ? BlendUp : BlendDown;
                triggerCatcher.OnExitTagSimple += blendUpOnEnter ? BlendDown : BlendUp;
            }
        }
        private void OnDisable()
        {
            if (triggerCatcher != null)
            {
                triggerCatcher.OnEnterTagSimple -= BlendUp;
                triggerCatcher.OnEnterTagSimple -= BlendDown;
                triggerCatcher.OnExitTagSimple -= BlendUp;
                triggerCatcher.OnExitTagSimple -= BlendDown;
            }
            BlendDown();
        }

        [ContextMenu("Blend Up")]
        public void BlendUp() => ChangeBlendTo(1);
        [ContextMenu("Blend Down")]
        public void BlendDown() => ChangeBlendTo(0);
        private void ChangeBlendTo(float value)
        {
            int rendersCount = renders.Count;
            for (int i = 0; i < rendersCount; ++i)
            {
                Material mat = GetMaterial(i);
                valueTimeChanger = new(GetMaterial(i).GetFloat(blendShaderName), value, secondsToChange, valueTimeChanger.Curve, x => mat.SetFloat(blendShaderName, x));
            }
        }
        private Material GetMaterial(int id) => renders[id].materials[materialIndex];
        #endregion methods
    }
}