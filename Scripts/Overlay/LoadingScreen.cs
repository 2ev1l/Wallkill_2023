using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Overlay
{
    public class LoadingScreen : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private ProgressBar progressBar;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            SceneLoader.OnSceneLoading += SetProgress;
        }
        private void OnDisable()
        {
            SceneLoader.OnSceneLoading -= SetProgress;
        }
        private void SetProgress(float value)
        {
            progressBar.Value = value;
        }
        #endregion methods
    }
}