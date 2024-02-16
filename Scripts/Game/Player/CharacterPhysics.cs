using UnityEngine;
using EditorCustom.Attributes;

namespace Game.Player
{
    public class CharacterPhysics : MonoBehaviour
    {
        #region fields & properties
        public float GravityConstant => Physics.gravity.y * GravityScale;

        [SerializeField] private CharacterController characterController;
        [SerializeField] private BoxCollider groundCheckCollider;
        public float GroundCheckOffset => groundCheckOffset;
        [SerializeField][Range(-10, 10)] private float groundCheckOffset = 0f;
        [SerializeField] private LayerMask groundLayers;
        public float GravityScale
        {
            get => gravityScale;
            set => gravityScale = value;
        }
        [SerializeField] private float gravityScale = 1f;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private Vector3 nextVelocity;
        private static readonly Vector3 deltaMoveFix = new(0.001f, 0f, 0.001f);
        public float CurrentSpeed => GetCurrentSpeed();
        public float FlyingSpeed => GetFlyingSpeed();
        public float DistanceToGround => GetDistanceToGround();

        public float GroundCheckColliderScale => groundCheckCollider.size.x * transform.localScale.x;
        public Vector3[] GroundCheckOffsets
        {
            get
            {
                groundCheckOffsets ??= GetGroundCheckOffsets();
                return groundCheckOffsets;
            }
        }
        private Vector3[] groundCheckOffsets = null;
        #endregion fields & properties

        #region methods
        public Vector3[] GetGroundCheckOffsets()
        {
            return new Vector3[]
            {
                Vector3.zero,
                new Vector3(1, 0, 1) * GroundCheckColliderScale / 2f,
                new Vector3(-1, 0, 1) * GroundCheckColliderScale / 2f,
                new Vector3(1, 0, -1) * GroundCheckColliderScale / 2f,
                new Vector3(-1, 0, -1) * GroundCheckColliderScale / 2f,

                new Vector3(0.5f, 0, 0.5f) * GroundCheckColliderScale / 2f,
                new Vector3(-0.5f, 0, 0.5f) * GroundCheckColliderScale / 2f,
                new Vector3(0.5f, 0, -0.5f) * GroundCheckColliderScale / 2f,
                new Vector3(-0.5f, 0, -0.5f) * GroundCheckColliderScale / 2f,

                new Vector3(1, 0, 0) * GroundCheckColliderScale / 2f,
                new Vector3(-1, 0, 0) * GroundCheckColliderScale / 2f,
                new Vector3(0, 0, 1) * GroundCheckColliderScale / 2f,
                new Vector3(0, 0, -1) * GroundCheckColliderScale / 2f,
            };
        }
        private void Awake()
        {
            characterController.detectCollisions = true;
        }
        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            if (!characterController.enabled) return;
            characterController.Move(deltaMoveFix * deltaTime);
            characterController.Move(-deltaMoveFix * deltaTime);

            nextVelocity.y += GravityConstant * deltaTime;
            characterController.Move(nextVelocity * deltaTime);


            if (characterController.isGrounded)
                nextVelocity.y = 0;
        }
        public void AddVerticalForce(float value)
        {
            nextVelocity.y += value;
        }
        private float GetCurrentSpeed()
        {
            Vector2 flatVelocity = new(characterController.velocity.x, characterController.velocity.z);
            return flatVelocity.magnitude;
        }
        private float GetFlyingSpeed()
        {
            return characterController.velocity.y;
        }
        private float GetDistanceToGround()
        {
            if (groundCheckCollider == null) return 0;
            Vector3 offsetVector = transform.position;
            float rayLength = 10f;
            offsetVector.y += GroundCheckOffset;
            int arrayLength = GroundCheckOffsets.Length;
            float minDistance = float.PositiveInfinity;
            for (int i = 0; i < arrayLength; ++i)
                if (Physics.Raycast(offsetVector + GroundCheckOffsets[i], -Vector3.up, out RaycastHit hit, rayLength, groundLayers, QueryTriggerInteraction.Ignore))
                {
                    if (hit.distance < minDistance) minDistance = hit.distance;
                }
            return minDistance;
        }
        #endregion methods
    }
}