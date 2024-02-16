using Data.Enums;
using Data.Stored;
using DebugStuff;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Rooms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Universal;

namespace Game.Labyrinth
{
    public class MazeCreator : SingleSceneInstance<MazeCreator>
    {
        #region fields & properties
        public UnityAction OnCreateEnd;
        [SerializeField] private Transform buildStart;

        [Title("Read Only")]
        [Tooltip("Braid Maze parameters takes from World Info")]
        [SerializeField][ReadOnly] private BraidMazeGenerator braidMaze;
        [SerializeField][ReadOnly] private List<RoomSpawnInfo> generatedRoomsInfo = new();
        public List<int> GeneratedRoomsId => generatedRoomsInfo.Select(x => x.Prefab.Id).ToList();
        [SerializeField][ReadOnly] private OpenedItemsStored<CountId> generatedRoomsCount = new();
        [SerializeField][ReadOnly] private List<Room> roomPrefabs;
        [SerializeField][ReadOnly] private List<Room> startRoomPrefabs;
        [SerializeField][ReadOnly] private List<Room> endRoomPrefabs;
        [SerializeField][ReadOnly] private List<Room> staticRoomPrefabs;
        public Room CurrentRoom => currentRoom;
        [SerializeField][ReadOnly] private Room currentRoom;
        #endregion fields & properties

        #region methods
        private void OnDestroy()
        {
            ClearGeneratedData();
        }

