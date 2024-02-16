using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Menu
{
    public abstract class TextItemList<I> : MonoBehaviour
    {
        #region fields & properties
        public ItemList<TextItem<I>, I> ItemsList => itemsList;
        [SerializeField] private ItemList<TextItem<I>, I> itemsList;
        public IReadOnlyList<TextItem<I>> CurrentPageItems => itemsList.CurrentPageItems;
        #endregion fields & properties

        #region methods
        public virtual void UpdateListData(List<I> list)
        {
            itemsList.UpdateListDefault(list, x => x);
        }
        public void SwitchPage(bool isNext) => itemsList.SwitchPage(isNext);
        #endregion methods
    }
}