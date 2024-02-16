using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.UI.Triggers
{
    public class InstantiateTrigger : DefaultTrigger
    {
        #region fields & properties
        [SerializeField] private List<InstantiateTriggerEventData> instantiates;
        #endregion fields & properties

        #region methods
        protected override void OnEnterTriggered()
        {
            foreach (var el in instantiates)
                el.Instantiate();
        }

        protected override void OnExitTriggered()
        {
            foreach (var el in instantiates)
                el.TryDestroy();
        }
        #endregion methods
        [System.Serializable]
        private class InstantiateTriggerEventData
        {
            [SerializeField] private GameObject prefab;
            [SerializeField] private Transform parent;
            [SerializeField] private Transform positionAndRotationOnSpawn;
            [SerializeField] private bool instantiateOnce = false;
            [SerializeField] private bool instantiateOnEnter = true;
            [SerializeField] private bool destroyOnExit = true;
            [SerializeField] private bool destroyAfterNewSpawn = false;
            [SerializeField] private bool stateOnSpawn = true;
            private GameObject instantiated;

            public void Instantiate()
            {
                if (!instantiateOnEnter) return;
                if (instantiated != null & instantiateOnce) return;
                if (instantiated != null && destroyAfterNewSpawn)
                    GameObject.Destroy(instantiated);
                instantiated = GameObject.Instantiate(prefab, positionAndRotationOnSpawn.position, positionAndRotationOnSpawn.rotation, parent);
                instantiated.SetActive(stateOnSpawn);
            }
            public void TryDestroy()
            {
                if (instantiated == null || !destroyOnExit) return;
                GameObject.Destroy(instantiated);
            }
        }
    }
}