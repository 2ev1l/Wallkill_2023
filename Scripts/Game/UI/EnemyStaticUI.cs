using Data.Stored;
using DebugStuff;
using EditorCustom.Attributes;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Universal;

namespace Game.UI
{
    public class EnemyStaticUI : SingleSceneInstance<EnemyStaticUI>
    {
        #region fields & properties
        [SerializeField] private GameObject UI;
        [SerializeField] private StatBar statBar;
        [SerializeField] private TextMeshProUGUI nameText;
        #endregion fields & properties

        #region methods
        public void EnableUI(EnemyUI reference)
        {
            UI.SetActive(true);
            statBar.Stat = reference.Enemy.Health;
            nameText.text = reference.EnemyName.Text;
        }
        public void DisableUI()
        {
            UI.SetActive(false);
        }
        #endregion methods
    }
}