using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Universal;
using Universal.UI;
using Universal.UI.Layers;

namespace Animation
{
    public class ObjectMove : MonoBehaviour
    {
        #region fields & properties
        public UnityAction OnMoveEnd;
        public Transform MovedObject => movedObject;
        [SerializeField] private Transform movedObject;

        [SerializeField] private bool useLocalPositions = false;
        [SerializeField] private bool storePositions = false;
        [SerializeField] private bool startAtFirstPosition = false;
        [SerializeField] private bool doLoopFromEnable = false;
        [SerializeField][DrawIf(nameof(doLoopFromEnable), true)] private bool breakLoopOnEndReached = false;
        public IReadOnlyList<Transform> Positions => positions;
        [SerializeField][DrawIf(nameof(storePositions), true, DisablingType.ReadOnly)] private List<Transform> positions;
        public float MoveTime
        {
            get => moveTime;
            set => moveTime = value;
        }
        [SerializeField][Min(0)] private float moveTime = 1f;
        [SerializeField] private VectorLayer moveLayer;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (doLoopFromEnable)
                StartCoroutine(InfinityLoop());
        }

        private IEnumerator InfinityLoop()
        {
            ValueTimeChangerFixed vtc = null;
            int positionId = 0;
            if (startAtFirstPosition)
            {
                positionId = 1;
                if (useLocalPositions)
                    MoveObjectGlobal(positions[0].position);
                else
                    MoveObjectLocal(positions[0].localPosition);
            }

            while (true)
            {
                if (BreakCondition()) yield break;
                MoveTo(positionId);
                vtc = new(0, 1, moveTime, moveLayer.VTC.Curve, null, null, BreakCondition, false);
                yield return vtc.WaitUntilEnd();
                positionId++;
                if (positionId == positions.Count && breakLoopOnEndReached) yield break;
                positionId %= positions.Count;
            }
        }
        private bool BreakCondition()
        {
            if (this == null) return true;
            if (!enabled) return true;
            return false;
        }
        public void MoveCycle()
        {
            StartCoroutine(WaitForMoveCycle());
        }
        public IEnumerator WaitForMoveCycle()
        {
            int positionId = 0;
            if (startAtFirstPosition)
            {
                positionId = 1;
                if (useLocalPositions)
                    MoveObjectGlobal(positions[0].position);
                else
                    MoveObjectLocal(positions[0].localPosition);
            }

            for (int i = positionId; i < positions.Count; ++i)
            {
                if (movedObject == null) yield break;
                Transform el = positions[i];
                MoveTo(el);
                yield return new WaitForSeconds(moveTime);
            }
        }
        public void MoveImmediateTo(int positionId)
        {
            Transform position = positions[positionId];
            if (useLocalPositions)
                movedObject.localPosition = position.localPosition;
            else
                movedObject.position = position.position;
        }
        public void MoveTo(int positionId) => MoveTo(positions[positionId]);
        [SerializedMethod]
        public void MoveTo(Transform position)
        {
            TryStorePosition(position);
            if (useLocalPositions)
                MoveTo(position.localPosition);
            else
                MoveTo(position.position);

        }
        public void MoveTo(Vector3 position)
        {
            if (movedObject == null) return;
            if (useLocalPositions)
                moveLayer.ChangeVector(movedObject.localPosition, position, moveTime, MoveObjectLocal, delegate { OnMoveEnd?.Invoke(); }, delegate { return movedObject == null; }, false);
            else
                moveLayer.ChangeVector(movedObject.position, position, moveTime, MoveObjectGlobal, delegate { OnMoveEnd?.Invoke(); }, delegate { return movedObject == null; }, false);
        }
        private void MoveObjectLocal(Vector3 position)
        {
            if (movedObject == null) return;
            movedObject.localPosition = position;
        }
        private void MoveObjectGlobal(Vector3 position)
        {
            if (movedObject == null) return;
            movedObject.position = position;
        }
        private void TryStorePosition(Transform position)
        {
            if (!storePositions) return;
            if (positions.Contains(position)) return;
            positions.Add(position);
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField] private bool doDebug = true;
        [SerializeField][DrawIf(nameof(doDebug), true)] private bool debugAlways = false;
        [SerializeField][DrawIf(nameof(doDebug), true)] private Vector3 debugObjectScale = Vector3.one;
        [SerializeField][DrawIf(nameof(doDebug), true)] private Mesh debugMesh;
        private void OnDrawGizmosSelected()
        {
            if (!doDebug) return;
            if (debugAlways) return;
            DrawPositionsRay();
        }
        private void OnDrawGizmos()
        {
            if (!doDebug) return;
            if (!debugAlways) return;
            DrawPositionsRay();
        }

        private void DrawPositionsRay()
        {
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
                if (debugMesh == null)
                    Gizmos.DrawWireCube(position.position, debugObjectScale);
                else
                    Gizmos.DrawWireMesh(debugMesh, position.position, movedObject.rotation, movedObject.lossyScale);
            }
        }
#endif //UNITY_EDITOR
    }
}