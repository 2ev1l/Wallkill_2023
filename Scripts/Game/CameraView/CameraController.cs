using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.CameraView
{
    public class CameraController : MonoBehaviour
    {
        #region fields & properties
        public CameraRotation CameraRotation => cameraRotation;
        [SerializeField] private CameraRotation cameraRotation;
        [SerializeField] private CameraPosition cameraPosition;
        [SerializeField] private CameraCollision cameraCollision;
        [SerializeField] private CameraCrop cameraCrop;

        [Header("Additional Settings")]
        [SerializeField] private bool disableRotationOnFollow = true;
        [SerializeField] private bool disableCollisionOnFreeView = true;
        public bool IsDevCameraEnabled => isDevCameraEnabled;
        [SerializeField] private bool isDevCameraEnabled = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            cameraPosition.OnFollowChanged += CheckRotationOnFollow;
        }
        private void OnDisable()
        {
            cameraPosition.OnFollowChanged -= CheckRotationOnFollow;
        }
        private void CheckRotationOnFollow(bool isFollow)
        {
            if (!disableRotationOnFollow)
            {
                cameraRotation.enabled = true;
                return;
            }
            cameraRotation.enabled = !isFollow;
        }
        private void Update()
        {
            SetPosition();
            DetectCollision();
            if (Input.GetKeyDown(KeyCode.F12))
                ChangeDevCamera();
        }
        private void ChangeDevCamera()
        {
            StateMachine overlayStateMachine = InstancesProvider.Instance.OverlayStateMachine.Context;
            if (isDevCameraEnabled)
                DisableDevCamera();
            else
            {
                if (overlayStateMachine.CurrentState != overlayStateMachine.DefaultState) return;
                EnableDevCamera();
            }
        }
        private void EnableDevCamera()
        {
            isDevCameraEnabled = true;
            cameraPosition.LerpFollowRotation = false;
            InstancesProvider.Instance.DevCameraFollow.StartFollow();
            cameraPosition.FollowTarget(InstancesProvider.Instance.DevCameraFollow.transform);
        }
        private void DisableDevCamera()
        {
            isDevCameraEnabled = false;
            cameraPosition.LerpFollowRotation = true;
            cameraPosition.DisableFollow();
            InstancesProvider.Instance.DevCameraFollow.EndFollow();
        }
        private void SetPosition()
        {
            if (cameraCrop.IsCropped) return;

            cameraPosition.SetValues();
        }
        private void DetectCollision()
        {
            if (!cameraPosition.IsFollow && disableCollisionOnFreeView) return;
            if (isDevCameraEnabled) return;
            
            cameraCollision.DetectCollision();
        }
        #endregion methods
    }
}