        [Button(nameof(CreateBraidMaze))]
        public void CreateBraidMaze()
        {
            //generate raw data
            ClearGeneratedData();
            SetWorldParameters();
            braidMaze.GenerateMaze();

            //generate objects
            Cell startCell = FindStartCell();
            CreateBeginningRoom(startCell, out RoomSpawnInfo currentRoomInfo);
            GenerateBraidMazeRecursive(currentRoomInfo);
            OnCreateEnd?.Invoke();
        }
        private Cell FindStartCell()
        {
            Cell startCell = new(0, 0);
            startCell.IsRoom = false;
            for (int x = 0; x < braidMaze.Size.x; x++)
            {
                for (int y = 0; y < braidMaze.Size.y; y++)
                {
                    startCell = braidMaze.Maze[x, y];
                    if (startCell.IsRoom) break;
                }
                if (startCell.IsRoom) break;
            }
            return startCell;
        }
        private void CreateBeginningRoom(Cell startCell, out RoomSpawnInfo roomInfo)
        {
            Vector3 spawnWorldPosition = new(startCell.Position.x + buildStart.position.x, buildStart.position.y, startCell.Position.y + buildStart.position.z);
            RoomSpawnInfo currentRoomInfo = new(RandomizeRoomPrefab(null, Direction.Right, startCell), spawnWorldPosition, startCell);
            generatedRoomsCount.TryOpenItem(new(currentRoomInfo.Prefab.Id, 1), x => x.Id == currentRoomInfo.Prefab.Id, out _);
            generatedRoomsInfo.Add(currentRoomInfo);
            roomInfo = currentRoomInfo;
        }
        private void SetWorldParameters()
        {
            WorldType currentWorldType = GameData.Data.WorldsData.CurrentWorld;
            WorldInfo currentWorldInfo = DB.Instance.WorldsInfo.Find(x => x.Data.WorldType == currentWorldType).Data;

            braidMaze.Size = currentWorldInfo.MazeSize;
            roomPrefabs = currentWorldInfo.Rooms.ToList();
            staticRoomPrefabs = GetStaticDefaultRoomPrefabs().ToList();
            startRoomPrefabs = currentWorldInfo.StartRooms.ToList();
            endRoomPrefabs = currentWorldInfo.EndRooms.ToList();
        }
        private void GenerateBraidMazeRecursive(RoomSpawnInfo roomInfo)
        {
            Vector2Int currentCellCoordinates = roomInfo.RelatedCell.Position;
            CheckStartRoomOnGenerate(roomInfo);

            foreach (Direction direction in roomInfo.RelatedCell.AvailableDirections)
            {
                Vector2Int nextCellCoordinates = direction.GetEndCoordinates(currentCellCoordinates);
                if (GetRoomInfoByCoordinates(nextCellCoordinates) != null) continue;

                CreateNextCell(currentCellCoordinates, roomInfo, direction, out RoomSpawnInfo nextCellInfo);

                GenerateBraidMazeRecursive(nextCellInfo);
            }
        }
        /// <summary>
        ///Set this if you want absolutely fixed maze and uses GetRandomRoomsWithFixedCoordinates().
        ///I don't care because player can only see adjacent rooms except current
        /// </summary>
        /// <param name="nextCellCoordinates"></param>
        /// <param name="currentCellCoordinates"></param>
        /// <param name="offsetRoom"></param>
        /// <param name="direction"></param>
        /// <param name="nextCellInfo"></param>
        private void CreateNextCellFithFixedCoordinates(Vector2Int currentCellCoordinates, RoomSpawnInfo offsetRoom, Direction direction, out RoomSpawnInfo nextCellInfo)
        {
            nextCellInfo = null;
            Vector2Int nextCellCoordinates = direction.GetEndCoordinates(currentCellCoordinates);
            Cell nextCell = braidMaze.Maze[nextCellCoordinates.x, nextCellCoordinates.y];
            Vector2Int nextCellDirection = nextCellCoordinates - currentCellCoordinates;
            Room nextCellPrefab = RandomizeRoomPrefab(offsetRoom, direction, nextCell);
            Vector2 nextRoomSize = nextCellPrefab.RectSize;
            Vector3 nextCellSpawnPosition = new(offsetRoom.SpawnWorldPosition.x + nextCellDirection.x * (nextRoomSize.x / 2 + offsetRoom.Prefab.RectSize.x / 2),
                                                offsetRoom.SpawnWorldPosition.y,
                                                offsetRoom.SpawnWorldPosition.z + nextCellDirection.y * (nextRoomSize.y / 2 + offsetRoom.Prefab.RectSize.y / 2));
            int counter = 0;
            foreach (Cell alginedCell in GetAlignedCells(nextCellPrefab, nextCellCoordinates, direction))
            {
                RoomSpawnInfo newInfo = new(nextCellPrefab, nextCellSpawnPosition, nextCell);
                if (counter == 0)
                {
                    nextCellInfo = newInfo;
                }
                else
                {
                    //newInfo.CanInstantiate = false;
                }

                generatedRoomsInfo.Add(newInfo);
                counter++;
            }
        }
        private void CreateNextCell(Vector2Int currentCellCoordinates, RoomSpawnInfo offsetRoom, Direction direction, out RoomSpawnInfo nextCellInfo)
        {
            Vector2Int nextCellCoordinates = direction.GetEndCoordinates(currentCellCoordinates);
            Cell nextCell = braidMaze.Maze[nextCellCoordinates.x, nextCellCoordinates.y];

            Room nextCellPrefab = RandomizeRoomPrefab(offsetRoom, direction, nextCell);
            Vector3 nextCellSpawnPosition = GetNextRoomSpawnPosition(nextCellCoordinates, currentCellCoordinates, offsetRoom, nextCellPrefab);
            nextCellInfo = new(nextCellPrefab, nextCellSpawnPosition, nextCell);

            generatedRoomsInfo.Add(nextCellInfo);
            if (!generatedRoomsCount.TryOpenItem(new(nextCellPrefab.Id, 1), x => x.Id == nextCellPrefab.Id, out CountId existsRoom))
                existsRoom.IncreaseCount();
        }
        private Vector3 GetNextRoomSpawnPosition(Vector2Int nextCellCoordinates, Vector2Int currentCellCoordinates, RoomSpawnInfo offsetRoom, Room prefab)
        {
            Vector2Int nextCellDirection = nextCellCoordinates - currentCellCoordinates;
            Vector2 nextRoomSize = prefab.RectSize;
            Vector3 nextCellSpawnPosition = new(offsetRoom.SpawnWorldPosition.x + nextCellDirection.x * (nextRoomSize.x / 2 + offsetRoom.Prefab.RectSize.x / 2),
                                                offsetRoom.SpawnWorldPosition.y,
                                                offsetRoom.SpawnWorldPosition.z + nextCellDirection.y * (nextRoomSize.y / 2 + offsetRoom.Prefab.RectSize.y / 2));
            return nextCellSpawnPosition;
        }
        private void CheckStartRoomOnGenerate(RoomSpawnInfo roomInfo)
        {
            if (!roomInfo.RelatedCell.Position.Equals(braidMaze.EntranceCell)) return;

            TryInstantiateRoom(roomInfo);
            currentRoom = roomInfo.Instantiated;
        }
        private void SubscribeAtRoom(Room room)
        {
            room.OnStartedRoom += ShowNearestRooms;
            room.OnStartedRoom += InvokeHideRooms;
            room.OnBeforeCompletedRoom += ShowNearestRooms;
            room.OnEnterBackRoom += ShowNearestRooms;
        }
        private void UnsubscribeAtRoom(Room room)
        {
            room.OnStartedRoom -= ShowNearestRooms;
            room.OnStartedRoom -= InvokeHideRooms;
            room.OnBeforeCompletedRoom -= ShowNearestRooms;
            room.OnEnterBackRoom -= ShowNearestRooms;
        }
        private void ShowNearestRooms(Room currentRoom)
        {
            RoomSpawnInfo currentRoomInfo = generatedRoomsInfo.Find(x => x.Instantiated == currentRoom);
            List<RoomSpawnInfo> adjacentRooms = GetAdjacentRooms(currentRoomInfo);
            this.currentRoom = currentRoom;

            //spawn
            foreach (RoomSpawnInfo adjacentRoom in adjacentRooms)
            {
                if (adjacentRoom.Instantiated != null) continue;
                TryInstantiateRoom(adjacentRoom);
            }

            //set active
            foreach (RoomSpawnInfo room in generatedRoomsInfo)
            {
                if (room.Instantiated == null) continue;
                bool isRoomAdjacent = adjacentRooms.Exists(x => x == room);
                bool isRoomCurrent = room == currentRoomInfo;
                room.Instantiated.gameObject.SetActive(isRoomAdjacent || isRoomCurrent);
            }

            UpdateRoomDoors(currentRoomInfo.RelatedCell.Position);
        }
        private void InvokeHideRooms(Room _) => Invoke(nameof(TryHideRoomsExceptCurrent), 1f);
        private void TryHideRoomsExceptCurrent()
        {
            if (currentRoom.IsCompleted) return;
            foreach (RoomSpawnInfo room in generatedRoomsInfo)
            {
                if (room.Instantiated == null) continue;
                bool isRoomCurrent = room.Instantiated == currentRoom;
                room.Instantiated.gameObject.SetActive(isRoomCurrent);
            }

        }
        private void UpdateRoomDoors(Vector2Int currentRoomCoordinates)
        {
            Cell cell = braidMaze.Maze[currentRoomCoordinates.x, currentRoomCoordinates.y];
            if (!cell.IsRoom) return;

            foreach (Direction direction in cell.AvailableDirections)
            {
                Vector2Int nextRoomCoordinates = direction.GetEndCoordinates(currentRoomCoordinates);
                RoomSpawnInfo nextRoomInfo = GetRoomInfoByCoordinates(nextRoomCoordinates);
                Room nextRoom = nextRoomInfo.Instantiated;

                TransformDirection currentRoomDirection = direction.ToTransformDirection();
                TransformDirection nextRoomDirection = currentRoomDirection.Diagonal();

                DoorEventData nextRoomDoor = nextRoom.GetDoorEventData(currentRoomDirection);
                DoorEventData currentRoomDoor = currentRoom.GetDoorEventData(nextRoomDirection);

                currentRoomDoor.SetAdjacentDoor(nextRoomDoor.Door, currentRoom);
                nextRoomDoor.SetAdjacentDoor(currentRoomDoor.Door, nextRoom);
            }
        }

