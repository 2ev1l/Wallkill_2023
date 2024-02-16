using UnityEngine;

namespace Universal.UI
{
    public abstract class StateChange : MonoBehaviour
    {
        #region methods
        public abstract void SetActive(bool active);
        public virtual void SetInteraction(bool isInteractive) { }
        #endregion methods
    }
}