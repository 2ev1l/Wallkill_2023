using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Universal.UI
{
    /// <summary>
    /// Item list with static positions (object pooling).
    /// If you want to use dynamically changing objects it will be easier to use <see cref="DynamicItemList{T, I}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="I"></typeparam>
    [System.Serializable]
    public class ItemList<T, I> where T : Component, IListUpdater<I>
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> - currentPositions;
        /// </summary>
        public UnityAction<List<T>> OnPageSwitch;
        public UnityAction OnPageSwitched;

        public IReadOnlyList<T> Items => _items;
        public IReadOnlyList<T> CurrentPageItems => _currentPageItems;
        protected List<T> _currentPageItems = new();

        protected IReadOnlyCollection<GameObject> Positions => positions;
        [SerializeField] private GameObject[] positions;
        [SerializeField] private GameObject arrowNext;
        [SerializeField] private GameObject arrowPrev;
        [SerializeField] private T prefabRoot;
        [SerializeField] private Transform parentForSpawn;
        [SerializeField] private bool showItems = false;

        [Title("Read Only")]
        [SerializeField][ReadOnly] protected int pageCounter;
        [SerializeField][ReadOnly] protected List<T> _items = new();
        [SerializeField][ReadOnly] protected List<I> _data = new();
        #endregion fields & properties

        #region methods
        protected virtual void RemoveAt(int id, bool init = false, bool initAtLastPage = false, bool destroyObject = true)
        {
            int itemsId = id % ((pageCounter + 1) * positions.Length);
            if (destroyObject)
                GameObject.Destroy(_items[itemsId].gameObject);
            _data.RemoveAt(id);
            if (init)
                Init(initAtLastPage);
        }
        public virtual void RemoveAtListParam(I listParam, bool init = false, bool initAtLastPage = false)
        {
            RemoveAt(_data.FindIndex(x => x.Equals(listParam)), init, initAtLastPage, false);
        }
        public virtual void RemoveAtLastListParam(I listParam, bool init = false, bool initAtLastPage = false)
        {
            RemoveAt(_data.FindLastIndex(x => x.Equals(listParam)), init, initAtLastPage, false);
        }
        public void RemoveRange(int start, int end, bool init = false, bool initAtLastPage = false)
        {
            if (start > _items.Count - 1) return;
            start = Mathf.Min(start, _items.Count - 1);
            end = Mathf.Min(end, _items.Count - 1);
            int removeCount = end - start + 1;
            while (true)
            {
                //Debug.Log($"itemsCount = {_items.Count}, start = {start}, end = {end}, remove count = {removeCount}");

                int itemsId = start % ((pageCounter + 1) * positions.Length);
                GameObject.Destroy(_items[itemsId].gameObject);
                removeCount--;
                if (removeCount <= 0) break;
            }

            if (init)
                Init(initAtLastPage);
        }
        /// <summary>
        /// <see cref="Data"/> also will be dropped.
        /// </summary>
        /// <param name="init"></param>
        /// <param name="initAtLastPage"></param>
        public void RemoveAll(bool init = false, bool initAtLastPage = false)
        {
            DestroyObjects(_items);
            _items = new();
            _data = new();
            if (init)
                Init(initAtLastPage);
        }
        protected void Add(T listUpdater, bool init = false, bool initAtLastPage = false)
        {
            _items.Add(listUpdater);
            if (init)
                Init(initAtLastPage);
        }
        public void SwitchPage(bool isNext)
        {
            PageSwitch(isNext);
        }
        public virtual void UpdateCurrentPage()
        {
            int positionsCount = positions.Length;
            int start = pageCounter * positionsCount;
            int end = start + positionsCount;
            end = Mathf.Min(end, _data.Count);
            for (int i = start; i < end; ++i)
                _items[i % positionsCount].OnListUpdate(_data[i]);
        }
        public void Clear() => RemoveAll(false, false);
        protected void Init(bool atLastPage = false)
        {
            if (!atLastPage)
                pageCounter = -1;
            else
            {
                int oversizeCount = GetOversizeCount(_data.Count, pageCounter + 1);
                if (oversizeCount <= 0)
                    while (oversizeCount <= 0)
                    {
                        pageCounter--;
                        oversizeCount = GetOversizeCount(_data.Count, pageCounter + 1);
                    }
                else if (pageCounter >= 0)
                    pageCounter -= 1;
            }
            TryInitArrowsUI();
            SwitchPage(true);
        }
        protected virtual int GetOversizeCount(int dataCount, int counter) => dataCount - positions.Length * (counter);
        public virtual void ShowAt(I listParam)
        {
            int index = _data.FindIndex(x => x.Equals(listParam));
            if (index < 0) return;
            int pageIndex = index / positions.Length;
            pageCounter = pageIndex;
            Init(true);
        }
        private void TryInitArrowsUI()
        {
            if (!IsArrowsEnabled()) return;
            arrowPrev.SetActive(false);
            arrowNext.SetActive(false);
        }
        private void DestroyObjects(List<T> list)
        {
            foreach (T el in list)
                GameObject.Destroy(el.gameObject);
        }
        private void ClearPositions()
        {
            foreach (var el in positions)
            {
                int childs = el.transform.childCount;
                for (int i = 0; i < childs; ++i)
                    el.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        protected virtual void TrySetPositions(int count)
        {
            if (Items.Count == 0) return;
            if (pageCounter == -1) return;
            int endValue = Positions.Count * pageCounter + count - 1;
            _currentPageItems = new List<T>();
            int positionsCount = positions.Length;
            endValue = Mathf.Min(endValue, _data.Count - 1);
            for (int i = positionsCount * pageCounter; i <= endValue; i++)
            {
                int currentPosition = i % positionsCount;
                SetItemVisible(currentPosition);
                _currentPageItems.Add(Items[currentPosition]);
                UpdateObject(_data[i], Items[currentPosition]);
            }
            AfterPositionsSet(_currentPageItems);
        }
        protected void SetItemVisible(int id)
        {
            Transform itemTransform = _items[id].gameObject.transform;
            Vector3 oldScale = itemTransform.localScale;
            itemTransform.SetParent(positions[id % positions.Length].transform);
            itemTransform.localScale = oldScale;
            itemTransform.localPosition = Vector3.zero;
            _items[id].gameObject.SetActive(true);
        }
        protected virtual void AfterPositionsSet(List<T> currentPositions) { OnPageSwitch?.Invoke(currentPositions); OnPageSwitched?.Invoke(); }
        private void PageSwitch(bool isNext)
        {
            ClearPositions();
            int oversizeCount = 0;
            oversizeCount = isNext ? TryIncreasePage(oversizeCount) : TryDecreasePage(oversizeCount);
            TrySetArrowsUI(oversizeCount);
        }
        private int TryIncreasePage(int oversizeCount)
        {
            oversizeCount = GetOversizeCount(_data.Count, pageCounter + 1);
            pageCounter++;
            if (oversizeCount <= positions.Length)
            {
                for (int i = 1 + Mathf.Abs(oversizeCount); i < positions.Length; i++)
                    positions[i].SetActive(false);
                for (int i = 0; i < Mathf.Abs(oversizeCount); i++)
                    positions[i].SetActive(true);
                TrySetPositions(Mathf.Abs(oversizeCount));
            }
            else
            {
                foreach (var el in positions)
                    el.SetActive(true);
                TrySetPositions(positions.Length);
            }
            return oversizeCount;
        }
        private int TryDecreasePage(int oversizeCount)
        {
            oversizeCount = GetOversizeCount(_data.Count, pageCounter - 1);
            foreach (var el in positions)
                el.SetActive(true);
            pageCounter--;
            TrySetPositions(positions.Length);
            return oversizeCount;
        }
        private void TrySetArrowsUI(int oversizeCount)
        {
            if (!IsArrowsEnabled()) return;
            arrowPrev.SetActive(pageCounter > 0);
            arrowNext.SetActive(oversizeCount > positions.Length);

            if (arrowNext.transform.childCount > 0 && arrowPrev.transform.childCount > 0)
            {
                arrowNext.transform.GetChild(0).GetComponent<Text>().text = $"{pageCounter + 1}";
                arrowPrev.transform.GetChild(0).GetComponent<Text>().text = $"{pageCounter - 1}";
            }
        }
        private bool IsArrowsEnabled() => (arrowNext != null && arrowPrev != null);

        protected GameObject UpdateObject(I param, T listUpdater)
        {
            bool currentObjectState = listUpdater.gameObject.activeSelf;
            listUpdater.gameObject.SetActive(true);
            listUpdater.OnListUpdate(param);
            listUpdater.gameObject.SetActive(currentObjectState);
            return listUpdater.gameObject;
        }
        public GameObject GetUpdatedObject(I param, out T iListUpdater)
        {
            GameObject obj = GetDefaultObject(prefabRoot, out T listUpdater);
            obj = UpdateObject(param, listUpdater);
            iListUpdater = listUpdater;
            return obj;
        }
        private GameObject GetDefaultObject(Component prefabRoot, out T listUpdater)
        {
            listUpdater = GameObject.Instantiate(prefabRoot, parentForSpawn) as T;
            if (!showItems)
                listUpdater.gameObject.hideFlags = HideFlags.HideInHierarchy;
            listUpdater.gameObject.SetActive(false);
            return listUpdater.gameObject;
        }
        public void UpdateListDefault<X>(IEnumerable<X> dataList, System.Func<X, I> updateMatch)
        {
            _data = new();
            foreach (X dataItem in dataList)
            {
                _data.Add(updateMatch.Invoke(dataItem));
            }
            pageCounter = Mathf.Max(0, pageCounter);
            UpdateListByData();
            Init(true);
        }
        protected virtual void UpdateListByData()
        {
            int pageStartPosition = positions.Length * (pageCounter);
            int pageEndPosition = pageStartPosition + positions.Length;
            int positionsCount = positions.Length;
            int itemsCount = _items.Count;
            int maxExistingPageValues = Mathf.Min(_data.Count, pageEndPosition);
            for (int i = pageStartPosition; i < maxExistingPageValues; i++)
            {
                var data = _data[i];
                int currentPosition = i % positionsCount;
                if (currentPosition < itemsCount)
                    UpdateObject(data, _items[currentPosition]);
                else
                {
                    GetUpdatedObject(data, out T listUpdater);
                    Add(listUpdater);
                }
            }
        }
        #endregion methods
    }
}