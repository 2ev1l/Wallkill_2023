using Data.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using EditorCustom.Attributes;

namespace Universal
{
    [System.Serializable]
    public class ObjectPool<T> : ICloneable<ObjectPool<T>> where T : Component, IPoolableObject<T>
    {
        #region fields & properties
        public UnityAction<T> OnObjectInstantiated;
        public T OriginalPrefab => originalPrefab;
        [SerializeField] private T originalPrefab;
        public int ObjectsLimit => objectsLimit;
        [Min(-1)][SerializeField] private int objectsLimit = -1;
        /// <summary>
        /// Affects only on newly spawned objects
        /// </summary>
        public bool HideObjectsInHierarchy
        {
            get => hideObjectsInHierarchy;
            set => hideObjectsInHierarchy = true;
        }
        [SerializeField] private bool hideObjectsInHierarchy = false;
        public IReadOnlyList<T> Objects => objects;
        [SerializeField][Header("Read Only")][ReadOnly] private List<T> objects = new();
        #endregion fields & properties

        #region methods
        /// <summary>
        /// When you got an object, this object will be marked as Used.
        /// </summary>
        /// <returns></returns>
        public T GetObject()
        {
            T obj = objects.Find(x => !x.IsUsing);
            if (obj == null)
            {
                if (originalPrefab == null)
                    Debug.LogError("Original prefab is null. Need to be fixed.");
                obj = originalPrefab.InstantiateThis();
                if (hideObjectsInHierarchy) 
                    obj.gameObject.hideFlags = HideFlags.HideInHierarchy;
                obj.OnDestroyed = RemoveObject;
                if (objectsLimit < 0 || objects.Count < objectsLimit)
                    objects.Add(obj);
                OnObjectInstantiated?.Invoke(obj);
            }
            else obj.RevertChangedParams();
            obj.IsUsing = true;
            return obj;
        }
        private void RemoveObject(T obj)
        {
            obj.OnDestroyed = null;
            objects.Remove(obj);
        }
        public ObjectPool() { }
        public ObjectPool(int ojbectsLimit, T originalPrefab)
        {
            this.objectsLimit = Mathf.Max(ojbectsLimit, -1);
            this.originalPrefab = originalPrefab;
        }
        public ObjectPool(T originalPrefab)
        {
            this.originalPrefab = originalPrefab;
        }
        public ObjectPool<T> Clone()
        {
            return new()
            {
                originalPrefab = originalPrefab,
                objects = Objects.ToList(),
                objectsLimit = objectsLimit
            };
        }
        #endregion methods
    }
}