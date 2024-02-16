using Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Universal.UI;

namespace Menu
{
    public class CutSceneInit : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private ObjectMove textMove;
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            textMove.MoveTo(0);
            Invoke(nameof(LoadMenu), textMove.MoveTime + 2);
        }
        private void LoadMenu()
        {
            SavingUtils.SaveGameData();
            SceneLoader.Instance.LoadScene("Menu");
        }
        #endregion methods
    }
}