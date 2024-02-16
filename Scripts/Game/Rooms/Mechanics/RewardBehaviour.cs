using DebugStuff;
using EditorCustom.Attributes;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms.Mechanics
{
    public class RewardBehaviour : MonoBehaviour
    {
        #region fields & properties
        public Reward Reward => reward;
        [SerializeField] private Reward reward;
        #endregion fields & properties

        #region methods
        public void AddReward() => reward.AddReward();
        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private bool ___testBool;
        [Button(nameof(AddRewardTest))]
        private void AddRewardTest()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            AddReward();
        }
#endif //UNITY_EDITOR
    }
}