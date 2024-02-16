using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Stored
{
    [System.Serializable]
    public class Modifier
    {
        #region fields & properties
        public UnityAction OnRankIncreased;
        public int Id => id;
        [SerializeField][Min(0)] private int id = 0;
        public int Rank => rank;
        [SerializeField][Min(0)] private int rank = 0;

        #endregion fields & properties

        #region methods
        /// <summary>
        /// Probably you need <see cref="PlayerData.TryIncreaseModifierRank(int)"/> for better UI
        /// </summary>
        /// <returns></returns>
        public bool TryIncreaseRank()
        {
            if (!CanIncreaseRank()) return false;
            rank++;
            OnRankIncreased?.Invoke();
            return true;
        }
        public int GetMaxRank() => id switch
        {
            0 => 2,
            1 => 2,
            2 => 1,
            3 => 1,
            4 => 0,
            5 => 0,
            6 => 0,
            _ => DebugUnknownModifier()
        };
        private int DebugUnknownModifier()
        {
            Debug.LogError($"Unknown modifier #{id} at Modifier.cs");
            return 0;
        }
        public bool CanIncreaseRank() => rank < GetMaxRank();
        public Modifier() { }
        public Modifier(int id, int rank)
        {
            this.id = id;
            this.rank = rank;
        }
        #endregion methods
    }
}