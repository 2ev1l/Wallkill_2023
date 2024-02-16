using Animation;
using EditorCustom.Attributes;
using Game.Player.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Game.Rooms.Mechanics
{
    public class GravityReceiver : MonoBehaviour
    {
        #region fields & properties
        public UnityEvent OnReceivesReachedMaxEvent;

        [HelpBox("Use global positions", HelpBoxMessageType.Info)]
        [SerializeField][Required] private ObjectMove objectMove;
        [SerializeField][Required] private Collider collision;
        [SerializeField] private Vector3Int freezePosition;
        [SerializeField][Min(-1)] private int maxReceives = -1;
        [SerializeField] private bool clampLocalPosition = false;
        [SerializeField][DrawIf(nameof(clampLocalPosition), true)] private Vector3 clampedMin;
        [SerializeField][DrawIf(nameof(clampLocalPosition), true)] private Vector3 clampedMax;

        [Title("Read Only")]
        [SerializeField][ReadOnly] private int currentReceives = 0;
        #endregion fields & properties

        #region methods
        public void ReceiveGravity(GravitySphere sender)
        {
            if (maxReceives > -1 && currentReceives >= maxReceives) return;
            currentReceives++;
            Vector3 startPosition = objectMove.MovedObject.transform.position;
            Vector3 finalPosition = CalculateFinalPosition(startPosition, sender);
            objectMove.MoveTo(finalPosition);
            if (currentReceives == maxReceives) OnReceivesReachedMaxEvent?.Invoke();
        }
        private Vector3 CalculateFinalPosition(Vector3 startPosition, GravitySphere sender)
        {
            Vector3 finalPosition = sender.NegateGravity ? NegateFinalPosition(startPosition, sender) : Vector3.Lerp(startPosition, sender.transform.position, 0.65f);
            if (freezePosition.x != 0) finalPosition.x = startPosition.x;
            if (freezePosition.y != 0) finalPosition.y = startPosition.y;
            if (freezePosition.z != 0) finalPosition.z = startPosition.z;
            if (clampLocalPosition)
            {
                Vector3 localPosition = objectMove.transform.parent.InverseTransformPoint(finalPosition);
                localPosition = localPosition.Clamp(clampedMin, clampedMax);
                finalPosition = objectMove.transform.parent.TransformPoint(localPosition);
            }
            return finalPosition;
        }
        private Vector3 NegateFinalPosition(Vector3 startPosition, GravitySphere sender)
        {
            Vector3 nearestPoint = CalculateNearestPointToSphere(startPosition, sender.transform.position);
            Vector3 moveDirectionUnclamped = nearestPoint - sender.transform.position;
            Vector3 moveDirectionClamped = moveDirectionUnclamped.normalized;
            float distanceToCenter = Vector3.Distance(nearestPoint, sender.transform.position);
            float distanceToFinalPoint = sender.SphereRadius - distanceToCenter;
            Vector3 offsetPointPosition = moveDirectionClamped * distanceToFinalPoint;
            return startPosition + offsetPointPosition;
        }
        private Vector3 CalculateNearestPointToSphere(Vector3 startPosition, Vector3 sphereCenter)
        {
            Vector3 nearestPoint = collision.ClosestPoint(sphereCenter);
            float distanceToPoint = Vector3.Distance(sphereCenter, nearestPoint);
            if (distanceToPoint.Approximately(0, 0.01f))
            {
                Vector3 directionToStart = (startPosition - sphereCenter).normalized;
                nearestPoint = sphereCenter + directionToStart * 0.1f;
            }
            return nearestPoint;
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField] private bool doDebug = true;
        [SerializeField][DrawIf(nameof(doDebug), true)] private bool debugAlways = false;
        [SerializeField][DrawIf(nameof(doDebug), true)] private GravitySphere debugSphere = null;
        private void OnDrawGizmosSelected()
        {
            if (!doDebug) return;
            if (debugAlways) return;
            DebugDraw();
        }
        private void OnDrawGizmos()
        {
            if (!doDebug) return;
            if (!debugAlways) return;
            DebugDraw();
        }
        private void DebugDraw()
        {
            Vector3 startPosition = objectMove.MovedObject.transform.position;
            Vector3 finalPosition = Vector3.one;
            if (freezePosition.x != 0) finalPosition.x = 0;
            if (freezePosition.y != 0) finalPosition.y = 0;
            if (freezePosition.z != 0) finalPosition.z = 0;


            Gizmos.color = Color.red;
            Gizmos.DrawRay(startPosition, Vector3.right * finalPosition.x);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(startPosition, Vector3.up * finalPosition.y);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(startPosition, Vector3.forward * finalPosition.z);

            if (debugSphere != null)
            {
                Gizmos.DrawWireSphere(CalculateNearestPointToSphere(startPosition, debugSphere.transform.position), 0.1f);
                Gizmos.DrawWireCube(NegateFinalPosition(startPosition, debugSphere), Vector3.one / 5f);
            }
        }
#endif //UNITY_EDITOR
    }
}