using Data.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Data.Stored
{
    [System.Serializable]
    public class WorldsData
    {
        #region fields & properties
        public UnityAction<WorldType> OnCurrentWorldChanged;
        /// <summary>
        /// World will be reset on scene reload
        /// </summary>
        public WorldType CurrentWorld
        {
            get => currentWorld;
            set => SetCurrentWorld(value);
        }
        [SerializeField][System.NonSerialized] private WorldType currentWorld = WorldType.Portal;

        public IReadOnlyList<WorldStats> Stats => stats;
        [SerializeField]
        private List<WorldStats> stats = new()
        {
            new(WorldType.Portal, true, false),
            new(WorldType.Factory, true, false),
            new(WorldType.CrystalClockwork, false, false),
            new(WorldType.MetalRecycling, false, false),
            new(WorldType.Chaos, false, false),
            new(WorldType.Hopelessness, false, false)
        };
        #endregion fields & properties

        #region methods
        private void SetCurrentWorld(WorldType value)
        {
            currentWorld = value;
            OnCurrentWorldChanged?.Invoke(currentWorld);
        }
        public bool TryCloseWorld(WorldType world) => GetWorldStats(world).TryClose();
        public bool TryOpenWorld(WorldType world) => GetWorldStats(world).TryOpen();
        public bool TryCompleteWorld(WorldType world) => GetWorldStats(world).TryComplete();

        /// <summary>
        /// Warning, provides original class.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public WorldStats GetWorldStats(WorldType world) => stats.Find(x => x.World == world);
        #endregion methods
    }
}