        private Room RandomizeStartRoomPrefab()
        {
            int choosedId = Random.Range(0, startRoomPrefabs.Count);
            return startRoomPrefabs[choosedId];
        }
        private Room RandomizeEndRoomPrefab()
        {
            int choosedId = Random.Range(0, endRoomPrefabs.Count);
            return endRoomPrefabs[choosedId];
        }
        private IEnumerable<Room> GetStaticDefaultRoomPrefabs()
        {
            IEnumerable<Room> choosedPrefabs = roomPrefabs;
            choosedPrefabs = GetRoomsByItemExists(choosedPrefabs);
            choosedPrefabs = GetRoomsByTaskCompletion(choosedPrefabs);
            return choosedPrefabs;
        }
        /// <summary>
        /// Some rooms enter each other but who cares
        /// </summary>
        /// <param name="offsetRoom"></param>
        /// <param name="spawnedDirection"></param>
        /// <returns></returns>
        private Room RandomizeRoomPrefab(RoomSpawnInfo offsetRoom, Direction spawnedDirection, Cell cell)
        {
            IEnumerable<Room> choosedPrefabs = staticRoomPrefabs;

            if (TryGetRandomCornerRoom(cell.Position, out Room cornerRoom)) return cornerRoom;

            choosedPrefabs = GetRandomRoomsWithMaxCount(choosedPrefabs);
            choosedPrefabs = GetRandomRoomsWithAllowedDirections(choosedPrefabs, spawnedDirection, cell);
            choosedPrefabs = GetRoomsNonRepeated(offsetRoom, choosedPrefabs);
            choosedPrefabs = GetRandomRoomsWithProbability(choosedPrefabs);
            choosedPrefabs = GetRoomsByHighPriority(choosedPrefabs);

            List<Room> choosedPrefabsList = new();
            foreach (var el in choosedPrefabs)
                choosedPrefabsList.Add(el);

            if (choosedPrefabsList.Count == 0)
            {
                choosedPrefabsList = FixNullRandomizedRoomsCount(offsetRoom, spawnedDirection, cell);
                Debug.Log($"No rooms allowed. Fix - enable repeating.");
            }
            if (choosedPrefabsList.Count == 0)
            {
                choosedPrefabsList = staticRoomPrefabs;
                Debug.LogError($"No rooms allowed. Fix - takes full rnd.");
            }

            int choosedId = Random.Range(0, choosedPrefabsList.Count);
            Room choosed = choosedPrefabsList[choosedId];
            return choosed;
        }
        private List<Room> FixNullRandomizedRoomsCount(RoomSpawnInfo offsetRoom, Direction spawnedDirection, Cell cell)
        {
            IEnumerable<Room> choosedPrefabs = staticRoomPrefabs;
            choosedPrefabs = GetRandomRoomsWithMaxCount(choosedPrefabs);
            choosedPrefabs = GetRandomRoomsWithAllowedDirections(choosedPrefabs, spawnedDirection, cell);
            choosedPrefabs = GetRandomRoomsWithProbability(choosedPrefabs);
            return choosedPrefabs.ToList();
        }
        private IEnumerable<Room> GetRoomsByHighPriority(IEnumerable<Room> choosedPrefabs)
        {
            List<Room> highPriorityRooms = choosedPrefabs.Where(x => x.HighPriority).ToList();
            if (highPriorityRooms.Count == 0) return choosedPrefabs;
            if (CustomMath.GetRandomChance(70)) return choosedPrefabs;
            return highPriorityRooms;
        }
        private IEnumerable<Room> GetRoomsByItemExists(IEnumerable<Room> choosedPrefabs)
        {
            return choosedPrefabs.Where(x => x.CanSpawnByItemExist);
        }
        private IEnumerable<Room> GetRoomsByTaskCompletion(IEnumerable<Room> choosedPrefabs)
        {
            return choosedPrefabs.Where(x => x.CanSpawnByTaskCompletion);
        }
        private IEnumerable<Room> GetRoomsNonRepeated(RoomSpawnInfo offsetRoom, IEnumerable<Room> choosedPrefabs)
        {
            if (offsetRoom == null) return choosedPrefabs;
            IEnumerable<Room> result = choosedPrefabs.Where(x => offsetRoom.Prefab != x);
            return result.Count() == 0 ? choosedPrefabs : result;
        }
        private bool TryGetRandomCornerRoom(Vector2Int cellCoordinates, out Room cornerRoom)
        {
            cornerRoom = null;
            if (cellCoordinates.Equals(braidMaze.EntranceCell) && startRoomPrefabs.Count > 0)
            {
                cornerRoom = RandomizeStartRoomPrefab();
                return true;
            }
            if (cellCoordinates.Equals(braidMaze.ExitCell) && endRoomPrefabs.Count > 0)
            {
                cornerRoom = RandomizeEndRoomPrefab();
                return true;
            }
            return false;
        }
        private IEnumerable<Room> GetRandomRoomsWithMaxCount(IEnumerable<Room> choosedPrefabs)
        {
            return choosedPrefabs.Where(IsRoomAllowedForMaxCount);
        }
        private bool IsRoomAllowedForMaxCount(Room prefab)
        {
            if (prefab.MaxSpawnCount == -1) return true;
            if (!generatedRoomsCount.IsOpened(r => r.Id == prefab.Id, out CountId opened)) return true;
            if (opened.Count < prefab.MaxSpawnCount) return true;
            return false;
        }
        private IEnumerable<Room> GetRandomRoomsWithProbability(IEnumerable<Room> choosedPrefabs)
        {
            IEnumerable<Room> rooms = choosedPrefabs.Where(x => Random.Range(0, 99.9f) < x.SpawnProbability);

            int totalCount = choosedPrefabs.Count();
            int randomizedCount = rooms.Count();
            int minFixedCount = 3;
            int maxFixedCount = Mathf.Max(Mathf.RoundToInt(totalCount / 5f), minFixedCount);
            if (randomizedCount > 0 && (randomizedCount > maxFixedCount || totalCount <= maxFixedCount)) return rooms;

            List<Room> fixedRandomRooms = new();
            List<Room> choosedPrefabsList = new();
            foreach (var el in choosedPrefabs)
                choosedPrefabsList.Add(el);
            for (int i = 0; i < maxFixedCount; ++i)
            {
                int randomId = Random.Range(0, totalCount);
                Room choosedRoom = choosedPrefabsList[randomId];
                fixedRandomRooms.Add(choosedRoom);
            }
            return fixedRandomRooms;
        }
        private IEnumerable<Room> GetRandomRoomsWithAllowedDirections(IEnumerable<Room> choosedPrefabs, Direction spawnedDirection, Cell cell)
        {
            return choosedPrefabs.Where(x => IsRoomAllowedForDirection(x, spawnedDirection, cell));
        }
        private bool IsRoomAllowedForDirection(Room prefab, Direction spawnedDirection, Cell cell)
        {
            if (!prefab.AllowedSpawnDirections.Contains(spawnedDirection)) return false;
            foreach (Direction nextRoomDirection in cell.AvailableDirections)
            {
                if (nextRoomDirection == spawnedDirection.Diagonal()) continue;
                if (!prefab.AllowedSpawnDirections.Contains(nextRoomDirection)) return false;
            }

            return true;
        }

