using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overlay
{
    public class CanvasInitializer : MonoBehaviour, IStartInitializable
    {
        #region fields & properties
        private static Camera OverlayCamera
        {
            get
            {
                if (overlayCamera == null)
                    overlayCamera = GameObject.Find("Overlay Camera").GetComponent<Camera>();
                return overlayCamera;
            }
        }
        private static Camera overlayCamera;
        [SerializeField] private Canvas canvas;
        #endregion fields & properties

        #region methods
        public void Start()
        {
            canvas.worldCamera = OverlayCamera;
        }
        #endregion methods
    }
}