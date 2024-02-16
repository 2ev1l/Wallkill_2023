using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal.UI;

namespace Menu
{
    public abstract class TextItem<T> : MonoBehaviour, IListUpdater<T>
    {
        #region fields & properties
        public T Value => value;
        protected virtual T value { get; set; }

        public TextMeshProUGUI Text => text;
        [SerializeField] private TextMeshProUGUI text;
        #endregion fields & properties

        #region methods
        public virtual void OnListUpdate(T param)
        {
            value = param;
            text.text = param.ToString();
        }
        #endregion methods
    }
}