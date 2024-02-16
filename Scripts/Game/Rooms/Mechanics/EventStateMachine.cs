using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Rooms.Mechanics
{
    public class EventStateMachine : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private List<EventStates> states = new();
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void ApplyState(int id)
        {
            foreach (var el in states)
            {
                el.ApplyState(id);
            }
        }

        #endregion methods

#if UNITY_EDITOR
        [Button(nameof(GetAllStatesInChild))]
        private void GetAllStatesInChild()
        {
            List<EventStates> s = transform.GetComponentsInChildren<EventStates>(true).ToList();
            s = s.Where(x => !states.Contains(x)).ToList();
            UnityEditor.Undo.RecordObject(this, "New child event states");
            states.AddRange(s);
        }
#endif //UNITY_EDITOR

    }
}