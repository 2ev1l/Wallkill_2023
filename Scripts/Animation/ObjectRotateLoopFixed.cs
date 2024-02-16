using EditorCustom.Attributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Animation
{
    /// <summary>
    /// Use this to animate with physics
    /// </summary>
    public class ObjectRotateLoopFixed : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Transform rotatedObject;
        [SerializeField] private bool useLocalRotation = false;
        [SerializeField] private bool startFromFirstRotation = true;
        public IReadOnlyList<Transform> Rotations => rotations;
        [SerializeField] private List<Transform> rotations;
        public float RotateTime
        {
            get => rotateTime;
            set => rotateTime = value;
        }
        [SerializeField][Min(0)] private float rotateTime = 1f;
        [SerializeField] private AnimationCurve rotateCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private Quaternion startStoredRotation = Quaternion.identity;
        private Quaternion endStoredRotation = Quaternion.identity;
        private int currentObjectId = -1;
        private bool isRotateStart = false;
        private float waitedTime;
        #endregion fields & properties

        #region methods
        private void FixedUpdate()
        {
            if (!isRotateStart)
            {
                SetNextObjectId();
                StartRotate();
            }

            waitedTime += Time.fixedDeltaTime;
            float lerpedValue = Mathf.InverseLerp(0, rotateTime, waitedTime);

            LerpRotate(lerpedValue);

            if (lerpedValue >= 1f)
            {
                EndMove();
            }
        }
        private void LerpRotate(float lerpedValue)
        {
            if (useLocalRotation)
            {
                rotatedObject.transform.localRotation = Quaternion.Lerp(startStoredRotation, endStoredRotation, rotateCurve.Evaluate(lerpedValue));
            }
            else
            {
                rotatedObject.transform.rotation = Quaternion.Lerp(startStoredRotation, endStoredRotation, rotateCurve.Evaluate(lerpedValue));
            }
        }
        private void EndMove()
        {
            isRotateStart = false;
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
                currentObjectId %= rotations.Count;
            }
        }
        private void InitializeObjectId()
        {
            if (startFromFirstRotation)
            {
                if (useLocalRotation)
                {
                    rotatedObject.transform.localRotation = rotations[0].localRotation;
                }
                else
                {
                    rotatedObject.transform.rotation = rotations[0].rotation;
                }
                currentObjectId = 1;
            }
            else
            {
                currentObjectId = 0;
            }
        }
        private void StartRotate()
        {
            isRotateStart = true;
            waitedTime = 0f;
            if (useLocalRotation)
            {
                startStoredRotation = rotatedObject.localRotation;
                endStoredRotation = rotations[currentObjectId].localRotation;
            }
            else
            {
                startStoredRotation = rotatedObject.rotation;
                endStoredRotation = rotations[currentObjectId].rotation;
            }
        }
        #endregion methods

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            DrawRotationRay();
        }
        [Title("Debug")]
        [SerializeField] private bool doDebug = true;
        private void DrawRotationRay()
        {
            if (!doDebug) return;
            try
            {
                if (rotations.Count == 0 || rotatedObject == null) return;
                Debug.DrawRay(rotatedObject.position, rotations[0].forward, Color.white);
            }
            catch { return; }
            for (int i = 0; i < rotations.Count; ++i)
            {
                Transform rotation = rotations[i];
                Transform nextRotation = rotations[(i + 1) % rotations.Count];
                Debug.DrawRay(rotatedObject.position, nextRotation.forward, Color.Lerp(Color.blue, Color.red, (float)i / rotations.Count));
            }
        }
        [SerializeField][Label("Debug Rotation Id")][Min(0)] private int ___debugRotation = 0;
        [Button(nameof(SetToRotationId))]
        private void SetToRotationId()
        {
            if (useLocalRotation)
            {
                rotatedObject.transform.localRotation = rotations[___debugRotation].localRotation;
            }
            else
            {
                rotatedObject.transform.rotation = rotations[___debugRotation].rotation;
            }
        }
        [Button(nameof(SaveCurrentRotation))]
        private void SaveCurrentRotation()
        {
            if (useLocalRotation)
            {
                rotations[___debugRotation].localRotation = rotatedObject.transform.localRotation;
            }
            else
            {
                rotations[___debugRotation].rotation = rotatedObject.transform.rotation;
            }
        }
#endif //UNITY_EDITOR
    }
}