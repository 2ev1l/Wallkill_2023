using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI;
using EditorCustom.Attributes;
using Data.Settings;

namespace Game.Player
{
    public class Moving : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnControlledMoveStop;
        public UnityAction OnControlledMoveStart;

        public UnityAction OnMoved;
        /// <summary>
        /// <see cref="{T0}"/> isMoved
        /// </summary>
        public UnityAction<bool> AfterPossibleMove;

        [SerializeField] private CharacterController characterController;
        [SerializeField] private Crouching crouching;
        public Transform CameraTransform => cameraTransform;
        [SerializeField] private Transform cameraTransform;

        public float DefaultMoveSpeed => moveSpeed;
        [Title("Settings")]
        [SerializeField][Range(0, 10)] private float moveSpeed = 1f;
        public float AccelerateMultiplier => accelerateMultiplier;
        [SerializeField][Range(1, 3)] private float accelerateMultiplier = 1.5f;
        public float SlowMultiplier => slowMultiplier;
        [SerializeField][Range(0.1f, 1f)] private float slowMultiplier = 0.5f;
        public bool ResetInputAtEachFrame => resetInputAtEachFrame;
        [SerializeField] private bool resetInputAtEachFrame = true;

        [SerializeField] private StatChangeLayer accelerateStaminaDecreaseLayer;

        /// <summary>
        /// Accelerate at the end of frame. Resets every frame if <see cref="resetInputAtEachFrame"/>
        /// </summary>
        public bool DoAccelerate
        {
            get => doAccelerate;
            set
            {
                if (IsMoveControlled) return;
                if (crouching.IsCrouching && !crouching.CanStopCrouching())
                {
                    doAccelerate = false;
                    return;
                }
                doAccelerate = value;
            }
        }
        private bool doAccelerate = false;
        /// <summary>
        /// Slow at the end of frame. Doesn't reset every frame
        /// </summary>
        public bool DoSlow
        {
            get => doSlow;
            set
            {
                if (IsMoveControlled) return;
                doSlow = value;
            }
        }
        private bool doSlow = false;

