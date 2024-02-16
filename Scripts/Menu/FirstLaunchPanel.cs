using Data.Settings;
using System.Collections;
using UnityEngine;
using Universal.UI;
using EditorCustom.Attributes;

namespace Menu
{
    public class FirstLaunchPanel : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private DefaultStateMachine stateMachine;
        [SerializeField] private StateChange firstLaunchState;
        [SerializeField] private StateChange defaultState;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            SetPanel();
            if (!SettingsData.Data.IsFirstLaunch) return;
            SettingsData.Data.IsFirstLaunch = false;
        }
        private void SetPanel()
        {
            bool isFirstLaunch = SettingsData.Data.IsFirstLaunch;
            stateMachine.ApplyState(isFirstLaunch ? firstLaunchState : defaultState);
        }

        [SerializedMethod]
        public void BackToNormalState()
        {
            float speed = 2f;
            StartCoroutine(BackToNormalStateAnimate(speed));
            StartCoroutine(ScreenFade.DoCycle(speed));
        }
        private IEnumerator BackToNormalStateAnimate(float speed)
        {
            yield return new WaitForSeconds(1f / speed);
            stateMachine.ApplyDefaultState();
        }
        #endregion methods
    }
}