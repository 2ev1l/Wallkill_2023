using Game.Labyrinth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Enums
{
    #region enum
    public enum TransformDirection
    {
        Forward, Backward, Right, Left
    }
    #endregion enum

    public static class TransformDirectionExtension
    {
        #region methods
        public static Direction ToDirection(this TransformDirection dir) => dir switch
        {
            TransformDirection.Forward => Direction.Up,
            TransformDirection.Backward => Direction.Down,
            TransformDirection.Right => Direction.Right,
            TransformDirection.Left => Direction.Left,
            _ => throw new System.NotImplementedException(dir.ToString())
        };
        public static TransformDirection Diagonal(this TransformDirection td) => td switch
        {
            TransformDirection.Forward => TransformDirection.Backward,
            TransformDirection.Backward => TransformDirection.Forward,
            TransformDirection.Right => TransformDirection.Left,
            TransformDirection.Left => TransformDirection.Right,
            _ => throw new System.NotImplementedException(td.ToString()),
        };
        #endregion methods
    }
}