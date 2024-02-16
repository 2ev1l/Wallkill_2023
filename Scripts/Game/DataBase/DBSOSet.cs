using Data.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public class DBSOSet<TypeSO> where TypeSO : DBScriptableObjectBase
    {
        #region fields & properties
        public IReadOnlyList<TypeSO> Data => data;
        [SerializeField] protected List<TypeSO> data = new();
        #endregion fields & properties

        #region methods
        public TypeSO GetObjectById(int id) => data[id];
        /// <summary>
        /// If you find by something like id, probably you need <see cref="GetObjectById(int)"/>
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public TypeSO Find(System.Predicate<TypeSO> match) 
        {
            return data.Find(match);
        }

        [ContextMenu("Collect All")]
        public void CollectAll()
        {
            IEnumerable<TypeSO> found = Resources.FindObjectsOfTypeAll<TypeSO>();
            found = found.OrderBy(x => x.ObjectId);
            if (data.Count != found.Count())
                Debug.Log($"Data in DBSOSet<{typeof(TypeSO)}> is outdated. Update and save it.");
            data = found.ToList();
        }
        private void CheckObjectId(TypeSO el)
        {
            int elId = el.ObjectId;
            if (data.Count(x => x.ObjectId == elId) > 1)
                Debug.LogError($"Wrong name in {el.name}", el);
        }
        /// <summary>
        /// Don't need to invoke this with <see cref="CatchExceptions(Action{TypeSO}, string)"/>
        /// </summary>
        public void CatchDefaultExceptions()
        {
            foreach (TypeSO el in Data)
            {
                CheckObjectId(el);
            }
        }
        public void CatchExceptions(System.Action<TypeSO> checkAction, string errorMessage)
        {
            foreach (TypeSO el in Data)
            {
                CheckObjectId(el);
                try { checkAction.Invoke(el); }
                catch (Exception e) { Debug.LogError($"{el.name}: Error message: {errorMessage}. Exception: {e.Message}", el); }
            }
        }
        #endregion methods
    }
}