using Data.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Labyrinth
{
    #region enum
    public enum Direction
    {
        Up,
        Down, 
        Left, 
        Right
    }
    #endregion enum

    public static class DirectionExtension
    {
        #region methods
        public static Direction Diagonal(this Direction dir) => dir switch
        {
            Direction.Up => Direction.Up,
            Direction.Down => Direction.Down,
            Direction.Right => Direction.Left,
            Direction.Left => Direction.Right,
            _ => throw new System.NotImplementedException(dir.ToString()),
        };
        public static Vector2Int GetEndCoordinates(this Direction dir, Vector2Int currentCoordinates) => dir switch
        {
            Direction.Up => new(currentCoordinates.x, currentCoordinates.y - 1),
            Direction.Down => new(currentCoordinates.x, currentCoordinates.y + 1),
            Direction.Right => new(currentCoordinates.x + 1, currentCoordinates.y),
            Direction.Left => new(currentCoordinates.x - 1, currentCoordinates.y),
            _=> throw new System.NotImplementedException(dir.ToString())
        };
        public static TransformDirection ToTransformDirection(this Direction dir) => dir switch
        {
            Direction.Up => TransformDirection.Forward,
            Direction.Down => TransformDirection.Backward,
            Direction.Right => TransformDirection.Right,
            Direction.Left => TransformDirection.Left,
            _ => throw new System.NotImplementedException(dir.ToString())
        };
        #endregion methods
    }
}