using UnityEngine;
using UnityEngine.Events;
using EditorCustom.Attributes;
using System;

namespace Game.Player
{
    public class Crouching : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnCrouchingStart;
        public UnityAction OnCrouchingStop;

        [SerializeField] private CharacterController characterController;
        [SerializeField] private Moving moving;
        [SerializeField] private Jumping jumping;
        [SerializeField] private BoxCollider boxCollider;

        [SerializeField] private Vector3 crouchColliderCenter;
        [SerializeField] private float crouchColliderHeight;

        [SerializeField] private Vector3 crouchBoxColliderCenter;
        [SerializeField] private float crouchBoxColliderHeight;

        [SerializeField] private Vector3 crouchCheckBackOffset = new(0, 0.6f, 0);
        [SerializeField][Min(0)] private float crouchCheckBackDistance = 1;
        [SerializeField] private LayerMask crouchCheckBackMask;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private Vector3 startColliderCenter;
        [SerializeField][ReadOnly] private float startColliderHeight;

        [SerializeField][ReadOnly] private Vector3 startBoxColliderCenter;
        [SerializeField][ReadOnly] private float startBoxColliderHeight;
        public bool IsCrouching => isCrouching;
        [SerializeField][ReadOnly] private bool isCrouching;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            moving.OnControlledMoveStart += StopCrouching;
            moving.AfterPossibleMove += CheckPlayerMove;
            jumping.OnJumped += StopCrouching;
        }
        private void OnDisable()
        {
            moving.OnControlledMoveStart -= StopCrouching;
            moving.AfterPossibleMove -= CheckPlayerMove;
            jumping.OnJumped -= StopCrouching;
        }
        private void Start()
        {
            startColliderCenter = characterController.center;
            startColliderHeight = characterController.height;

            startBoxColliderCenter = boxCollider.center;
            startBoxColliderHeight = boxCollider.size.y;
        }
        private void ResetCharacterControllerCollider() => SetCharacterControllerCollider(startColliderCenter, startColliderHeight, startBoxColliderCenter, startBoxColliderHeight);
        private void SetCharacterControllerCollider() => SetCharacterControllerCollider(crouchColliderCenter, crouchColliderHeight, crouchBoxColliderCenter, crouchBoxColliderHeight);
        private void SetCharacterControllerCollider(Vector3 characterColliderCenter, float characterColliderHeight, Vector3 boxColliderCenter, float boxColliderHeight)
        {
            characterController.center = characterColliderCenter;
            characterController.height = characterColliderHeight;

            Vector3 boxSize = boxCollider.size;
            boxSize.y = boxColliderHeight;
            boxCollider.size = boxSize;
            boxCollider.center = boxColliderCenter;
        }
        private void CheckPlayerMove(bool isMoved)
        {
            if (moving.DoAccelerate)
                StopCrouching();
        }
        [ContextMenu(nameof(StartCrouching))]
        public void StartCrouching()
        {
            if (moving.IsMoveControlled || isCrouching) return;
            isCrouching = true;
            SetCharacterControllerCollider();
            moving.DoSlow = true;
            OnCrouchingStart?.Invoke();
        }
        [ContextMenu(nameof(StopCrouching))]
        public void StopCrouching()
        {
            if (!isCrouching) return;
            if (!CanStopCrouching()) return;
            isCrouching = false;
            ResetCharacterControllerCollider();
            moving.DoSlow = false;
            OnCrouchingStop?.Invoke();
        }
        public bool CanStopCrouching()
        {
            Vector3 origin = characterController.transform.position + crouchCheckBackOffset;
            return !Physics.Raycast(origin, Vector3.up, crouchCheckBackDistance, crouchCheckBackMask, QueryTriggerInteraction.Ignore);
        }
        private void OnDrawGizmos()
        {
            Vector3 origin = characterController.transform.position + crouchCheckBackOffset;
            Debug.DrawRay(origin, Vector3.up * crouchCheckBackDistance);
        }
        #endregion methods
    }
}