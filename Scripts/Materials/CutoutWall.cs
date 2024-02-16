using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;
using EditorCustom.Attributes;
using System.Linq;

namespace Materials
{
    public class CutoutWall : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private Transform targetObject;
        [SerializeField] private LayerMask wallMask;
        [SerializeField] private float cutoutSize = 0.3f;
        [SerializeField] private float faloffSize = 0.2f;
        [SerializeField] private float wallLifeTime = 1f;
        [SerializeField] private float cutoutSizeMultiplier = 1f;
        private TimeLiveableList<Renderer> CurrentWalls
        {
            get
            {
                if (Mathf.Abs(currentWalls.BaseTimeLife - wallLifeTime) > 0.1f)
                {
                    currentWalls = new(wallLifeTime);
                }
                return currentWalls;
            }
        }
        [SerializeField][Header("Read Only")][ReadOnly] private TimeLiveableList<Renderer> currentWalls = new(1f);
        [SerializeField][ReadOnly] private List<Renderer> deadLiveables = new();
        [SerializeField][ReadOnly] private float calculatedCutoutSize;
        private RaycastHit[] hitObjectsCycle1 = new RaycastHit[5];
        private RaycastHit[] hitObjectsCycle2 = new RaycastHit[5];

        #endregion fields & properties

        #region methods
        public void Start()
        {
            if (targetObject == null)
                targetObject = GameObject.FindGameObjectWithTag("Player").transform;
        }
        private void OnEnable()
        {
            CurrentWalls.OnLiveableDead += ClearWall;
            CurrentWalls.OnDeadAlive += OnLiveableAlive;
        }
        private void OnDisable()
        {
            CurrentWalls.OnLiveableDead -= ClearWall;
            CurrentWalls.OnDeadAlive -= OnLiveableAlive;
        }
        private void Update()
        {
            SetWallsCutout();
            CurrentWalls.DecreaseListTime(Time.deltaTime);
        }
        private void SetWallsCutout()
        {
            Camera camera = Camera.main;
            if (targetObject == null || camera == null) return;
            Vector2 cutoutPos = camera.WorldToViewportPoint(targetObject.position);
            cutoutPos.y /= (Screen.width / Screen.height);
            Vector3 direction = targetObject.position - camera.transform.position;
            float maxDistance = Vector3.Distance(targetObject.position, camera.transform.position);
            SetRaycastCutout(camera, direction, maxDistance, cutoutPos, hitObjectsCycle1, null);
            SetRaycastCutout(camera, targetObject.position - camera.transform.position + Vector3.up, maxDistance, cutoutPos, hitObjectsCycle2, hitObjectsCycle1);
        }
        private void SetRaycastCutout(Camera camera, Vector3 direction, float maxDistance, Vector2 cutoutPos, RaycastHit[] hitObjects, params RaycastHit[] excludedHitObjects)
        {
            int hits = Physics.RaycastNonAlloc(camera.transform.position, direction, hitObjects, maxDistance, wallMask);
            Debug.DrawRay(camera.transform.position, direction);
            calculatedCutoutSize = cutoutSize / (maxDistance / 8) * cutoutSizeMultiplier;
            for (int i = 0; i < hits; ++i)
            {
                RaycastHit hit = hitObjects[i];
                bool canAdd = true;
                if (excludedHitObjects != null)
                {
                    for (int j = 0; j < excludedHitObjects.Length; ++j)
                    {
                        if (excludedHitObjects[j].collider != hit.collider) continue;
                        canAdd = false;
                        break;
                    }
                }

                if (!canAdd) continue;
                if (!hit.transform.TryGetComponent(out Renderer render)) continue;
                CurrentWalls.StackObject(render, Time.deltaTime);

                SetCutout(render, cutoutPos);
            }
        }
        private void SetCutout(Renderer render, Vector2 cutoutPos)
        {
            for (int j = 0; j < render.materials.Length; ++j)
            {
                render.materials[j].SetVector("_Cutout_Position", cutoutPos);
                render.materials[j].SetFloat("_Cutout_Size", calculatedCutoutSize);
                render.materials[j].SetFloat("_Falloff_Size", faloffSize);
            }
        }
        private void OnLiveableAlive(TimeLiveable<Renderer> tl)
        {
            if (!deadLiveables.Contains(tl.Object)) return;
            deadLiveables.Remove(tl.Object);
        }
        private void ClearWall(TimeLiveable<Renderer> tl)
        {
            StartCoroutine(WallClear(tl));
        }
        private IEnumerator WallClear(TimeLiveable<Renderer> tl)
        {
            deadLiveables.Add(tl.Object);
            ValueTimeChanger vtc = new(faloffSize, 1, 3f + wallLifeTime);
            Material[] mats = tl.Object.materials;
            while (!vtc.IsEnded)
            {
                yield return CustomMath.WaitAFrame();
                if (!deadLiveables.Contains(tl.Object)) yield break;
                for (int i = 0; i < mats.Length; ++i)
                {
                    mats[i].SetFloat("_Falloff_Size", vtc.Value);
                }
            }
            for (int i = 0; i < mats.Length; ++i)
            {
                mats[i].SetFloat("_Cutout_Size", 0);
            }
            if (!deadLiveables.Contains(tl.Object)) yield break;
            deadLiveables.Remove(tl.Object);
        }
        #endregion methods
    }
}
