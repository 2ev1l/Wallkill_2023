using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class RandomizeEvent : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private List<RandomEvent> randomEvents = new();

        [Title("Settings")]
        [SerializeField] private bool randomizeEachOnce = false;
        #endregion fields & properties

        #region methods
        public void Randomize()
        {
            RandomEvent choosedEvent = null;
            if (randomEvents.Count == 0) return;

            if (randomizeEachOnce)
            {
                List<RandomEvent> fixedEvents = randomEvents.Where(x => x.RandomizedCount == 0).ToList();
                if (fixedEvents.Count == 0) return;
                choosedEvent = fixedEvents[Random.Range(0, fixedEvents.Count)];
            }
            else
                choosedEvent = randomEvents[Random.Range(0, randomEvents.Count)];
            
            if (choosedEvent == null) return;
            choosedEvent.Event?.Invoke();
            choosedEvent.RandomizedCount++;
        }
        #endregion methods

        [System.Serializable]
        private class RandomEvent
        {
            public UnityEvent Event;
            public int RandomizedCount { get; set; } = 0;
        }
    }
}