using Data.Stored;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Rooms.Mechanics
{
    public class ShowPlayerId : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private TextMeshProUGUI text;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            GameData.Data.StatisticData.OnDeathCountChanged += ShowID;
            ShowID();
        }
        private void OnDisable()
        {
            GameData.Data.StatisticData.OnDeathCountChanged -= ShowID;
        }
        private void ShowID(int _) => ShowID();
        private void ShowID()
        {
            text.text = GameData.Data.StatisticData.PlayerId;
        }
        #endregion methods
    }
}