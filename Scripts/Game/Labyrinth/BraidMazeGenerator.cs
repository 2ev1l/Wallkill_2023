using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universal;

namespace Game.Labyrinth
{
    [System.Serializable]
    public class BraidMazeGenerator : MazeGenerator
    {
        #region fields & properties
        /// <summary>
        /// Provides original array
        /// </summary>
        public Cell[,] Maze => maze;
        [SerializeField] private Cell[,] maze;
        /// <summary>
        /// don't use it in case of complexity of code that will need for properly instantiating objects
        /// </summary>
        //[SerializeField][Range(0, 100)] private int chanceToCarve = 5; 
        private static readonly int minSize = 5;
        private static readonly int maxSize = 101;
        #endregion fields & properties

        #region methods
        public override void GenerateMaze()
        {
            do { GenerateMazeUnprotected(); }
            while (!maze[exitCell.x, exitCell.y].AvailableDirections.Any());
        }
        private void GenerateMazeUnprotected()
        {
            CheckSize();
            ResetMaze();
            RandomizeEntranceAndExitCells();
            GenerateMazeRecursive(entranceCell.x, entranceCell.y);
            UpdateCells();
        }
        private void CheckSize()
        {
            if (size.x % 2 == 0 || size.y % 2 == 0)
                throw new System.ArgumentException($"Size must be an 'odd' number");
            if (size.x < minSize || size.y < minSize)
                throw new System.ArgumentException($"Min size of the labyrinth must be >= {minSize}");
            if (size.x > maxSize || size.y > maxSize)
                throw new System.ArgumentException($"Max size of the labyrinth must be <= {maxSize}");
        }
        private void ResetMaze()
        {
            maze = new Cell[size.x, size.y];
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    maze[x, y] = new Cell(x, y);
        }
        private void RandomizeEntranceAndExitCells()
        {
            int scenario = Random.Range(0, 3);
            entranceCell = scenario switch
            {
                0 => new(0, Random.Range(1, size.y / 2)),
                1 => new(Random.Range(1, size.x - 1), 0),
                2 => new(size.x - 1, Random.Range(1, size.y / 2)),
                _ => throw new System.NotImplementedException("Braid maze generator scenario"),
            };
            if (entranceCell.y % 2 == 0)
                ++entranceCell.y;

            exitCell = new(size.x - Random.Range(2, size.x / 2), size.y - 1);
            if (exitCell.x % 2 == 0)
                ++exitCell.x;

            maze[entranceCell.x, entranceCell.y].IsRoom = true;
            maze[exitCell.x, exitCell.y].IsRoom = true;
        }
        private void GenerateMazeRecursive(int entranceX, int entranceY)
        {
            Direction[] directions = { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
            Shuffle(directions);

            foreach (Direction direction in directions)
            {
                int dx = 0, dy = 0;

                switch (direction)
                {
                    case Direction.Up: dy = -1; break;
                    case Direction.Down: dy = 1; break;
                    case Direction.Left: dx = -1; break;
                    case Direction.Right: dx = 1; break;
                    default: continue;
                }

                int nextX = entranceX + (dx * 2);
                int nextY = entranceY + (dy * 2);

                bool isOnBoard = nextX >= 1 && nextX < size.x - 1 && nextY >= 1 && nextY < size.y - 1;
                if (!isOnBoard) continue;

                if (!maze[nextX, nextY].IsRoom)
                {
                    maze[nextX, nextY].IsRoom = true;
                    maze[entranceX + dx, entranceY + dy].IsRoom = true;

                    GenerateMazeRecursive(nextX, nextY);
                }
                else
                {
                    /*if (CustomMath.GetRandomChance(chanceToCarve))
                    {
                        maze[entranceX + dx, entranceY + dy].IsRoom = true;
                        //GenerateMazeRecursive(nextX, nextY); //causes overstack with high chance to carve
                    }*/
                }
            }
        }

        private void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }
        private void UpdateCells()
        {
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    UpdateCellDirections(x, y);
        }
        private void UpdateCellDirections(int x, int y)
        {
            Cell currentCell = maze[x, y];
            if (y - 1 >= 0 && maze[x, y - 1].IsRoom) currentCell.AddAvailableDirection(Direction.Up);
            if (y + 1 < size.y && maze[x, y + 1].IsRoom) currentCell.AddAvailableDirection(Direction.Down);

            if (x + 1 < size.x && maze[x + 1, y].IsRoom) currentCell.AddAvailableDirection(Direction.Right);
            if (x - 1 >= 0 && maze[x - 1, y].IsRoom) currentCell.AddAvailableDirection(Direction.Left);
        }
        #endregion methods
    }
}