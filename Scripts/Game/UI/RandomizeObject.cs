using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Game.UI
{
    public class RandomizeObject : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private bool choosedState = true;
        [SerializeField] private bool modifyIfDisabled = true;
        [SerializeField] private bool randomizeAtAwake = true;
        [SerializeField] private bool randomizeAtStart = false;

        [SerializeField] private bool randomizeCountRange = false;
        private int RandomizedCount => randomizeCountRange ? Random.Range(randomRange.Min(), randomRange.Max() + 1) : randomizedCount;
        [SerializeField][DrawIf(nameof(randomizeCountRange), false)][Min(1)] private int randomizedCount = 1;
        [SerializeField][DrawIf(nameof(randomizeCountRange), true)] private Vector2Int randomRange = new(0, 1);

        [SerializeField] private List<GameObject> gameObjects = new();
        [SerializeField][ReadOnly] private bool isRandomized = false;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            if (randomizeAtAwake && !isRandomized)
            {
                isRandomized = true;
                Randomize();
            }
        }
        private void Start()
        {
            if (randomizeAtStart && !isRandomized)
            {
                isRandomized = true;
                Randomize();
            }
        }
        [Button(nameof(Randomize))]
        private void Randomize()
        {
            List<int> choosedIds = GetAllRandomizedIds();

            for (int i = 0; i < gameObjects.Count; ++i)
            {
                GameObject obj = gameObjects[i];
                if (choosedIds.Contains(i))
                {
                    bool objectState = obj.activeSelf;
                    if (modifyIfDisabled || objectState)
                        obj.SetActive(obj);
                }
                else
                {
                    obj.SetActive(!choosedState);
                }
            }
        }
        private List<int> GetAllRandomizedIds()
        {
            List<int> rawIds = new();
            List<int> resultIds = new();
            for (int i = 0; i < gameObjects.Count; ++i)
                rawIds.Add(i);
            int rndCount = RandomizedCount;
            for (int i = 0; i < rndCount; ++i)
            {
                int rndIndex = Random.Range(0, rawIds.Count);
                int rndValue = rawIds[rndIndex];
                resultIds.Add(rndValue);
                rawIds.RemoveAt(rndIndex);
            }

            return resultIds;
        }
        #endregion methods

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (randomRange.x < 0 && randomizeCountRange)
            {
                Debug.LogError("Randomized count X is less than zero. Fixing.", this);
                randomRange.x = 0;
            }
            if (randomRange.y < 0 && randomizeCountRange)
            {
                Debug.LogError("Randomized count Y is less than zero. Fixing.", this);
                randomRange.y = 0;
            }

            if (randomizedCount > gameObjects.Count)
            {
                Debug.LogError("Randomized count is more than GameObjects Count. Fixing.", this);
                randomizedCount = gameObjects.Count;
            }
            if (randomRange.x > gameObjects.Count && randomizeCountRange)
            {
                Debug.LogError("Randomized count X is more than GameObjects Count. Fixing.", this);
                randomRange.x = gameObjects.Count;
            }
            if (randomRange.y > gameObjects.Count && randomizeCountRange)
            {
                Debug.LogError("Randomized count Y is more than GameObjects Count. Fixing.", this);
                randomRange.y = gameObjects.Count;
            }

            if (isRandomized && !Application.isPlaying)
            {
                Debug.LogError("IsRandomized bool must not be set to true", this);
            }
        }

        [Button(nameof(GetAllChilds))]
        private void GetAllChilds()
        {
            UnityEditor.Undo.RecordObject(this, "New childs for randomize");
            gameObjects = new();
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                GameObject child = transform.GetChild(i).gameObject;
                gameObjects.Add(child);
            }
        }
#endif //UNITY_EDITOR
    }
}