using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI.Triggers;

namespace Game.Rooms.Mechanics
{
    public class SetParent : MonoBehaviour
    {
        #region fields & properties
        [HelpBox("You need to make sure manually that Player is not staying on trigger before object is disabled", HelpBoxMessageType.Warning)]
        [SerializeField] private TriggerCatcher triggerCatcher;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private bool isStay = false;
        [SerializeField][ReadOnly] private bool isInitialized = false;
        private static Transform stayingParent;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            Init();
        }
        private void OnEnable()
        {
            triggerCatcher.OnEnterTagSimple += SetPlayerParent;
            triggerCatcher.OnExitTagSimple += ResetPlayerParent;
            
        }
        private void OnDisable()
        {
            triggerCatcher.OnEnterTagSimple -= SetPlayerParent;
            triggerCatcher.OnExitTagSimple -= ResetPlayerParent;

            //if (isStay) //this is not working
            //    ResetPlayerParent();
        }
        private void Init()
        {
            if (isInitialized) return;
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            isInitialized = true;
        }
        private void ResetPlayerParent()
        {
            isStay = false;
            if (stayingParent != transform) return;
            InstancesProvider.Instance.PlayerAttack.CharacterController.transform.SetParent(null);
        }
        private void SetPlayerParent()
        {
            isStay = true;
            stayingParent = transform;
            InstancesProvider.Instance.PlayerAttack.CharacterController.transform.SetParent(transform);
        }
        #endregion methods
    }
}