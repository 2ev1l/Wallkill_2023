using UnityEngine.Events;

namespace Data.Interfaces
{
    /// <summary>
    /// You must using prefabs to work properly with this interface.
    /// The original prefab values will not be changed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPoolableObject<T> where T : class
    {
        #region fields & properties
        public UnityAction<T> OnDestroyed { get; set; }
        /// <summary>
        /// Set to false when the object is not visible to user or ready to be fake instantiated again.
        /// </summary>
        public bool IsUsing { get; set; }
        #endregion fields & properties

        #region methods
        /// <summary>
        /// Use this method to set <see cref="IsUsing"/> to false and disable UI.
        /// </summary>
        public void DisableObject();
        /// <summary>
        /// Make sure to return instantiated class and not the class in prefab.
        /// Not necessary to change <see cref="IsUsing"/> in this method
        /// </summary>
        /// <returns></returns>
        public T InstantiateThis();
        /// <summary>
        /// This method is used after <see cref="IsUsing"/> was set to false and object must be (fake) instantiated again.
        /// Make sure to unsubscribe at any events, clear data, etc. like object was not instantiated </summary>
        /// <returns></returns>
        public T RevertChangedParams();
        #endregion methods
    }
}