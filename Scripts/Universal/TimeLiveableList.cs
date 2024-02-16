using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Universal
{
    [System.Serializable]
    public class TimeLiveableList<T> where T : class
    {
        #region fields & properties
        public UnityAction<TimeLiveable<T>> OnLiveableDead;
        public UnityAction<TimeLiveable<T>> OnLiveableSpawn;
        public UnityAction<TimeLiveable<T>> OnDeadAlive;
        public IReadOnlyList<TimeLiveable<T>> TimeLiveables => timeLiveables;
        [SerializeField] private List<TimeLiveable<T>> timeLiveables = new();
        [SerializeField] private List<TimeLiveable<T>> timeDead = new();
        public float BaseTimeLife => baseTimeLife;
        [SerializeField][Min(0)] private float baseTimeLife = 0;
        [SerializeField][Min(0)] private float baseDeadMultiplier = 5;
        #endregion fields & properties

        #region methods
        public void DecreaseListTime(float timeDecrease)
        {
            DecreaseTimeLivablesTime(timeDecrease);
            DecreaseDeadTime(timeDecrease);
        }
        /// <summary>
        /// Probably this method works not as you think.
        /// You must use properly constructor with this method or know what you're doing.
        /// </summary>
        public void DecreaseListFrame() => DecreaseListTime(1f);
        private void DecreaseTimeLivablesTime(float timeDecrease)
        {
            int c = 0;
            while (c < timeLiveables.Count)
            {
                var el = timeLiveables[c];
                el.DecreaseTime(timeDecrease);
                if (el.IsLifeEnded)
                {
                    MoveLiveableToDead(el);
                    continue;
                }
                c++;
            }
        }
        private void DecreaseDeadTime(float timeDecrease)
        {
            int c = 0;
            while (c < timeDead.Count)
            {
                var el = timeDead[c];
                el.DecreaseTime(timeDecrease);
                if (el.IsLifeEnded)
                {
                    timeDead.Remove(el);
                    continue;
                }
                c++;
            }
        }
        private void MoveLiveableToDead(TimeLiveable<T> tl)
        {
            timeLiveables.Remove(tl);
            if (!timeDead.Contains(tl))
            {
                timeDead.Add(tl);
                tl.IncreaseTime(baseTimeLife * baseDeadMultiplier);
            }
            OnLiveableDead?.Invoke(tl);
        }
        private bool TryRemoveDeadObject(T obj)
        {
            TimeLiveable<T> dead = timeDead.Find(x => x.Object == obj);
            if (dead == null) return false;
            timeDead.Remove(dead);
            return true;
        }
        public void StackObjectFrame(T obj) => StackObject(obj, 1f);
        /// <summary>
        /// Increases object time by timeIncrease or adding new object with <see cref="baseTimeLife"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="timeIncrease">This time will also be applied to the new object</param>
        public TimeLiveable<T> StackObject(T obj, float timeIncrease)
        {
            TimeLiveable<T> tl = timeLiveables.Find(x => x.Object == obj);
            if (tl == null)
            {
                tl = new(baseTimeLife);
                tl.Object = obj;
                timeLiveables.Add(tl);
                OnLiveableSpawn?.Invoke(tl);
            }

            tl.IncreaseTime(timeIncrease);

            if (TryRemoveDeadObject(obj))
                OnDeadAlive?.Invoke(tl);
            
            return tl;
        }
        public TimeLiveableList(float baseTimeLife)
        {
            this.baseTimeLife = baseTimeLife;
        }
        public TimeLiveableList(float baseTimeLife, float baseDeadTimeMultiplier)
        {
            this.baseTimeLife = baseTimeLife;
            this.baseDeadMultiplier = baseDeadTimeMultiplier;
        }
        /// <summary>
        /// Use this, <see cref="StackObjectFrame(T)"/> and <see cref="DecreaseListFrame"/> to achieve needed result
        /// </summary>
        /// <param name="baseFrameLife"></param>
        public TimeLiveableList(int baseFrameLife)
        {
            this.baseTimeLife = baseFrameLife;
        }
        #endregion methods
    }
}