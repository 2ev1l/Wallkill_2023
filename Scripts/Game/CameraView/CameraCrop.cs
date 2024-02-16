using Data.Settings;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI;
using EditorCustom.Attributes;

namespace Game.CameraView
{
    public class CameraCrop : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnCropStart;
        public UnityAction OnCropEnd;

        [SerializeField] private Camera cam;
        [SerializeField][Range(5, 100)] private float cropFOV = 30;
        [SerializeField][Range(0.01f, 5)] private float minFOV = 5;
        [SerializeField][Range(0, 10)] private float sensitivity = 1f;

        public bool IsCropped => isCropped;
        [Header("Read Only")]
        [SerializeField][ReadOnly] private bool isCropped;
        [SerializeField][ReadOnly] private float lastFOV;
        [SerializeField][ReadOnly] private Quaternion lastRotation;
        [SerializeField][ReadOnly] private Vector3 lastPosition;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            InputController.OnKeyDown += CheckPressedKey;
        }
        private void OnDisable()
        {
            InputController.OnKeyDown -= CheckPressedKey;
            if (isCropped)
                RevertCrop();
        }
        private void CheckPressedKey(KeyCode key)
        {
            if (key != SettingsData.Data.KeyCodeSettings.CameraCrop.Key) return;

            if (!isCropped) StartCrop();
            else RevertCrop();
        }
        public void StartCrop()
        {
            SaveParams();
            Crop();
            isCropped = true;
            CursorSettings.OnMouseWheelDirectionChanged += ChangeFOVByWheel;
            OnCropStart?.Invoke();
        }
        public void RevertCrop()
        {
            RevertParams();
            isCropped = false;
            CursorSettings.OnMouseWheelDirectionChanged -= ChangeFOVByWheel;
            OnCropEnd?.Invoke();
        }
        private void Crop()
        {
            cam.fieldOfView = cropFOV;
            cam.transform.LookAt(CursorSettings.LastWorldPoint3D, Vector3.up);
        }
        private void ChangeFOVByWheel(float wheelDirection)
        {
            cam.fieldOfView -= wheelDirection * sensitivity;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, cropFOV);
        }
        private void SaveParams()
        {
            lastFOV = cam.fieldOfView;
            cam.transform.GetLocalPositionAndRotation(out lastPosition, out lastRotation);
        }
        private void RevertParams()
        {
            cam.fieldOfView = lastFOV;
            cam.transform.SetLocalPositionAndRotation(lastPosition, lastRotation);
        }
        #endregion methods
    }
}