using Data.Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EditorCustom.Attributes;
using Universal.UI.Triggers;
using Game.Labyrinth;
using Game.Player;
using Universal;
using Game.UI;
using Data.Stored;

namespace Game.Rooms
{
    [DisallowMultipleComponent]
    public class Room : MonoBehaviour
    {
        #region fields & properties
        public static UnityAction<int> OnRoomStart;
        public static UnityAction<int> OnRoomCompleted;

        public UnityEvent OnRoomStartEvents;
        public UnityEvent OnRoomCompleteEvents;

        public UnityAction<Room> OnBeforeCompletedRoom;
        public static UnityAction<Room> OnCompletedRoom;
        public UnityAction OnCompleted;

        public UnityAction<Room> OnStartedRoom;
        public UnityAction OnStart;
        /// <summary>
        /// Invokes if player enter back but room is completed already
        /// </summary>
        public UnityAction<Room> OnEnterBackRoom;
        public UnityAction OnEnterBack;

        public int Id => id;
        [SerializeField][Min(0)] private int id;
        [SerializeField] private TriggerCatcher playerTriggerCatcher;
        public Vector2 RectSize => rectSize;
        [Title("Spawn")][SerializeField] private Vector2 rectSize = new(10, 10);
        /// <summary>
        /// Serialize only when you use <see cref="MazeCreator.GetRandomRoomsWithFixedCoordinates(IEnumerable{Room}, RoomSpawnInfo, Direction)"/>
        /// </summary>
        public Vector2Int CellSize => cellSize;
        private Vector2Int cellSize = new(1, 1);
        public int SpawnProbability => spawnProbability;
        [SerializeField][Range(0, 100)] private int spawnProbability = 50;
        public int MaxSpawnCount => maxSpawnCount;
        [SerializeField][Min(-1)] private int maxSpawnCount = -1;
        public bool CanSpawnByItemExist => checkItemExists ? checkExistsObject.Exists() : true;
        [SerializeField] private bool checkItemExists = false;
        [SerializeField][DrawIf(nameof(checkItemExists), true)] private CheckItemExists checkExistsObject;
        public bool CanSpawnByTaskCompletion => checkTaskCompletion ? GameData.Data.TasksData.CompletedTasks.Exists(x => x == requiredCompletedTask, out _) : true;
        [SerializeField] private bool checkTaskCompletion = false;
        [SerializeField][DrawIf(nameof(checkTaskCompletion), true)][Min(0)] private int requiredCompletedTask = 0;
        public bool HighPriority => highPriority;
        [SerializeField] private bool highPriority = false;
        public Transform SafePlayerOffset => safePlayerOffset;
        [SerializeField] private Transform safePlayerOffset;
        [SerializeField] public IReadOnlyList<Direction> AllowedSpawnDirections => allowedSpawnDirections;
        [SerializeField] private List<Direction> allowedSpawnDirections = new() { Direction.Up, Direction.Down, Direction.Right, Direction.Left };
        [SerializeField] private List<DoorEventData> doors = new();

        [Title("Settings")]
        [SerializeField] private bool doReward = false;
        [SerializeField][DrawIf(nameof(doReward), true)] private Reward reward;
        public float OptimalTimeForReward => optimalTimeForReward;
        [SerializeField][DrawIf(nameof(doReward), true)][Min(0)] private float optimalTimeForReward = 15;
        [SerializeField][DrawIf(nameof(doReward), true)] private AnimationCurve rewardCurve = AnimationCurve.Linear(0, 0, 1, 1);


        public bool IsCompleted => isCompleted;
        [Title("Read Only")][SerializeField][ReadOnly] private bool isCompleted = false;
        public bool IsStarted => isStarted;
        [SerializeField][ReadOnly] private bool isStarted = false;

        [SerializeField][ReadOnly] private float startTime = -Mathf.Infinity;
        [SerializeField][ReadOnly] private float completionTime = Mathf.Infinity;
        public float TimeSpentToComplete => completionTime - startTime;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            playerTriggerCatcher.OnEnterTagSimple += OnPlayerEntered;
        }
        private void OnDisable()
        {
            playerTriggerCatcher.OnEnterTagSimple -= OnPlayerEntered;
        }
        private void OnPlayerEntered()
        {
            TryStartRoom();
            TryEnterRoomBack();
        }

        [Button(nameof(TryCompleteRoom))]
        public void TryCompleteRoom()
        {
            if (isCompleted) return;
            OnBeforeCompletedRoom?.Invoke(this);
            CheckDoorsForAdjacency();
            StoreCompletionTime();
            AddReward();
            isCompleted = true;
            OnCompleted?.Invoke();
            OnCompletedRoom?.Invoke(this);
            OnRoomCompleteEvents?.Invoke();
            OnRoomCompleted?.Invoke(id);
        }

        private void StoreStartTime()
        {
            startTime = Time.time;
        }
        private void StoreCompletionTime()
        {
            completionTime = Time.time;
        }
        [Button(nameof(AddReward))]
        private void AddReward()
        {
            if (!doReward) return;
            int maxRewardCount = CustomMath.Multiply(reward.Count, rewardCurve.Evaluate(TimeSpentToComplete / optimalTimeForReward) * 100);
            maxRewardCount = Mathf.Clamp(maxRewardCount, 0, reward.Count);
            reward.AddReward(maxRewardCount);
        }
        private void CheckDoorsForAdjacency()
        {
            foreach (DoorEventData data in doors)
            {
                if (data.Door.AdjacentRoom == null)
                    data.Door.DisableOpening();
            }
        }
        public List<Direction> GetAvailableDoorsDirections()
        {
            List<Direction> directions = new();
            foreach (DoorEventData data in doors)
            {
                if (data.Door.CanOpen) continue;
                directions.Add(data.Door.Direction.ToDirection());
            }
            return directions;
        }
        [Button(nameof(TryStartRoom))]
        public void TryStartRoom()
        {
            if (isStarted || isCompleted) return;
            StoreStartTime();
            isStarted = true;
            OnStart?.Invoke();
            OnStartedRoom?.Invoke(this);
            OnRoomStartEvents?.Invoke();
            OnRoomStart?.Invoke(id);
        }

        [Button(nameof(TryEnterRoomBack))]
        private void TryEnterRoomBack()
        {
            if (!isCompleted) return;
            OnEnterBack?.Invoke();
            OnEnterBackRoom?.Invoke(this);
        }
        public DoorEventData GetDoorEventData(TransformDirection direction) => doors.Find(x => x.Door.Direction == direction);
        #endregion methods
    }
}