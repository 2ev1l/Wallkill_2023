using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal.UI
{
    public class ScreenFade : MonoBehaviour, IStartInitializable, IInitializable
    {
        #region fields & properties
        public static ScreenFade Instance { get; private set; }
        public static UnityAction<bool> OnBlackScreenFading;
        public static UnityAction OnBlackScreenFadeUp;
        public static UnityAction OnBlackScreenFadeDown;
        [SerializeField] private CanvasGroup fadeCanvas;
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;
        }
        public void Start()
        {
            Fade(false);
        }
        private void OnEnable()
        {
            SavingUtils.OnDataReset += DisableLoadingCanvas;
            SceneLoader.OnStartLoading += OnSceneLoading;
        }
        private void OnDisable()
        {
            SavingUtils.OnDataReset -= DisableLoadingCanvas;
            SceneLoader.OnStartLoading -= OnSceneLoading;
        }
        private void DisableLoadingCanvas() => Instance.fadeCanvas.alpha = 0;
        private void OnSceneLoading(float offsetTime)
        {
            Fade(true, 1f / offsetTime);
        }
        public static IEnumerator DoCycle(float animationSpeed = 1f)
        {
            Fade(true, animationSpeed);
            yield return new WaitForSeconds(1f / animationSpeed);
            Fade(false, animationSpeed);
            yield return new WaitForSeconds(1f / animationSpeed);
        }
        public static void Fade(bool fadeUp, float animationSpeed = 1f)
        {
            Instance.fadeCanvas.gameObject.SetActive(true);
            Instance.fadeCanvas.alpha = fadeUp ? 0 : 1;
            Instance.fadeCanvas.blocksRaycasts = fadeUp;
            int finalValue = fadeUp ? 1 : 0;
            Instance.StartCoroutine(BlackScreenAlphaChange(finalValue, 0.9f / animationSpeed));
            if (fadeUp) OnBlackScreenFadeUp?.Invoke();
            else OnBlackScreenFadeDown?.Invoke();
        }
        private static IEnumerator BlackScreenAlphaChange(int finalValue, float time)
        {
            ValueTimeChanger vsc = new((finalValue + 1) % 2, finalValue, time);
            bool up = (finalValue + 1) % 2 == 0;
            while (!vsc.IsEnded)
            {
                yield return CustomMath.WaitAFrame();
                OnBlackScreenFading?.Invoke(up);
                Instance.fadeCanvas.alpha = vsc.Value;
            }
        }
        public static void BlackScreenFadeZero()
        {
            Instance.fadeCanvas.gameObject.SetActive(false);
        }
		public static bool IsBlackScreenFade() => Instance.fadeCanvas.alpha > 0f && Instance.fadeCanvas.alpha < 1f;

        #endregion methods
    }
}