        /// <summary>
        /// Makes absolutely fixed maze.
        /// Maze will be less complex.
        /// </summary>
        /// <param name="choosedPrefabs"></param>
        /// <param name="offsetRoom"></param>
        /// <param name="spawnedDirection"></param>
        /// <returns></returns>
        private IEnumerable<Room> GetRandomRoomsWithFixedCoordinates(IEnumerable<Room> choosedPrefabs, RoomSpawnInfo offsetRoom, Direction spawnedDirection)
        {
            Vector2Int maxCellSize = new(1, 1);
            Vector2Int currentCoordinates = spawnedDirection.GetEndCoordinates(offsetRoom.RelatedCell.Position);
            currentCoordinates = spawnedDirection.GetEndCoordinates(currentCoordinates);
            while (true)
            {
                Cell nextCell = null;

                try { nextCell = braidMaze.Maze[currentCoordinates.x, currentCoordinates.y]; }
                catch { break; }
                if (!nextCell.IsRoom || generatedRoomsInfo.Exists(x => x.RelatedCell == nextCell)) break;
                if (spawnedDirection == Direction.Right || spawnedDirection == Direction.Left)
                    maxCellSize.x++;
                else //up or down
                    maxCellSize.y++;

                currentCoordinates = spawnedDirection.GetEndCoordinates(currentCoordinates);
            }
            return choosedPrefabs.Where(x => x.CellSize.x <= maxCellSize.x && x.CellSize.y <= maxCellSize.y);
        }
        /// <summary>
        /// Adjacent rooms doesn't collide with each other.
        /// Makes struct of maze completely broken.
        /// </summary>
        /// <param name="choosedPrefabs"></param>
        /// <param name="offsetRoom"></param>
        /// <param name="spawnedDirection"></param>
        /// <returns></returns>
        private IEnumerable<Room> GetRandomRoomsWithFixedSize(IEnumerable<Room> choosedPrefabs, RoomSpawnInfo offsetRoom, Direction spawnedDirection)
        {
            List<RoomSpawnInfo> allAdjacentRooms = GetAdjacentRooms(offsetRoom);

            List<Direction> adjacentBlockedRoomDirections = new();
            if (spawnedDirection == Direction.Right || spawnedDirection == Direction.Left)
            {
                adjacentBlockedRoomDirections.AddRange(offsetRoom.RelatedCell.AvailableDirections.Where(x => x != Direction.Right && x != Direction.Left));
            }
            else //up or down
            {
                adjacentBlockedRoomDirections.AddRange(offsetRoom.RelatedCell.AvailableDirections.Where(x => x != Direction.Up && x != Direction.Down));
            }
            List<RoomSpawnInfo> adjacentBlockedRooms = new();
            foreach (var el in adjacentBlockedRoomDirections)
            {
                RoomSpawnInfo adjRoomInfo = GetRoomInfoByCoordinates(el.GetEndCoordinates(offsetRoom.RelatedCell.Position));
                if (adjRoomInfo == null || adjRoomInfo.Prefab == null) continue;
                adjacentBlockedRooms.Add(adjRoomInfo);
            }

            if (spawnedDirection == Direction.Right || spawnedDirection == Direction.Left)
            {
                bool IsRoomAllowed(Room checkRoom)
                {
                    if (checkRoom.RectSize.y <= offsetRoom.Prefab.RectSize.y) return true;
                    foreach (var adjRoom in adjacentBlockedRooms)
                    {
                        if (adjRoom.Prefab.RectSize.x > offsetRoom.Prefab.RectSize.x) return false;
                    }
                    return true;
                }
                choosedPrefabs = choosedPrefabs.Where(x => IsRoomAllowed(x));
            }
            else //up or down
            {
                bool IsRoomAllowed(Room checkRoom)
                {
                    if (checkRoom.RectSize.x <= offsetRoom.Prefab.RectSize.x) return true;
                    foreach (var adjRoom in adjacentBlockedRooms)
                    {
                        if (adjRoom.Prefab.RectSize.y > offsetRoom.Prefab.RectSize.y) return false;
                    }
                    return true;
                }
                choosedPrefabs = choosedPrefabs.Where(x => IsRoomAllowed(x));
            }
            return choosedPrefabs;
        }

