using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.CameraView
{
    [RequireComponent(typeof(CameraController))]
    public class CameraCollision : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Transform cameraCenter;
        [SerializeField] private Transform cameraObject;
        [SerializeField] private Vector3 positionOffset = Vector3.zero;
        [SerializeField][Range(0.01f, 100f)] private float rayScale = 0.01f;
        #endregion fields & properties

        #region methods
        public void DetectCollision()
        {
            Vector3 rayStart = cameraCenter.position + positionOffset;
            Vector3 rayEnd = cameraObject.position - cameraObject.forward * rayScale;
            RaycastHit[] hits = Physics.RaycastAll(rayStart, rayEnd - rayStart, Vector3.Distance(rayStart, rayEnd));
            foreach(var hit in hits)
            {
                if (hit.collider.isTrigger) return;
                if (hit.collider.CompareTag("Player")) return;
                cameraObject.position = hit.point;
                cameraObject.localPosition += transform.forward * rayScale;
                return;
            }
        }
        #endregion methods
    }
}