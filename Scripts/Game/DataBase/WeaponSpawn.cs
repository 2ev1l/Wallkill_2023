using Data.Interfaces;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class WeaponSpawn : ICloneable<WeaponSpawn>
    {
        #region fields & properties
        public WeaponModel Prefab => prefab;
        [SerializeField] private WeaponModel prefab;
        public WeaponModel Instantiated => instantiated;
        private WeaponModel instantiated = null;
        [SerializeField] private Vector3 spawnOffset;
        [SerializeField] private Vector3 spawnScale = Vector3.one;
        #endregion fields & properties

        #region methods
        public bool TryInstantiate(Transform handTransform, out WeaponModel instantiated)
        {
            instantiated = null;
            if (prefab == null || this.instantiated != null) return false;
            instantiated = Instantiate(handTransform);
            this.instantiated = instantiated;
            return true;
        }
        public WeaponModel Instantiate(Transform handTransform)
        {
            instantiated = GameObject.Instantiate(prefab, spawnOffset, prefab.transform.rotation);
            instantiated.transform.localScale = spawnScale;
            instantiated.transform.SetParent(handTransform, true);
            instantiated.transform.SetLocalPositionAndRotation(spawnOffset, prefab.transform.rotation);
            return instantiated;
        }
        public WeaponSpawn Clone()
        {
            return new()
            {
                prefab = prefab,
                instantiated = instantiated,
                spawnOffset = spawnOffset,
                spawnScale = spawnScale
            };
        }
        #endregion methods
    }
}