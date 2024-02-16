using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.CameraView
{
    public class DevCameraFollow : MonoBehaviour
    {
        #region fields & properties
        [SerializeField][Min(0)] private float sensitivity = 5f;
        [SerializeField][Min(0)] private float moveSpeed = 5f;
        #endregion fields & properties

        #region methods
        public void StartFollow()
        {
            gameObject.SetActive(true);
            ResetPosition();
            InstancesProvider.Instance.OverlayStateMachine.ApplyState(7);
        }
        public void EndFollow()
        {
            gameObject.SetActive(false);
            InstancesProvider.Instance.OverlayStateMachine.ApplyState(0);
        }
        public void ResetPosition()
        {
            transform.position = InstancesProvider.Instance.PlayerAttack.CharacterController.transform.position + Vector3.up;
        }
        private void Update()
        {
            Transform cam = InstancesProvider.Instance.CameraPosition.CameraObject;

            if (Input.GetKey(KeyCode.W))
                MoveTo(cam.forward);

            if (Input.GetKey(KeyCode.A))
                MoveTo(-cam.right);

            if (Input.GetKey(KeyCode.S))
                MoveTo(-cam.forward);

            if (Input.GetKey(KeyCode.D))
                MoveTo(cam.right);

            if (Input.GetKey(KeyCode.LeftShift))
                MoveTo(cam.up);

            if (Input.GetKey(KeyCode.LeftControl))
                MoveTo(-cam.up);

            if (Input.GetKey(KeyCode.Mouse1))
                RotateToDirection();
        }
        private void RotateToDirection()
        {
            Transform cam = InstancesProvider.Instance.CameraPosition.CameraObject;
            Vector3 move = new(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            cam.eulerAngles += sensitivity * move;
        }
        private void MoveTo(Vector3 axis) => transform.Translate(moveSpeed * Time.deltaTime * axis, Space.Self);
        #endregion methods
    }
}