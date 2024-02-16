using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Stored
{
    [System.Serializable]
    public class PlayerStats
    {
        #region fields & properties
        public Stat Health => health;
        [SerializeField] private Stat health = new(100, true, 0, true, 100);
        public Stat Stamina => stamina;
        [SerializeField] private Stat stamina = new(100, true, 0, true, 100);
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}