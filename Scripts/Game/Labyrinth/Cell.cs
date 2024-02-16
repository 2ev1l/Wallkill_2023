using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Labyrinth
{
    [System.Serializable]
    public class Cell
    {
        #region fields & properties
        public Vector2Int Position => position;
        [SerializeField] private Vector2Int position;
        public bool IsRoom 
        { 
            get => isRoom;
            set => isRoom = value;
        }
        [SerializeField] private bool isRoom = false;
        public IReadOnlyList<Direction> AvailableDirections => availableDirections;
        [SerializeField] private List<Direction> availableDirections = new();
        #endregion fields & properties

        #region methods
        public void AddAvailableDirection(Direction direction)
        {
            availableDirections.Add(direction);
        }
        #endregion methods

        public Cell(int x, int y)
        {
            this.position = new(x, y);
            isRoom = false;
            availableDirections = new();
        }
        public Cell(Vector2Int position)
        {
            this.position = position;
            isRoom = false;
            availableDirections = new();
        }
    }
}