        private void TryInstantiateRoom(RoomSpawnInfo info)
        {
            //if (!info.CanInstantiate) return;
            Room room = GameObject.Instantiate(info.Prefab);
            room.transform.position = info.SpawnWorldPosition;
            info.Instantiated = room;
            SubscribeAtRoom(room);
            return;
        }
        /// <summary>
        /// Invokes automatically in <see cref="CreateBraidMaze"/>
        /// </summary>
        public void ClearGeneratedData()
        {
            foreach (var el in generatedRoomsInfo)
            {
                if (el.Instantiated == null) continue;
                UnsubscribeAtRoom(el.Instantiated);
                Destroy(el.Instantiated.gameObject);
            }
            generatedRoomsCount = new();
            generatedRoomsInfo = new();
        }

        private RoomSpawnInfo GetRoomInfoByCoordinates(Vector2Int currentRoomCoordinates) => generatedRoomsInfo.Find(x => x.RelatedCell.Position.Equals(currentRoomCoordinates));
        private List<RoomSpawnInfo> GetAdjacentRooms(RoomSpawnInfo relatedRoom)
        {
            List<RoomSpawnInfo> infos = new();
            foreach (Direction direction in relatedRoom.RelatedCell.AvailableDirections)
            {
                Vector2Int nextCellCoordinates = direction.GetEndCoordinates(relatedRoom.RelatedCell.Position);
                RoomSpawnInfo info = GetRoomInfoByCoordinates(nextCellCoordinates);
                infos.Add(info);
            }
            return infos;
        }
        private List<Cell> GetAlignedCells(Room prefab, Vector2Int currentCoordinates, Direction spawnedDirection)
        {
            List<Cell> result = new();
            for (int x = 0; x < prefab.CellSize.x; ++x)
            {
                for (int y = 0; y < prefab.CellSize.y; ++y)
                {
                    result.Add(braidMaze.Maze[currentCoordinates.x + x, currentCoordinates.y + y]);
                }
            }
            return result;
        }
        #endregion methods

