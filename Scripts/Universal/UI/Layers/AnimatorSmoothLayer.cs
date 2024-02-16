using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI.Layers
{
    [System.Serializable]
    public class AnimatorSmoothLayer : SmoothLayer
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public void ChangeLayerWeight(Animator animator, int layer, float finalWeight, float seconds)
        {
            Context.StartCoroutine(ChangeLayerWeightSmooth(animator, layer, finalWeight, seconds));
        }
        public IEnumerator ChangeLayerWeightSmooth(Animator animator, int layer, float finalWeight, float seconds)
        {
            vtc?.Break();
            vtc = new(animator.GetLayerWeight(layer), finalWeight, seconds, x => animator.SetLayerWeight(layer, x));
            yield return vtc.WaitUntilEnd();
        }
        #endregion methods
    }
}