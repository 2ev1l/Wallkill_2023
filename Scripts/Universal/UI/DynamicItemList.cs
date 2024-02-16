using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Universal.UI
{
    /// <summary>
    /// Item list with dynamic positions.
    /// Warning, this class may greatly reduce performance.
    /// If you working with static objects in list, you might want to use <see cref="ItemList{T, I}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="I"></typeparam>
    [System.Serializable]
    [System.Obsolete]
    public class DynamicItemList<T, I> : ItemList<T, I> where T : Component, IListUpdater<I>
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        protected override void RemoveAt(int id, bool init = false, bool initAtLastPage = false, bool destroyObject = true)
        {
            if (destroyObject)
                GameObject.Destroy(_items[id].gameObject);
            _items.RemoveAt(id);
            _data.RemoveAt(id);
            if (init)
                Init(initAtLastPage);
        }
        public override void RemoveAtListParam(I listParam, bool init = false, bool initAtLastPage = false)
        {
            RemoveAt(_data.FindIndex(x => x.Equals(listParam)), init, initAtLastPage, true);
        }
        public override void RemoveAtLastListParam(I listParam, bool init = false, bool initAtLastPage = false)
        {
            RemoveAt(_data.FindLastIndex(x => x.Equals(listParam)), init, initAtLastPage, true);
        }
        protected override void UpdateListByData()
        {
            int dataCount = _data.Count;
            if (dataCount == 0) return;
            for (int i = 0; i < dataCount; i++)
            {
                var el = _data[i];
                if (i < Items.Count)
                    UpdateObject(el, Items[i]);
                else
                {
                    GetUpdatedObject(el, out T listUpdater);
                    Add(listUpdater);
                }
            }
        }
        #endregion methods
    }
}