using EditorCustom.Attributes;
using Game.Animations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Animation
{
    public class SimpleAnimations : MonoBehaviour
    {
        #region fields & properties
        [Title("Move")]
        [SerializeField] private bool moveObjects = true;
        [SerializeField][DrawIf(nameof(moveObjects), true, DisablingType.ReadOnly)] private List<DelayedAction<ObjectMove>> objectsMovers;

        [Title("Rotation")]
        [SerializeField] private bool rotateObjects = true;
        [SerializeField][DrawIf(nameof(rotateObjects), true, DisablingType.ReadOnly)] private List<DelayedAction<ObjectRotate>> objectsRotate;

        [Title("State")]
        [Header("Enable")]
        [SerializeField] private bool enableObjects = true;
        [SerializeField][DrawIf(nameof(enableObjects), true, DisablingType.ReadOnly)] private List<DelayedAction<ObjectsState>> objectsToEnable;

        [Header("Disable")]
        [SerializeField] private bool disableObjects = true;
        [SerializeField][DrawIf(nameof(disableObjects), true, DisablingType.ReadOnly)] private List<DelayedAction<ObjectsState>> objectsToDisable;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        [Button(nameof(DoAnimation))]
        public void DoAnimation()
        {
            StartCoroutine(StartAnimation());
        }
        private IEnumerator StartAnimation()
        {
            List<OrderOperation> orders = new();
            AddListToOrders(orders, objectsMovers, 0);
            AddListToOrders(orders, objectsRotate, 1);
            AddListToOrders(orders, objectsToEnable, 2);
            AddListToOrders(orders, objectsToDisable, 3);

            IEnumerable<OrderOperation> ordered = orders.OrderBy(x => x.Order);

            foreach (OrderOperation el in ordered)
            {
                yield return DoOperation(el);
            }

            yield break;
        }
        private IEnumerator DoOperation(OrderOperation orderOperation)
        {
            switch (orderOperation.Operation)
            {
                case 0: yield return DoMove(orderOperation.Index); break;
                case 1: yield return DoRotate(orderOperation.Index); break;
                case 2: yield return DoEnable(orderOperation.Index); break;
                case 3: yield return DoDisable(orderOperation.Index); break;
                default: throw new System.NotImplementedException($"No operation #{orderOperation.Operation}");
            }
            yield break;
        }

        private IEnumerator DoEnable(int index)
        {
            var el = objectsToEnable[index];
            el.Data.EnableObjects();
            yield return new WaitForSeconds(el.WaitTime);
        }
        private IEnumerator DoDisable(int index)
        {
            var el = objectsToDisable[index];
            el.Data.DisableObjects();
            yield return new WaitForSeconds(el.WaitTime);
        }
        private IEnumerator DoMove(int index)
        {
            var mover = objectsMovers[index];
            mover.Data.MoveCycle();
            yield return new WaitForSeconds(mover.WaitTime);
        }
        private IEnumerator DoRotate(int index)
        {
            var rotater = objectsRotate[index];
            rotater.Data.RotateCycle();
            yield return new WaitForSeconds(rotater.WaitTime);
        }
        /// <summary>
        /// Modifies original orders list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orders">Warning, this list will be modified with new elements</param>
        /// <param name="list"></param>
        /// <param name="operation"></param>
        private void AddListToOrders<T>(List<OrderOperation> orders, List<DelayedAction<T>> list, int operation)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                orders.Add(new(list[i].Order, operation, i));
            }
        }

        #endregion methods

        [System.Serializable]
        private struct OrderOperation
        {
            public int Order => order;
            [SerializeField] private int order;
            public int Operation => operation;
            [SerializeField] private int operation;
            public int Index => index;
            [SerializeField] private int index;

            public OrderOperation(int order, int operation, int index)
            {
                this.order = order;
                this.operation = operation;
                this.index = index;
            }
        }

        [System.Serializable]
        private struct DelayedAction<T> : DelayedOrder
        {
            public T Data => data;
            [SerializeField] private T data;
            public int Order => order;
            [SerializeField] private int order;
            public float WaitTime => waitTime;
            [SerializeField][Min(0)] private float waitTime;

            public DelayedAction(float waitTime, int order, T data)
            {
                this.data = data;
                this.waitTime = waitTime;
                this.order = order;
            }
        }
        private interface DelayedOrder
        {
            public int Order { get; }
            public float WaitTime { get; }
        }
    }
}