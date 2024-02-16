using Data;
using Data.Interfaces;
using Data.Settings;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Universal.UI
{
    public class HelpUpdater : MonoBehaviour, IInitializable
    {
        #region fields & properties
        private Vector3 defaultBodyScale;
        [Tooltip("Used to modify the position based on cursor speed")]
        [SerializeField] private float directionScale = 0.1f;
        [Tooltip("Use Vector.zero for body placement")]
        [SerializeField] private GameObject body;
        [Tooltip("Use the third quarter to place the panel")]
        [SerializeField] private Transform negativeMirror;
        protected Vector3 startMirror;
        private bool isParamsInitialized = false;
        public bool State { get; private set; }
        private Camera OverlayCamera
        {
            get
            {
                if (overlayCamera == null)
                    overlayCamera = GameObject.Find("Overlay Camera").GetComponent<Camera>();
                return overlayCamera;
            }
        }
        private Camera overlayCamera = null;
        #endregion fields & properties

        #region methods
        public virtual void Init()
        {
            if (!isParamsInitialized)
            {
                defaultBodyScale = body.transform.localScale;
                startMirror = negativeMirror.localPosition;
                isParamsInitialized = true;
            }
            body.SetActive(false);
            UpdateBody();
        }
        private void OnEnable()
        {
            SettingsData.Data.OnGraphicsChanged += delegate { UpdateBody(); };
        }
        private void OnDisable()
        {
            SettingsData.Data.OnGraphicsChanged -= delegate { UpdateBody(); };
        }

        private void UpdateBody()
        {
            body.transform.localScale = defaultBodyScale * CustomMath.GetOptimalScreenScale();
        }
        public virtual void OpenPanel(Vector3 position)
        {
            State = true;
            body.SetActive(true);
            TransformBodyPosition(position);
        }
        private void TransformBodyPosition(Vector3 position)
        {
            position += CursorSettings.MouseDirection * directionScale;
            Vector3 newBodyPosition = new(position.x, position.y, 0);
            body.transform.position = newBodyPosition;

            newBodyPosition = body.transform.localPosition;
            newBodyPosition.z = 0;
            body.transform.localPosition = newBodyPosition;

            Vector3 rotationDirection = position;
            rotationDirection.y = 0;
            body.transform.forward = rotationDirection;

            Vector2 screenSquare = CustomMath.GetScreenSquare(OverlayCamera.transform.position, position);
            Vector3 newMirrorPosition = startMirror;
            newMirrorPosition.x *= screenSquare.x;
            newMirrorPosition.y *= screenSquare.y;
            newMirrorPosition.z = 0;
            negativeMirror.localPosition = newMirrorPosition;
        }
        public void HidePanel()
        {
            if (body != null)
            {
                body.SetActive(false);
                State = false;
            }
        }
        #endregion methods
    }
}