using UnityEngine;
using EditorCustom.Attributes;
using Data.Stored;
using Game.Player;
using Game.Rooms;

namespace Universal.UI
{
    public class ButtonActions : MonoBehaviour
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public void StartNewGame()
        {
            SavingUtils.ResetTotalProgress();
            SceneLoader.Instance.LoadScene("Game");
        }

        [SerializedMethod]
        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        [SerializedMethod]
        public void CloseApplication()
        {
            Application.Quit();
        }

        [SerializedMethod]
        public void LoadScene(string sceneName) => SceneLoader.Instance.LoadScene(sceneName);

        [SerializedMethod]
        public void DoPlayerDeath()
        {
            StatsBehaviour.Stats.Health.SetToMin();
        }

        [SerializedMethod]
        public void StopPlayerInput()
        {
            InstancesProvider.Instance.PlayerInput.StopInput();
        }
        [SerializedMethod]
        public void StopPlayerCollision()
        {
            InstancesProvider.Instance.PlayerAttack.CharacterController.enabled = false;
        }
        [SerializedMethod]
        public void SaveImmediate()
        {
            SavingUtils.SaveGameData();
        }
        #endregion methods
    }
}