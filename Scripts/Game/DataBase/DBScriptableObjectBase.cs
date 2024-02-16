using Data.Interfaces;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DataBase
{
    [System.Serializable]
    public abstract class DBScriptableObjectBase : ScriptableObject
    {
        #region fields & properties
        public int ObjectId
        {
            get
            {
                if (objectId == -1) objectId = GetObjectId();
                return objectId;
            }
        }
        [SerializeField][Min(-1)] private int objectId = -1;
        #endregion fields & properties

        #region methods
        private int GetObjectId()
        {
            string name = base.name;
            int numberStart = 1 + name.LastIndexOf(" ");
            int numberEnd = name.Length - 1;
            int count = numberEnd - numberStart + 1;
            string subString = name.Substring(numberStart, count);
            int number = System.Convert.ToInt32(subString);
            return number;
        }
        #endregion methods
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            UnityEditor.Undo.RecordObject(this, "New DB object id");
            objectId = GetObjectId();
        }
#endif //UNITY_EDITOR
    }
}