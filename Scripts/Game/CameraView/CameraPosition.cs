using UnityEngine;
using UnityEngine.Events;
using Universal.UI;
using EditorCustom.Attributes;
using Game.Rooms;
using Universal.UI.Layers;
using Universal;

namespace Game.CameraView
{
    [RequireComponent(typeof(CameraController))]
    public class CameraPosition : MonoBehaviour
    {
        #region fields & properties
        public UnityAction<bool> OnFollowChanged;

        public Transform CenterOffset => centerOffset;
        [SerializeField] private Transform centerOffset;
        [SerializeField] private Transform centerObject;
        public Transform CameraObject => cameraObject;
        [SerializeField] private Transform cameraObject;
        [SerializeField] private Transform target;
        public bool IsFollow
        {
            get => isFollow;
            set
            {
                if (value) isFollow = CheckTarget();
                else isFollow = value;
                OnFollowChanged?.Invoke(isFollow);
            }
        }

        [SerializeField][Range(0.01f, 10f)] private float followTargetSpeed = 1f;
        [SerializeField] private Vector3 followPlayerCenterOffset = Vector3.up;
        [SerializeField] private Vector3 followPlayerCameraOffset = Vector3.forward;

        public bool ResetRotationByDefault
        {
            get => resetRotationByDefault;
            set => resetRotationByDefault = value;
        }
        [SerializeField] private bool resetRotationByDefault = true;
        public bool ResetPositionByDefault
        {
            get => resetPositionByDefault;
            set => resetPositionByDefault = value;
        }
        [SerializeField] private bool resetPositionByDefault = true;
        public bool LerpFollowRotation
        {
            get => lerpFollowRotation;
            set => lerpFollowRotation = value;
        }
        [SerializeField] private bool lerpFollowRotation = true;

        [SerializeField] private VectorLayer centerMoveLayer;
        [SerializeField] private TimeDelay centerMoveChangeReload;
        [SerializeField] private VectorLayer localOffsetLayer;
        [SerializeField] private TimeDelay localOffsetChangeReload;

        [Header("Read Only")]
        [SerializeField][ReadOnly] private bool isFollow = false;
        [SerializeField][ReadOnly] private Vector3 centerStartLocalPosition;
        public Vector3 CameraStartLocalPosition => cameraStartLocalPosition;
        [SerializeField][ReadOnly] private Vector3 cameraStartLocalPosition;
        [SerializeField][ReadOnly] private Quaternion cameraStartLocalRotation;
        [SerializeField][ReadOnly] private Quaternion centerStartLocalRotation;
        #endregion fields & properties

        #region methods
        private void Start()
        {
            StoreDefaultValues();
        }
        public void UpdateCameraCenterSmoothly(Vector3 newWorldPosition, float time)
        {
            if (!centerMoveChangeReload.CanActivate) return;
            centerMoveChangeReload.Activate();
            centerMoveLayer.ChangeVector(centerOffset.position, newWorldPosition, time, x => centerOffset.position = x, delegate { centerOffset.position = newWorldPosition; }, null);
        }
        public void UpdateCameraLocalOffsetSmoothly(Vector3 newLocalPosition, float time)
        {
            if (!localOffsetChangeReload.CanActivate) return;
            localOffsetChangeReload.Activate();
            localOffsetLayer.ChangeVector(cameraStartLocalPosition, newLocalPosition, time, x => cameraStartLocalPosition = x, delegate { cameraStartLocalPosition = newLocalPosition; }, null);
        }
        public void UpdateCameraCenter(Vector3 newWorldPosition)
        {
            centerOffset.position = newWorldPosition;
        }
        private void StoreDefaultValues()
        {
            cameraStartLocalPosition = cameraObject.localPosition;
            cameraStartLocalRotation = cameraObject.localRotation;

            centerStartLocalPosition = centerObject.localPosition;
            centerStartLocalRotation = centerObject.localRotation;
        }
        public void SetValues()
        {
            if (IsFollow) SetFollowValues();
            else SetDefaultValues();
        }
        private void SetDefaultValues()
        {
            float lerpT = Time.deltaTime * followTargetSpeed;
            if (ResetPositionByDefault)
            {
                LerpLocalPosition(centerObject, centerStartLocalPosition, lerpT);
                LerpLocalPosition(cameraObject, cameraStartLocalPosition, lerpT);
            }

            if (ResetRotationByDefault)
            {
                LerpLocalRotation(centerObject, centerStartLocalRotation, lerpT);
                LerpLocalRotation(cameraObject, cameraStartLocalRotation, lerpT);
            }
        }
        private void SetFollowValues()
        {
            float lerpT = Time.deltaTime * followTargetSpeed / 3f;
            Vector3 finalCenterPosition = target.position + followPlayerCenterOffset;
            Vector3 finalCameraPosition = target.position + GetOffsetBasedOnTransform(target, followPlayerCameraOffset);

            LerpPosition(centerObject, finalCenterPosition, lerpT);
            LerpPosition(cameraObject, finalCameraPosition, lerpT);
            
            if (lerpFollowRotation)
            {
                Quaternion finalCameraRotation = Quaternion.LookRotation(new(target.forward.x + cameraStartLocalRotation.x, 0, target.forward.z + cameraStartLocalRotation.z), Vector3.up);
                LerpRotation(cameraObject, finalCameraRotation, lerpT);
            }
        }
        [SerializedMethod]
        public void ChangeTargetForwardAxis(Transform targetForward)
        {
            CustomAnimation.RotateToDirectionForwardSmooth(target, targetForward.forward, 0.1f);
        }

        [ContextMenu("Follow Player")]
        [SerializedMethod]
        public void FollowPlayer() => FollowTarget(InstancesProvider.Instance.PlayerAttack.CharacterController.transform);
        public void FollowTarget(Transform newTarget)
        {
            target = newTarget;
            FollowTarget();
        }
        public void FollowTarget()
        {
            IsFollow = true;
        }

        [ContextMenu("Disable Follow")]
        [SerializedMethod]
        public void DisableFollow()
        {
            IsFollow = false;
        }
        private bool CheckTarget()
        {
            if (target == null)
            {
                Debug.LogError("Can't find a target. Follow set to default");
                IsFollow = false;
                return false;
            }
            return true;
        }


        private void LerpLocalPosition(Transform transform, Vector3 newPosition, float t)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, t);
        }
        private void LerpPosition(Transform transform, Vector3 newPosition, float t)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, t);
        }
        private void LerpLocalRotation(Transform transform, Quaternion newRotation, float t)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, t);
        }
        private void LerpRotation(Transform transform, Quaternion newRotation, float t)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, t);
        }
        private Vector3 GetOffsetBasedOnTransform(Transform relative, Vector3 offset)
        {
            return (relative.right * offset.x) +
                   (relative.up * offset.y) +
                   (relative.forward * offset.z);
        }
        #endregion methods
    }
}