        [System.Serializable]
        private class CountId
        {
            public int Id => id;
            [SerializeField] private int id;
            public int Count => count;
            [SerializeField] private int count;

            public void IncreaseCount() => count++;
            public void DecreaseCount() => count--;
            public CountId(int id, int count)
            {
                this.id = id;
                this.count = count;
            }
        }

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private bool ___testBool;
        [Button(nameof(ShowAllRooms))]
        private void ShowAllRooms()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            StartCoroutine(WaitForShowAllRooms(GetRoomInfoByCoordinates(braidMaze.EntranceCell)));
        }
        private IEnumerator WaitForShowAllRooms(RoomSpawnInfo instantiatedInfo)
        {
            Vector2Int currentCellCoordinates = instantiatedInfo.RelatedCell.Position;
            foreach (var el in generatedRoomsInfo)
            {
                if (el.Instantiated != null)
                {
                    el.Instantiated.gameObject.SetActive(true);
                    continue;
                }

                TryInstantiateRoom(el);
                yield return null;
            }
            foreach (var el in generatedRoomsInfo)
            {
                UpdateRoomDoors(el.RelatedCell.Position);
            }
        }
        
        [Button(nameof(DoAllTests))]
        private void DoAllTests()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;

            DebugCommands.BenchmarkRepeat(500, delegate { TestCreatingMaze(WorldType.Factory); }, "Factory Gen");
            DebugCommands.BenchmarkRepeat(500, delegate { TestCreatingMaze(WorldType.CrystalClockwork); }, "Crystal Clockwork Gen");
            DebugCommands.BenchmarkRepeat(500, delegate { TestCreatingMaze(WorldType.MetalRecycling); }, "Metal Recycling Gen");
            DebugCommands.BenchmarkRepeat(500, delegate { TestCreatingMaze(WorldType.Chaos); }, "Chaos Gen");
            DebugCommands.BenchmarkRepeat(500, delegate { TestCreatingMaze(WorldType.Hopelessness); }, "Hopelessness Gen");
        }
        private void TestCreatingMaze(WorldType world)
        {
            GameData.Data.WorldsData.CurrentWorld = world;
            ClearGeneratedData();
            SetWorldParameters();
            braidMaze.GenerateMaze();

            Cell startCell = FindStartCell();
            CreateBeginningRoom(startCell, out RoomSpawnInfo currentRoomInfo);
            TestGenerateRecursive(currentRoomInfo);
            //OnCreateEnd?.Invoke(); don't invoke other components
        }
        private void TestGenerateRecursive(RoomSpawnInfo roomInfo)
        {
            Vector2Int currentCellCoordinates = roomInfo.RelatedCell.Position;
            //CheckStartRoomOnGenerate(roomInfo); don't need performance lose

            foreach (Direction direction in roomInfo.RelatedCell.AvailableDirections)
            {
                Vector2Int nextCellCoordinates = direction.GetEndCoordinates(currentCellCoordinates);
                if (GetRoomInfoByCoordinates(nextCellCoordinates) != null) continue;

                CreateNextCell(currentCellCoordinates, roomInfo, direction, out RoomSpawnInfo nextCellInfo);

                TestGenerateRecursive(nextCellInfo);
            }
        }
#endif //UNITY_EDITOR
    }
}