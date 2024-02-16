using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;
using Universal.UI.Layers;

namespace Universal
{
    [System.Serializable]
    public class TimeAverage
    {
        #region fields & properties
        /// <summary>
        /// This parameter must be updated as soon as the original value changes
        /// </summary>
        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueChanged();
            }
        }
        [SerializeField] private float value;

        [SerializeField][Min(0.01f)] private float valuesLifeTime = 1f;
        [SerializeField][Min(0f)] private float accuracyTime = 0.1f;
        [SerializeField] private TimeLiveableList<DefaultLayer<float>> averageValues;
        private float lastTimeAdded = 0;
        private float TimeBetweenChanges => Time.time - lastTimeAdded;
        private float waitedTime = 0f;
        #endregion fields & properties

        #region methods
        private void OnValueChanged()
        {
            waitedTime += TimeBetweenChanges;
            float currentTime = Time.time;
            if (waitedTime > valuesLifeTime)
            {
                int maxDecreaseTimes = Mathf.FloorToInt(waitedTime / valuesLifeTime);
                float timeSpend = valuesLifeTime * maxDecreaseTimes;
                waitedTime -= timeSpend;
                averageValues.DecreaseListTime(currentTime - valuesLifeTime);
            }

            if (TimeBetweenChanges < accuracyTime) return;
            var timeLiveable = averageValues.StackObject(new(0, value), currentTime);
            timeLiveable.SetBaseTimeLife(currentTime);
            lastTimeAdded = currentTime;
        }
        public float GetAverageSpeed(float currentValue)
        {
            int count = 0;
            float finalValue = 0f;
            float currentTime = Time.time;
            foreach (var el in averageValues.TimeLiveables)
            {
                count++;
                float timeSpent = currentTime - el.BaseTimeLife;
                float weight = 1f / timeSpent;
                finalValue += Mathf.Lerp(currentValue, el.Object.Value, weight);
            }
            float avgFinalValue = finalValue / count;
            float avg = count == 0 ? value : (avgFinalValue);
            float speed = currentValue - avg;
            return speed;
        }
        public void Init()
        {
            averageValues = new(0.01f, 0);
            lastTimeAdded = Time.time;
        }
        /// <summary>
        /// Use <see cref="Init"/> to initialize
        /// </summary>
        public TimeAverage() { }
        #endregion methods
    }
}