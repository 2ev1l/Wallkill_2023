using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.UI.Layers
{
    [System.Serializable]
    public class DefaultLayer<T>
    {
        #region fields & properties
        public int Id
        {
            get => id;
            set => id = value;
        }
        [SerializeField] private int id;
        public T Value
        {
            get => value;
            set
            {
                this.value = value;
            }
        }
        [SerializeField] protected T value;
        #endregion fields & properties

        #region methods
        public DefaultLayer(int id, T value)
        {
            this.id = id;
            this.value = value;
        }
        public DefaultLayer(int id)
        {
            this.id = id;
        }

        public DefaultLayer() { }
        #endregion methods
    }
}