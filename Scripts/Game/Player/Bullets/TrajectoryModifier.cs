using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Bullets
{
    public class TrajectoryModifier : ModifierBehaviour
    {
        #region fields & properties
        protected override int ReferenceId => 1;
        [SerializeField] private List<GameObject> lines = new();
        #endregion fields & properties

        #region methods
        protected override void SetDefaultModifierParams()
        {
            SetModifierParams(-1);
        }
        protected override void SetModifierParams(int rank)
        {
            int activeLinesCount = rank switch
            {
                -1 => 0,
                0 => 1,
                1 => 2,
                _ => DebugUnknownModifier(0),
            };
            for (int i = 0; i < lines.Count; ++i)
            {
                lines[i].SetActive(i < activeLinesCount);
            }
        }
        #endregion methods
    }
}