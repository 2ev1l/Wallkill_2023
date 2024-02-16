using DebugStuff;
using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI.Audio;
using Universal.UI.Layers;

namespace Game.Player.Bullets
{
    public class PortalProvider : MonoBehaviour
    {
        #region fields & properties
        public PortalProvider Exit => exit;
        [SerializeField] private PortalProvider exit;
        [SerializeField] private Collider collision;

        [Title("UI")]
        [SerializeField] private Renderer render;
        [SerializeField] private string emissionShaderPropertyName = "_EmissionColor";
        [SerializeField][ColorUsage(true, true)] private Color defaultEmissionColor;
        [SerializeField][ColorUsage(true, true)] private Color burstEmissionColor;
        [SerializeField][Min(0)] private float secondsToChange = 1f;
        [SerializeField] private ColorLayer colorLayer;
        private bool isColorBursting = false;

        [SerializeField] private AudioClipData audioClip;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            collision.enabled = true;
            SetColor(defaultEmissionColor);
        }
        private void OnDisable()
        {

        }
        public void EnterPortal(Transform receiver)
        {
            audioClip.Play();
            exit.collision.enabled = false;
            Vector3 closestPointGlobal = collision.ClosestPoint(receiver.position);
            Vector3 finalPosition = closestPointGlobal + (exit.transform.position - transform.position);
            receiver.position = finalPosition;
            Invoke(nameof(SetExitCollision), Time.fixedDeltaTime * 2);
            BurstColor();
            exit.BurstColor();
        }
        private void BurstColor()
        {
            isColorBursting = true;
            colorLayer.ChangeColor(GetCurrentColor(), burstEmissionColor, secondsToChange, x => SetColor(x), delegate { ResetColor(); }, delegate { return !gameObject.activeSelf; });
        }
        private void ResetColor()
        {
            isColorBursting = false;
            colorLayer.ChangeColor(GetCurrentColor(), defaultEmissionColor, secondsToChange, x => SetColor(x), null, delegate { return !gameObject.activeSelf || isColorBursting; });
        }
        private Color GetCurrentColor() => render.material.GetColor(emissionShaderPropertyName);
        private void SetColor(Color color)
        {
            render.material.SetColor(emissionShaderPropertyName, color);
        }
        private void SetExitCollision() => exit.collision.enabled = true;
        #endregion methods


#if UNITY_EDITOR
        [Title("Debug")]
        [SerializeField] private bool doDebug = true;
        [SerializeField][DrawIf(nameof(doDebug), true)] private bool debugAlways = false;
        [SerializeField][DrawIf(nameof(doDebug), true)][Min(0)] private float debugLineScale = 1f;

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
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.forward * debugLineScale);
        }

#endif //UNITY_EDITOR
    }
}