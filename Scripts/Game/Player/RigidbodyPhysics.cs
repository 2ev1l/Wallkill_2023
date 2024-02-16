using UnityEngine;

namespace Game.Player
{
    public class RigidbodyPhysics : MonoBehaviour
    {
        #region fields & properties
        private float GravityConstant => Physics.gravity.y * GravityScale;
        [SerializeField] private Rigidbody rigidBody;

        public float GravityScale
        {
            get => gravityScale;
            set => gravityScale = value;
        }
        [SerializeField] private float gravityScale = 1f;
        #endregion fields & properties

        #region methods
        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            rigidBody.velocity += deltaTime * GravityConstant * Vector3.up;
        }
        #endregion methods
    }
}