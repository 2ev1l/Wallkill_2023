using UnityEngine;
using Data;
using System.Collections.Generic;
using Data.Interfaces;
using System.Linq;
using Universal.UI;
using UnityEngine.Rendering;
using Steamworks;

namespace Universal
{
    [ExecuteAlways]
    public class SingleGameInstance : MonoBehaviour
    {
        #region fields
        public static SingleGameInstance Instance => instance;
        private static SingleGameInstance instance;
        private static bool isInitialized = false;
        private bool isMain = false;

        [SerializeField] private SavingUtils savingUtils;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private List<Component> iInitializable;
        [SerializeField] private List<Component> iStartInitializable;
        #endregion fields

        #region methods
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif //UNITY_EDITOR

            SavingUtils.OnDataReset += InitAfterReset;
            SceneLoader.OnSceneLoaded += Awake;
        }
        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif //UNITY_EDITOR

            SavingUtils.OnDataReset -= InitAfterReset;
            SceneLoader.OnSceneLoaded -= Awake;
        }
        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (automaticallyUpdateEditor)
                    CheckInterfaces();
                return;
            }
#endif //UNITY_EDITOR

            if (!isInitialized)
            {
                isMain = true;
                isInitialized = true;
                instance = this;
                OnInitialize();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (!isMain)
                {
                    DestroyImmediate(gameObject);
                    return;
                }
                OnLoad();
            }
        }
        private void OnInitialize()
        {
            savingUtils.Init();
            InitAfterReset();
        }

        private void OnLoad()
        {
            sceneLoader.Start();
            savingUtils.Start();
            foreach (var el in iStartInitializable.Cast<IStartInitializable>()) el.Start();
        }
        public void InitAfterReset()
        {
            //init before
            sceneLoader.Init();
            int counter = 0;
            foreach (var el in iInitializable.Cast<IInitializable>())
            {
                el.Init();
                counter++;
            }

            //change state next
            counter = 0;
            foreach (var el in iInitializable.Cast<IInitializable>())
            {
                ChangeObjectState(iInitializable[counter]);
                counter++;
            }
            ChangeObjectState(sceneLoader);
        }
        private void ChangeObjectState(Component component)
        {
            GameObject Object = component.gameObject;
            if (!Object.activeSelf) return;
            Object.SetActive(false);
            Object.SetActive(true);
        }
        #endregion methods

#if UNITY_EDITOR
        [SerializeField] private bool automaticallyUpdateEditor = true;
        [ContextMenu("Check Interfaces")]
        private void CheckInterfaces()
        {
            static void CastInterfacesList<T>(List<Component> list, string errorMessage)
            {
                int counter = 0;
                try
                {
                    foreach (var el in list.Cast<T>())
                    {
                        counter++;
                    }
                }
                catch { Debug.LogError($"{list[counter].name} {errorMessage}", list[counter]); }
            }

            CastInterfacesList<IStartInitializable>(iStartInitializable, "doesn't contain IStartInitializable Interface");
            CastInterfacesList<IInitializable>(iInitializable, "doesn't contain IInitializable Interface");
        }
#endif //UNITY_EDITOR
    }
}