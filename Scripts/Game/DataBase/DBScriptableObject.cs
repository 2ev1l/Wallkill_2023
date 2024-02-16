using Data.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    public abstract class DBScriptableObject<T> : DBScriptableObjectBase, IDataProvider<T> where T : class
    {
        #region fields & properties
        public T Data => data;
        [SerializeField] private T data;
        #endregion fields & properties

        #region methods
        #endregion methods
    }
}