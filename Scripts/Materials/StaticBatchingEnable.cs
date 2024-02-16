using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Materials
{
    /// <summary>
    /// Place this component on root of the static objects.
    /// Can be used with Instantiating
    /// </summary>
    [DisallowMultipleComponent]
    public class StaticBatchingEnable : MonoBehaviour
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        private void Awake()
        {
            BatchThis();
        }
        private void BatchThis() => Batch(gameObject);
        private void Batch(GameObject go) => StaticBatchingUtility.Combine(go);
        #endregion methods
    }
}