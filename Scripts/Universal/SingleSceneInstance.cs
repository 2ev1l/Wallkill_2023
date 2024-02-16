using EditorCustom.Attributes;
using System.Linq;
using UnityEngine;

namespace Universal
{
    [DisallowMultipleComponent]
    public abstract class SingleSceneInstance<T> : SingleSceneInstanceBase where T : SingleSceneInstance<T>
    {
        #region fields & properties
        public static T Instance
        {
            get
            {
                return instance;
            }
        }
        private static T instance;
        [HelpBox("This object is singleton.", HelpBoxMessageType.Info)]
        [SerializeField][ReadOnly][Label("Current Instance")] private T i;
        #endregion fields & properties

        #region methods
        protected override void Awake()
        {
            base.Awake();
            if (instance == (T)this)
                OnAwake();
        }
        protected override void TrySetInstance()
        {
            if (instance == null)
            {
                instance = (T)this;
                i = instance;
                OnInstanceSet();
            }
            else if (instance != (T)this)
            {
                Debug.LogError($"More than 1 instance of {instance.GetType()}. Fix - destroying in '{gameObject.name}'", gameObject);
                Destroy(this);
                return;
            }
        }
        protected virtual void OnInstanceSet() { }
        protected virtual void OnAwake() { }
        #endregion methods
    }
}