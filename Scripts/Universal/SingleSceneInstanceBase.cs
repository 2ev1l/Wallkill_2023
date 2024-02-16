using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal
{
    /// <summary>
    /// Required for script execution order.
    /// </summary>
    public abstract class SingleSceneInstanceBase : MonoBehaviour
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected virtual void Awake()
        {
            TrySetInstance();
        }
        protected abstract void TrySetInstance();
        #endregion methods
    }
}