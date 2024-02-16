using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Labyrinth
{
    [System.Serializable]
    public class RoomSpawnInfo
    {
        #region fields & properties
        public Room Prefab
        {
            get => prefab;
            set => prefab = value;
        }
        [SerializeField] private Room prefab;
        public Room Instantiated
        {
            get => instantiated;
            set => instantiated = value;
        }
        [SerializeField] private Room instantiated;
        public Vector3 SpawnWorldPosition
        {
            get => spawnWorldPosition;
            set => spawnWorldPosition = value;
        }
        [SerializeField] private Vector3 spawnWorldPosition;

        public Cell RelatedCell
        {
            get => relatedCell;
            set => relatedCell = value;
        }
        [SerializeField] private Cell relatedCell;

        /*public bool CanInstantiate
        {
            get => canInstantiate;
            set => canInstantiate = value;
        }
        [SerializeField] private bool canInstantiate = true;*/
        #endregion fields & properties

        #region methods
        public RoomSpawnInfo(Room prefab, Vector3 spawnWorldPosition, Cell relatedCell)
        {
            this.prefab = prefab;
            this.spawnWorldPosition = spawnWorldPosition;
            this.relatedCell = relatedCell;
        }
        #endregion methods
    }
}