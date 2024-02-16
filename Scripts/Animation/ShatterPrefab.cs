using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    public class ShatterPrefab : MonoBehaviour
    {
        #region fields & properties
        public IReadOnlyList<Rigidbody> Rigidbodies => rigidbodies;
        [SerializeField] private List<Rigidbody> rigidbodies;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}