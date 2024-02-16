using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class DBSet<T> where T : class
    {
        #region fields & properties
        public IReadOnlyList<T> Data => data;
        [SerializeField] protected List<T> data = new();
        #endregion fields & properties

        #region methods
        public T Find(System.Predicate<T> match) => data.Find(match);
        #endregion methods
    }
}