using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugStuff
{
    public class DrawGizmo : MonoBehaviour
    {
#if UNITY_EDITOR
        #region fields & properties
        [SerializeField] private GizmoType gizmoType = GizmoType.WireSphere;
        [SerializeField][DrawIf(nameof(gizmoType), GizmoType.WireSphere)][Min(0)] private float radius = 1f;
        [SerializeField][DrawIf(nameof(gizmoType), GizmoType.WireCube)] private Vector3 scale;
        [SerializeField][DrawIf(nameof(gizmoType), GizmoType.WireMesh)] private Mesh mesh;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private bool drawAlways = true;
        [SerializeField] private bool useThisPosition = true;
        [SerializeField] private bool useThisRotation = true;
        [SerializeField][DrawIf(nameof(gizmoType), GizmoType.WireMesh)] private bool useThisScale = true;

        private Transform ReferencePositionTransform => useThisPosition ? transform : referencePositionTransform;
        [SerializeField][DrawIf(nameof(useThisPosition), false)] private Transform referencePositionTransform;
        private Transform ReferenceRotationTransform => useThisRotation ? transform : referenceRotationTransform;
        [SerializeField][DrawIf(nameof(useThisRotation), false)] private Transform referenceRotationTransform;
        private Transform ReferenceScaleTransform => useThisScale ? transform : referenceScaleTransform;
        [SerializeField][DrawIf(nameof(useThisScale), false)][DrawIf(nameof(gizmoType), GizmoType.WireMesh)] private Transform referenceScaleTransform;
        #endregion fields & properties

        #region methods
        private void DrawWireSphere()
        {
            Gizmos.DrawWireSphere(ReferencePositionTransform.position, radius);
        }
        private void DrawWireCube()
        {
            Gizmos.DrawWireCube(ReferencePositionTransform.position, scale);
        }
        private void DrawWireMesh()
        {
            Gizmos.DrawWireMesh(mesh, ReferencePositionTransform.position, ReferenceRotationTransform.rotation, ReferenceScaleTransform.lossyScale);
        }
        private void OnDrawGizmos()
        {
            if (!drawAlways) return;
            DrawChoosedGizmo();
        }
        private void OnDrawGizmosSelected()
        {
            if (drawAlways) return;
            DrawChoosedGizmo();
        }
        private void DrawChoosedGizmo()
        {
            if (!enabled) return;
            Gizmos.color = color;
            try
            {
                switch (gizmoType)
                {
                    case GizmoType.WireSphere: DrawWireSphere(); break;
                    case GizmoType.WireCube: DrawWireCube(); break;
                    case GizmoType.WireMesh: DrawWireMesh(); break;
                }
            }
            catch{ }
        }
        #endregion methods
        private enum GizmoType
        {
            WireSphere,
            WireCube,
            WireMesh
        }
#endif //UNITY_EDITOR
    }
}