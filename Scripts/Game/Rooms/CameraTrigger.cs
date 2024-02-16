using EditorCustom.Attributes;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI.Triggers;

namespace Game.Labyrinth
{
    public class CameraTrigger : MonoBehaviour
    {
        #region fields & properties
        [HelpBox("Make sure that none of the triggers is not collide with each other, or trigger catcher is not set 'on stay'", HelpBoxMessageType.Warning)]
        [SerializeField][Required] private Transform cameraOffset;
        [SerializeField][Min(0)] private float moveTime = 0.5f;
        [SerializeField] private Vector3 newCameraLocalPosition = new(0, 4, -4.25f);
        [SerializeField][Required] private TriggerCatcher triggerCatcher;
        [SerializeField] private TimeDelay triggerFixDelay = new(1f);
        #endregion fields & properties

        #region methods
        public void OnEnable()
        {
            triggerCatcher.OnEnterTagSimple += MoveCamera;
            triggerCatcher.OnStayTagSimple += OnPlayerStay;
        }
        public void OnDisable()
        {
            triggerCatcher.OnEnterTagSimple -= MoveCamera;
            triggerCatcher.OnStayTagSimple -= OnPlayerStay;
        }
        private void OnPlayerStay()
        {
            if (!triggerFixDelay.CanActivate) return;
            triggerFixDelay.Activate();
            if (InstancesProvider.Instance.CameraPosition.CameraStartLocalPosition.Approximately(newCameraLocalPosition, 0.5f) &&
                InstancesProvider.Instance.CameraPosition.CenterOffset.position.Approximately(cameraOffset.position, 0.5f)) return;
            MoveCamera();
        }
        private void MoveCamera()
        {
            InstancesProvider.Instance.CameraPosition.UpdateCameraCenterSmoothly(cameraOffset.position, moveTime);
            InstancesProvider.Instance.CameraPosition.UpdateCameraLocalOffsetSmoothly(newCameraLocalPosition, moveTime);
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField] private BoxCollider calculateCollider;
        [SerializeField] private float calculatedZMin;
        [SerializeField] private float calculatedZMax;
        [SerializeField] private float calculatedZAvg;

        private static readonly float calculateSafeZoneCameraPositionZ = -4.25f;
        private static readonly float calculateSafeZoneColliderScale = 9f;

        private void CalculateSafeOffset()
        {
            if (calculateCollider == null) return;
            if (calculateCollider.transform.localScale != Vector3.one)
            {
                Debug.LogError("Local scale must equals Vector3.one");
                return;
            }
            float minScale = Mathf.Min(calculateCollider.size.x, calculateCollider.size.z);
            float maxScale = Mathf.Max(calculateCollider.size.x, calculateCollider.size.z);
            float avgScale = (calculateCollider.size.x + calculateCollider.size.z) / 2f;
            calculatedZMin = minScale * calculateSafeZoneCameraPositionZ / calculateSafeZoneColliderScale;
            calculatedZMax = maxScale * calculateSafeZoneCameraPositionZ / calculateSafeZoneColliderScale;
            calculatedZAvg = avgScale * calculateSafeZoneCameraPositionZ / calculateSafeZoneColliderScale;
        }
        private void CheckFields()
        {
            if (newCameraLocalPosition.y < 0)
                Debug.LogError("Camera local position.y must be > 0", this);

            if (newCameraLocalPosition.z > 0)
                Debug.LogError("Camera local position.z must be < 0", this);
        }
        private void OnDrawGizmosSelected()
        {
            CheckFields();
            CalculateSafeOffset();
            UnityEditor.Handles.DrawWireDisc(cameraOffset.transform.position + Vector3.up * newCameraLocalPosition.y, Vector3.up, Mathf.Abs(newCameraLocalPosition.z), 5f);
        }
#endif //UNITY_EDITOR
    }
}