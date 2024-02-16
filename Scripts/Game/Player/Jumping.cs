using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using EditorCustom.Attributes;

namespace Game.Player
{
    public class Jumping : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnJumped;
        public UnityAction OnFalled;

        [SerializeField] private CharacterController characterController;
        [SerializeField] private CharacterPhysics characterPhysics;
        [SerializeField] private Moving moving;
        [SerializeField] private Crouching crouching;

        public float JumpForce => jumpForce;
        [Title("Settings")]
        [SerializeField][Range(0, 3)] private float jumpForce = 1f;
        public int MaxJumps => maxJumps;
        [SerializeField][Range(0, 5)] private int maxJumps = 1;
        public float JumpDelaySeconds => jumpDelaySeconds;
        [SerializeField][Range(0, 1)] private float jumpDelaySeconds = 0.3f;
        public float MinDistanceToJump => minDistanceToJump;
        [SerializeField][Range(0, 0.3f)] private float minDistanceToJump = 0.1f;
        [SerializeField] private StatChangeLayer staminaChangeLayer;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private float lastTimeJumped;
        public float MaxHeight => (jumpForce / characterPhysics.GravityScale);
        public float FallingTime => Mathf.Sqrt(2 * MaxHeight / Mathf.Abs(characterPhysics.GravityConstant));
        public bool IsJumping => isJumping;
        [SerializeField][ReadOnly] private bool isJumping;
        public int CurrentJump => currentJump;
        [SerializeField][ReadOnly] private int currentJump;
        public float ClampedAltitude => GetClampedAltitude();
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            InitLayers();
        }
        private void InitLayers()
        {
            staminaChangeLayer.Value = StatsBehaviour.Stats.Stamina;
        }
        private void OnDisable()
        {
            isJumping = false;
        }
        public bool TryJump()
        {
            if (!CanJump()) return false;
            Jump();
            return true;
        }
        private bool CanJump()
        {
            bool isMoveControlled = moving.IsMoveControlled;
            if (isMoveControlled) return false;

            bool isGrounded = IsGrounded();
            if (isGrounded) currentJump = 0;

            bool jumpCountAllow = currentJump < MaxJumps;
            bool timeAllow = Time.realtimeSinceStartup - lastTimeJumped > JumpDelaySeconds;

            bool crouchingAllow = !crouching.IsCrouching || crouching.CanStopCrouching();

            bool staminaAllow = staminaChangeLayer.Value.MinChangesLimit > staminaChangeLayer.ChangeSpeed;
            if (!staminaAllow) //need for ui actions
            {
                staminaChangeLayer.SetChangedAmountToSpeed();
                staminaChangeLayer.TryDecreaseStat(false, false);
            }
            return isGrounded && timeAllow && jumpCountAllow && crouchingAllow && staminaAllow;
        }
        public bool IsGrounded() => IsGrounded(characterPhysics.DistanceToGround);
        public bool IsGrounded(float distanceToGround) => distanceToGround < minDistanceToJump;
        public void Jump()
        {
            currentJump++;
            float force = Mathf.Sqrt(20 * JumpForce);
            characterPhysics.AddVerticalForce(force);
            lastTimeJumped = Time.realtimeSinceStartup;
            staminaChangeLayer.SetChangedAmountToSpeed();
            staminaChangeLayer.TryDecreaseStat(false, false);
            isJumping = true;
            OnJumped?.Invoke();
            StartCoroutine(WaitUntilFall());
        }
        private IEnumerator WaitUntilFall()
        {
            yield return new WaitForSeconds(FallingTime / 2); // less wait in case of unreached height
            while (!IsGrounded())
                yield return new WaitForFixedUpdate();
            OnFalled?.Invoke();
            isJumping = false;
        }
        private float GetClampedAltitude()
        {
            float distanceToGround = characterPhysics.DistanceToGround;
            if (IsGrounded(distanceToGround)) return 0;

            float altitude = Mathf.Clamp01(characterPhysics.DistanceToGround / MaxHeight);
            return altitude;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 offsetVector = transform.position;
            float rayLength = characterPhysics.GroundCheckColliderScale * 10f;
            Vector3[] GroundCheckOffsets = characterPhysics.GetGroundCheckOffsets();
            offsetVector.y += characterPhysics.GroundCheckOffset;
            int arrayLength = GroundCheckOffsets.Length;
            for (int i = 0; i < arrayLength; ++i)
                Debug.DrawRay(offsetVector + GroundCheckOffsets[i], -Vector3.up * minDistanceToJump);
        }
#endif //UNITY_EDITOR
        #endregion methods
    }
}