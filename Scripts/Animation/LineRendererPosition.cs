using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Animation
{
    public class LineRendererPosition : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LayerMask layersToTrigger = Physics.AllLayers;
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
        [SerializeField][Min(0)] private float maxDistance = 100f;
        [SerializeField][Min(0)] private int lineRendererStartId = 0;
        [SerializeField][Min(0)] private int lineRendererPositionFixId = 1;
        [SerializeField] private Vector3 minPointPosition = Vector3.zero;

        [SerializeField] private bool fixedUpdateOnRuntime = true;
        [SerializeField] private bool fixObjectsRotation = false;
        [SerializeField] private List<Transform> objectsToFix;

        [SerializeField] private bool useFixCollider = true;
        [Tooltip("Set collider as other object")]
        [SerializeField][DrawIf(nameof(useFixCollider), true)] private BoxCollider fixCollider;
        #endregion fields & properties

        #region methods
        private void FixedUpdate()
        {
            if (!fixedUpdateOnRuntime) return;
            Ray ray = GetRay();
            if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, layersToTrigger, triggerInteraction))
            {
                FixObjects(lineRenderer.transform.InverseTransformPoint(lineRenderer.GetPosition(lineRendererPositionFixId)), ray, hit);
                return;
            }
            Vector3 localPosition = lineRenderer.transform.InverseTransformPoint(hit.point).MaxClamp(minPointPosition);
            FixObjects(localPosition, ray, hit);
            lineRenderer.SetPosition(lineRendererPositionFixId, localPosition);
        }
        private void FixObjects(Vector3 localPosition, Ray ray, RaycastHit hit)
        {
            Vector3 reflection = Vector3.Reflect(ray.direction, hit.normal);
            foreach (var el in objectsToFix)
            {
                el.localPosition = localPosition;
                if (!fixObjectsRotation) continue;
                el.rotation = Quaternion.LookRotation(reflection);
            }
            FixCollider(ray);
        }

        private void FixCollider(Ray ray)
        {
            if (!useFixCollider) return;
            Vector3 pointStartLocalPosition = lineRenderer.GetPosition(lineRendererStartId);
            Vector3 pointEndLocalPosition = lineRenderer.GetPosition(lineRendererPositionFixId);
            if (ray.direction != Vector3.zero)
                fixCollider.transform.rotation = Quaternion.LookRotation(ray.direction);
            fixCollider.transform.localPosition = (pointEndLocalPosition - pointStartLocalPosition) / 2;

            Vector3 colldierSize = fixCollider.size;
            colldierSize.z = Vector3.Magnitude(pointEndLocalPosition - pointStartLocalPosition);
            fixCollider.size = colldierSize;
        }
        private Ray GetRay()
        {
            Vector3 globalPositionStart = GetGlobalPositionOfPoint(lineRendererStartId);
            Vector3 globalPositionEnd = GetGlobalPositionOfPoint(lineRendererPositionFixId);
            return new(globalPositionStart, globalPositionEnd - globalPositionStart);
        }
        private Vector3 GetGlobalPositionOfPoint(int pointId)
        {
            Vector3 localPointPosition = lineRenderer.GetPosition(pointId);
            Vector3 globalPointPosition = lineRenderer.transform.position;
            globalPointPosition += localPointPosition.x * lineRenderer.transform.right;
            globalPointPosition += localPointPosition.y * lineRenderer.transform.up;
            globalPointPosition += localPointPosition.z * lineRenderer.transform.forward;

            return globalPointPosition;
        }
        #endregion methods

#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField] private bool doDebug = true;
        [SerializeField][DrawIf(nameof(doDebug), true)] private bool debugAlways = false;
        [SerializeField][DrawIf(nameof(doDebug), true)][DrawIf(nameof(fixedUpdateOnRuntime), true)] private bool doFixedUpdate = false;
        private void OnDrawGizmos()
        {
            if (!doDebug) return;
            if (!debugAlways) return;
            DebugGizmos();
        }
        private void OnDrawGizmosSelected()
        {
            if (!doDebug) return;
            if (debugAlways) return;
            DebugGizmos();
        }
        private void DebugGizmos()
        {
            if (doFixedUpdate)
                FixedUpdate();
            try
            {
                Ray ray = GetRay();
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.blue);
                if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, layersToTrigger, triggerInteraction)) return;
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                Vector3 reflection = Vector3.Reflect(ray.direction, hit.normal);
                Debug.DrawRay(hit.point, reflection, Color.cyan);
            }
            catch { return; }
        }
#endif //UNITY_EDITOR
    }
}