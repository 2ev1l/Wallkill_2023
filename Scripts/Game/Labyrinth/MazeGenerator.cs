using EditorCustom.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Labyrinth
{
    [System.Serializable]
    public abstract class MazeGenerator
    {
        #region fields & properties
        public Vector2Int Size
        {
            get => size;
            set => size = value;
        }
        [SerializeField] protected Vector2Int size;
        public Vector2Int EntranceCell => entranceCell;
        [SerializeField][ReadOnly] protected Vector2Int entranceCell;
        public Vector2Int ExitCell => exitCell;
        [SerializeField][ReadOnly] protected Vector2Int exitCell;
        #endregion fields & properties

        #region methods
        public abstract void GenerateMaze();
        #endregion methods
    }
}