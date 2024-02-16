using Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Rooms
{
    public class WallBlockUI : MonoBehaviour
    {
        #region fields & properties
        [SerializeField] private WallBlock wallBlock;
        [SerializeField] private AnimationRandomizer animationRandomizer;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            wallBlock.Room.OnCompleted += DisableAnimation;
            wallBlock.Room.OnStart += StartAnimation;
        }
        private void OnDisable()
        {
            wallBlock.Room.OnCompleted -= DisableAnimation;
            wallBlock.Room.OnStart -= StartAnimation;
        }
        private void DisableAnimation() => animationRandomizer.ResetAnimatorSmoothly();
        private void StartAnimation() => animationRandomizer.StartAnimationRandomly();
        #endregion methods
    }
}