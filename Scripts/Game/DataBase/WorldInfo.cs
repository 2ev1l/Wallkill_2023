using Data.Stored;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.DataBase
{
    [System.Serializable]
    public class WorldInfo
    {
        #region fields & properties
        public Sprite Texture => texture;
        [SerializeField] private Sprite texture;
        public WorldType WorldType => worldType;
        [SerializeField] private WorldType worldType;
        public LanguageInfo Language => language;
        [SerializeField] private LanguageInfo language;
        public IReadOnlyList<Room> Rooms => rooms;
        [SerializeField] private List<Room> rooms = new();
        public IReadOnlyList<Room> StartRooms => startRooms;
        [SerializeField] private List<Room> startRooms = new();
        public IReadOnlyList<Room> EndRooms => endRooms;
        [SerializeField] private List<Room> endRooms = new();
        public Vector2Int MazeSize => mazeSize;
        [SerializeField] private Vector2Int mazeSize = new(11, 11);
        public WorldStats Stats => GameData.Data.WorldsData.GetWorldStats(worldType);
        #endregion fields & properties

        #region methods

        #endregion methods
    }
}