        public bool LastFrameMoved => lastFrameMoved;
        private bool lastFrameMoved = false;
        public bool LastFrameAccelerate => lastFrameAccelerate;
        private bool lastFrameAccelerate = false;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private Vector3 surfaceNormal = Vector3.up;
        public float FinalSpeed => moveSpeed * (DoAccelerate ? accelerateMultiplier : 1) * (DoSlow ? slowMultiplier : 1);
        public Vector2 LastInput => lastInput;
        [SerializeField][ReadOnly] private Vector2 lastInput = Vector2.zero;
        public bool IsMoveControlled => isMoveControlled;
        [SerializeField][ReadOnly] private bool isMoveControlled = false;
        public bool IsAlwaysMovingWithCamera => isAlwaysMovingWithCamera;
        [SerializeField] private bool isAlwaysMovingWithCamera = false;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            isAlwaysMovingWithCamera = SettingsData.Data.KeyCodeSettings.AlwaysMoveWithCamera;
        }
        private void Awake()
        {
            InitializeLayers();
        }
        private void InitializeLayers()
        {
            accelerateStaminaDecreaseLayer.Value = StatsBehaviour.Stats.Stamina;
        }
        private Vector3 GetNormalizedDirection(Vector3 moveVector)
        {
            moveVector = CustomMath.Project(moveVector, surfaceNormal);
            return moveVector.normalized;
        }
        public void SetDefaultMoveSpeed(float value)
        {
            moveSpeed = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveVector">x = forward; y = right</param>
        public void SetInputMove(Vector2 moveVector)
        {
            if (isMoveControlled) return;

            lastInput = moveVector;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveVector">x = forward; y = right</param>
        public void AddInputMove(Vector2 moveVector)
        {
            if (isMoveControlled) return;

            lastInput += moveVector;
        }
        private void LateUpdate()
        {
            bool isMoved = TryMove();
            AfterPossibleMove?.Invoke(isMoved);

            lastFrameMoved = isMoved;
            lastFrameAccelerate = DoAccelerate;

            if (resetInputAtEachFrame)
                ResetLastInput();
        }
        private bool TryMove()
        {
            if (lastInput.Equals(Vector2.zero)) return false;
            CheckStamina();
            if (isMoveControlled)
            {
                DoMove(characterController.transform);
            }
            else
            {
                DoMove(isAlwaysMovingWithCamera ? cameraTransform : characterController.transform);
            }
            return true;
        }
        private void CheckStamina()
        {
            if (IsMoveControlled) return;
            if (!DoAccelerate) return;
            accelerateStaminaDecreaseLayer.IncreaseChangedAmountByTimeSpeed();
            if (!accelerateStaminaDecreaseLayer.TryDecreaseStat(true, false))
            {
                DoAccelerate = false;
            }
        }
        public void CalculateMoveVector(Vector2 lastInput, Transform relativeDirection, out Vector2 clampedInput, out Vector3 moveVector)
        {
            clampedInput = Vector2.ClampMagnitude(lastInput, 1);
            moveVector = GetMoveVector(lastInput, relativeDirection);
            moveVector = GetNormalizedDirection(moveVector);
        }
        private void DoMove(Transform relativeDirection)
        {
            CalculateMoveVector(lastInput, relativeDirection, out lastInput, out Vector3 moveVector);
            Vector3 offset = FinalSpeed * Time.deltaTime * moveVector;
            characterController.Move(offset);
            OnMoved?.Invoke();
        }
        private void ResetLastInput()
        {
            lastInput = Vector2.zero;
            DoAccelerate = false;
        }
        public void StopControlledMove()
        {
            isMoveControlled = false;
            DoSlow = false;
            OnControlledMoveStop?.Invoke();
        }
        public void StartControlledMoveFullfilled(Transform target) => StartControlledMove(target, true, true);
        [SerializedMethod]
        public void StartControlledMoveFullfilledSafe(Transform target) => StartControlledMove(target, false, true);
        public void StartControlledMoveOnly(Transform target) => StartControlledMove(target, false, false);
        public void StartControlledMove(Transform target, bool positionateAtEnd, bool rotateAtEnd)
        {
            StartCoroutine(WaitForControlledMove(target, positionateAtEnd, rotateAtEnd, false));
        }
        /// <summary>
        /// Move to [calculated once at the invoke moment] target position
        /// </summary>
        /// <param name="target"></param>
        /// <param name="positionateAtEnd"></param>
        /// <param name="rotateAtEnd"></param>
        /// <returns></returns>
        public System.Collections.IEnumerator WaitForControlledMove(Transform target, bool positionateAtEnd, bool rotateAtEnd, bool accelerate)
        {
            DoAccelerate = accelerate;
            isMoveControlled = true;
            OnControlledMoveStart?.Invoke();
            Vector3 worldPosition = target.position;

            Vector3 currentPosition = characterController.transform.position;
            Vector3 startDirection = worldPosition - currentPosition;
            float flatDistance = Vector2.Distance(new(currentPosition.x, currentPosition.z), new(worldPosition.x, worldPosition.z));

            //Rotate to point
            float secondsToRotate = 0.1f;
            yield return CustomAnimation.RotateToDirectionForwardSmoothly(characterController.transform, startDirection, secondsToRotate);

            //Move to point (just forward)
            lastInput = Vector2.right;
            float secondsToMove = flatDistance / FinalSpeed;

            ValueTimeChanger vtc = new(0, 1, secondsToMove, x => { lastInput = Vector2.right; CustomAnimation.RotateToDirectionForward(characterController.transform, startDirection, secondsToRotate); }, delegate { return !isMoveControlled; });
            yield return vtc.WaitUntilEnd();
            vtc = null;
            lastInput = Vector2.right;

            //Set final weights
            if (isMoveControlled)
            {
                if (positionateAtEnd) TeleportTo(worldPosition);
                if (rotateAtEnd)
                {
                    float rotateSeconds = 0.4f;
                    yield return CustomAnimation.RotateToDirectionForwardSmoothly(characterController.transform, target.transform.forward, rotateSeconds);
                }
            }

            StopControlledMove();
        }
        /// <summary>
        /// Teleports safely to open location
        /// </summary>
        /// <param name="position"></param>
        public void TeleportTo(Vector3 position)
        {
            characterController.Move(position - characterController.transform.position);
        }
        /// <summary>
        /// Teleports safely ignoring layers. Triggers may be activated during move
        /// </summary>
        public void TeleportToIgnoreLayer(Vector3 position, LayerMask ignoreLayers)
        {
            LayerMask oldLayers = characterController.excludeLayers;
            int oldPriority = characterController.layerOverridePriority;

            characterController.excludeLayers = ignoreLayers;
            characterController.layerOverridePriority = 100;

            TeleportTo(position);

            characterController.layerOverridePriority = oldPriority;
            characterController.excludeLayers = oldLayers;
        }
        /// <summary>
        /// May cause bugs inside the triggers
        /// </summary>
        /// <param name="position"></param>
        public void TeleportToUnsafe(Vector3 position)
        {
            bool lastCharacterControllerState = characterController.enabled;
            characterController.enabled = false;
            if (characterController.transform.parent != null) characterController.transform.SetParent(null);
            characterController.transform.position = position;
            characterController.enabled = lastCharacterControllerState;
        }
        private Vector3 GetMoveVector(Vector2 inputVector, Transform relativeDirection)
        {
            Vector3 result = Vector3.zero;

            result += relativeDirection.forward * inputVector.x;
            result += relativeDirection.right * inputVector.y;

            result = Vector3.ClampMagnitude(result, 1);
            return result;
        }
        #endregion methods
    }
}