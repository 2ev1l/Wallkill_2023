using EditorCustom.Attributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Animation
{
    /// <summary>
    /// Use this to animate with physics
    /// </summary>
    public class ObjectMoveLoopFixed : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Transform movedObject;
        [SerializeField] private bool useLocalPositions = false;
        [SerializeField] private bool startFromFirstPosition = true;
        public IReadOnlyList<Transform> Positions => positions;
        [SerializeField] private List<Transform> positions;
        public float MoveTime
        {
            get => moveTime;
            set => moveTime = value;
        }
        [SerializeField][Min(0)] private float moveTime = 1f;
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private Vector3 startStoredPosition = Vector3.zero;
        private Vector3 endStoredPosition = Vector3.zero;
        private int currentObjectId = -1;
        private bool isMoveStart = false;
        private float waitedTime;
        #endregion fields & properties

        #region methods
        private void FixedUpdate()
        {
            if (!isMoveStart)
            {
                SetNextObjectId();
                StartMove();
            }

            waitedTime += Time.fixedDeltaTime;
            float lerpedValue = Mathf.InverseLerp(0, moveTime, waitedTime);

            LerpMove(lerpedValue);

            if (lerpedValue >= 1f)
            {
                EndMove();
            }
        }
        private void LerpMove(float lerpedValue)
        {
            if (useLocalPositions)
            {
                movedObject.transform.localPosition = Vector3.Lerp(startStoredPosition, endStoredPosition, moveCurve.Evaluate(lerpedValue));
            }
            else
            {
                movedObject.transform.position = Vector3.Lerp(startStoredPosition, endStoredPosition, moveCurve.Evaluate(lerpedValue));
            }
        }
        private void EndMove()
        {
            isMoveStart = false;
        }
        private void SetNextObjectId()
        {
            if (currentObjectId == -1)
            {
                InitializeObjectId();
            }
            else
            {
                currentObjectId++;
                currentObjectId %= positions.Count;
            }
        }
        private void InitializeObjectId()
        {
            if (startFromFirstPosition)
            {
                if (useLocalPositions)
                {
                    movedObject.transform.localPosition = positions[0].localPosition;
                }
                else
                {
                    movedObject.transform.position = positions[0].position;
                }
                currentObjectId = 1;
            }
            else
            {
                currentObjectId = 0;
            }
        }
        private void StartMove()
        {
            isMoveStart = true;
            waitedTime = 0f;
            if (useLocalPositions)
            {
                startStoredPosition = movedObject.localPosition;
                endStoredPosition = positions[currentObjectId].localPosition;
            }
            else
            {
                startStoredPosition = movedObject.position;
                endStoredPosition = positions[currentObjectId].position;
            }
        }
        #endregion methods

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            DrawPositionsRay();
        }
        [Title("Debug")]
        [SerializeField] private bool doDebug = true;
        [SerializeField][DrawIf(nameof(doDebug), true)] private Vector3 debugObjectScale = Vector3.one;
        private void DrawPositionsRay()
        {
            if (!doDebug) return;
            try
            {
                if (positions.Count == 0 || movedObject == null) return;
                Debug.DrawLine(movedObject.position, positions[0].position, Color.white);
            }
            catch { return; }
            for (int i = 0; i < positions.Count; ++i)
            {
                Transform position = positions[i];
                Transform nextPosition = positions[(i + 1) % positions.Count];
                Debug.DrawLine(position.position, nextPosition.position, Color.Lerp(Color.blue, Color.red, (float)i / positions.Count));
                Gizmos.DrawWireCube(position.position, debugObjectScale);
            }
        }
#endif //UNITY_EDITOR
    }
}