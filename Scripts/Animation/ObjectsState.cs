using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Animations
{
    public class ObjectsState : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private List<GameObject> objects;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void EnableObjects() => ChangeObjectsState(true);
        [SerializedMethod]
        public void DisableObjects() => ChangeObjectsState(false);
        private void ChangeObjectsState(bool state) => objects.ForEach(x => x.SetActive(state));
        #endregion methods
    }
}