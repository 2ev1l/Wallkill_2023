using EditorCustom.Attributes;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Game.Rooms
{
    public class SceneReloader : SingleSceneInstance<SceneReloader>
    {
        #region fields & properties
        [SerializeField] private string sceneToReload = "Game";
        [SerializeField] private bool reloadOnPlayerDead = true;
        [SerializeField][DrawIf(nameof(reloadOnPlayerDead), true)][Min(0)] private float reloadTimeOnPlayerDead = 3f;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            StatsBehaviour.Stats.Health.OnValueReachedMinimum += OnPlayerDead;
        }
        private void OnDisable()
        {
            StatsBehaviour.Stats.Health.OnValueReachedMinimum -= OnPlayerDead;
        }
        private void OnPlayerDead()
        {
            if (!reloadOnPlayerDead) return;
            Invoke(nameof(ReloadScene), reloadTimeOnPlayerDead);
        }
        public void ReloadScene() => SceneLoader.Instance.LoadScene(sceneToReload, true);
        #endregion methods
    }
}