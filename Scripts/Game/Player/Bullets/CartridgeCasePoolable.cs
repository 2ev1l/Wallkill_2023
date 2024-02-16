using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Game.Player.Bullets
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class CartridgeCasePoolable : DestroyablePoolableObject
    {
        #region fields & properties
        public Rigidbody Rigidbody => rigidBody;
        [SerializeField] private Rigidbody rigidBody;
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}