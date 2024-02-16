using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal
{
    [System.Serializable]
    public class TimeLiveable<T> where T : class
    {
        #region fields & properties
        public UnityAction<TimeLiveable<T>> OnTimeEnded;
        public T Object { get => _object; set => _object = value; }
        [SerializeField] private T _object;
        public float TimeLeft => timeLeft;
        [SerializeField] private float timeLeft;
        public bool IsLifeEnded => isLifeEnded;
        [SerializeField] private bool isLifeEnded = false;
        public float BaseTimeLife => baseTimeLife;
        [SerializeField][Min(0)] private float baseTimeLife = 0.5f;
        #endregion fields & properties

        #region methods
        public void DecreaseTime(float time)
        {
            time = Mathf.Max(time, 0);
            timeLeft -= time;
            timeLeft = Mathf.Max(timeLeft, 0);
            if (timeLeft <= 0.001f)
            {
                timeLeft = 0f;
                isLifeEnded = true;
                OnTimeEnded?.Invoke(this);
            }
        }
        /// <summary>
        /// You highly not recommended to use this method unless you know what you exactly need
        /// </summary>
        /// <param name="value"></param>
        public void SetBaseTimeLife(float value) => baseTimeLife = value;
        public void SetBaseTimeLeft()
        {
            timeLeft = baseTimeLife;
            DecreaseTime(0);
        }
        public void IncreaseTime(float time)
        {
            time = Mathf.Max(time, 0);
            timeLeft += time;
            if (timeLeft > 0)
                isLifeEnded = false;
        }
        public TimeLiveable(float baseTimeLife)
        {
            if (baseTimeLife < 0)
                throw new System.ArgumentOutOfRangeException(nameof(baseTimeLife));
            this.baseTimeLife = baseTimeLife;
            SetBaseTimeLeft();
        }
        #endregion methods
    }
}