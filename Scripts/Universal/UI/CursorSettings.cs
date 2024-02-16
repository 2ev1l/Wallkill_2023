using Data.Interfaces;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Universal.UI.Audio;
using AudioType = Universal.UI.Audio.AudioType;

namespace Universal.UI
{
    public class CursorSettings : MonoBehaviour, IInitializable
    {
        #region fields & properties
        /// <summary>
        /// <see cref="{T0}"/> changedValue;
        /// </summary>
        public static UnityAction<float> OnMouseWheelDirectionChanged;
        public static CursorSettings Instance { get; private set; }

        [SerializeField] private Texture2D cursorDefault;
        [SerializeField] private Texture2D cursorPoint;
        [SerializeField] private Texture2D cursorClicked;
        [SerializeField] private Vector2 cursorDefaultOffset;

        [SerializeField] private AudioClip onClickSound;

        [SerializeField] private LayerMask worldRaycastHitMask;

        public static CursorState CursorState { get; private set; }

        /// <summary>
        /// Represents direction based on screen resolution. 
        /// Use <see cref="Vector3.ClampMagnitude(Vector3, float)"/> if you want to use const value
        /// </summary>
        public static Vector3 MouseDirection { get; private set; } = Vector3.zero;
        public static float MouseWheelDirection { get; private set; } = 0;
        /// <summary>
        /// Represents position based on screen resolution
        /// </summary>
        public static Vector3 LastMousePointOnScreen { get; private set; } = Vector3.zero;
        /// <summary>
        /// Scaled <see cref="LastMousePointOnScreen"/> to (0,0)~(1,1)
        /// </summary>
        public static Vector3 LastMousePointOnScreenScaled { get; private set; } = Vector3.zero;
        public static Vector3 LastMousePoint2D { get; private set; } = Vector3.zero;
        public static Vector3 LastWorldPoint3D { get; private set; } = Vector3.zero;
        public static Ray LastRay3D { get; private set; } = new();
        public static RaycastHit LastRayHit3D { get; private set; } = new();
        private static Vector3 currentMousePosition3D = Vector3.zero;
        private static readonly string mouseWheelAxis = "Mouse ScrollWheel";
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
            SetDefaultCursor();
        }
        private void OnEnable()
        {
            ScreenFade.OnBlackScreenFading += OnBlackScreenFading;
        }
        private void OnDisable()
        {
            ScreenFade.OnBlackScreenFading -= OnBlackScreenFading;
        }

        private void OnBlackScreenFading(bool fadeUp)
        {
            if (!fadeUp) return;
            SetDefaultCursor();
        }
        public void SetDefaultCursor()
        {
            Cursor.SetCursor(cursorDefault, cursorDefaultOffset, CursorMode.Auto);
            CursorState = CursorState.Normal;
        }
        public void SetPointCursor()
        {
            Cursor.SetCursor(cursorPoint, cursorDefaultOffset, CursorMode.Auto);
            CursorState = CursorState.Point;
        }
        public void SetClickedCursor()
        {
            Cursor.SetCursor(cursorClicked, cursorDefaultOffset, CursorMode.Auto);
            CursorState = CursorState.Hold;
        }

        public void DoClickSound() => AudioManager.PlayClip(onClickSound, AudioType.Sound);

        private void Update()
        {
            MouseDirection = Input.mousePosition - LastMousePointOnScreen;
            LastMousePointOnScreen = Input.mousePosition;
            LastMousePointOnScreenScaled = Camera.main.ScreenToViewportPoint(LastMousePointOnScreen);

            MouseWheelDirection = Input.GetAxisRaw(mouseWheelAxis);
            if (MouseWheelDirection != 0)
                OnMouseWheelDirectionChanged?.Invoke(MouseWheelDirection);

            currentMousePosition3D.x = Input.mousePosition.x;
            currentMousePosition3D.y = Input.mousePosition.y;
            currentMousePosition3D.z = Camera.main.nearClipPlane;
            LastWorldPoint3D = Camera.main.ScreenToWorldPoint(currentMousePosition3D);

            LastRay3D = Camera.main.ScreenPointToRay(currentMousePosition3D);

            if (!Physics.Raycast(CursorSettings.LastRay3D, out RaycastHit hit, Mathf.Infinity, worldRaycastHitMask, QueryTriggerInteraction.Ignore)) return;
            LastRayHit3D = hit;
        }
        #endregion methods
    }
}