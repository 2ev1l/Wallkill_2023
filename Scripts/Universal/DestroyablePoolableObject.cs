using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using EditorCustom.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Universal
{
    public class DestroyablePoolableObject : MonoBehaviour, IPoolableObject<DestroyablePoolableObject>
    {
        #region fields & properties
        public UnityAction<DestroyablePoolableObject> OnDestroyed { get; set; }
        public bool IsUsing { get; set; }

        protected float LiveTime
        {
            get => liveTime;
            set
            {
                liveTime = value;
                if (liveTime > destroyTime)
                    destroyTime = liveTime + 1;
                Init();
            }
        }
        [Title("Settings")][SerializeField][Min(0)] private float liveTime = 3f;
        [SerializeField] private bool randomizeLiveTime = true;
        [DrawIf(nameof(randomizeLiveTime), true)][SerializeField][Range(0.001f, 0.999f)] private float randomizeScale = 0.1f;
        [SerializeField][Min(0)] private float destroyTime = 60f;
        #endregion fields & properties

        #region methods
        private void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
        private void DestroyObject()
        {
            DisableObject();
            Destroy(gameObject);
        }
        public void DisableObject()
        {
            gameObject.SetActive(false);
            IsUsing = false;
            Invoke(nameof(DestroyObject), destroyTime);
        }
        private void Init()
        {
            CancelInvoke(nameof(DisableObject));
            CancelInvoke(nameof(DestroyObject));
            Invoke(nameof(DisableObject), randomizeLiveTime ? Random.Range(liveTime * (1f - randomizeScale), liveTime * (1f + randomizeScale)) : liveTime);
        }
        public DestroyablePoolableObject InstantiateThis()
        {
            DestroyablePoolableObject obj = Instantiate(this);
            obj.gameObject.SetActive(true);
            obj.Init();
            return obj;
        }

        public DestroyablePoolableObject RevertChangedParams()
        {
            gameObject.SetActive(true);
            Init();
            return this;
        }
        #endregion methods
    }
}