using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugStuff
{
    public class DrawBounds : MonoBehaviour
    {
#if UNITY_EDITOR
        #region fields & properties
        private Renderer Render => useThisRenderer ? GetComponent<Renderer>() : render;
        [SerializeField] private bool useThisRenderer = true;
        [SerializeField][DrawIf(nameof(useThisRenderer), false)] private Renderer render;
        [SerializeField] private Color color = Color.blue;
        [SerializeField] private bool drawAlways = true;
        #endregion fields & properties

        #region methods
        private void OnDrawGizmos()
        {
            if (!drawAlways) return;
            Draw();
        }
        public void OnDrawGizmosSelected()
        {
            if (drawAlways) return;
            Draw();
        }
        private void Draw()
        {
            if (Render == null) return;
            var bounds = Render.bounds;
            Gizmos.color = color;
            Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
        }
        #endregion methods
#endif //UNITY_EDITOR
    }
}