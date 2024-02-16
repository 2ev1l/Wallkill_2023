using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Rooms.Mechanics
{
    public class EventStates : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private List<UnityEvent> states;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void ApplyState(int id) => states[id].Invoke();
        #endregion methods
    }
}