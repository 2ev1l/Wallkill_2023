using Data.Stored;
using EditorCustom.Attributes;
using Game.DataBase;
using Game.Rooms;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal;
using Universal.UI;
using Universal.UI.Audio;

namespace Game.Labyrinth
{
    public class WorldLoader : SingleSceneInstance<WorldLoader>
    {
        #region fields & properties
        public UnityAction<WorldType> OnWorldLoaded;

        [SerializeField] private MazeCreator mazeCreator;
        [SerializeField] private Room portalRoom;
        [SerializeField] private MessageReceiver horizontalMessages;
        [SerializeField] private List<WorldAudioData> worldsAudioData;
        public bool IsLoadingFromPortalObject
        {
            get => isLoadingFromPortalObject;
            set => isLoadingFromPortalObject = value;
        }
        [SerializeField] private bool isLoadingFromPortalObject = false;
        public bool SaveLastWorld
        {
            get => saveLastWorld;
            set => saveLastWorld = value;
        }
        [SerializeField] private bool saveLastWorld = false;
        [SerializeField] private WorldType lastWorldType = WorldType.Portal;
        #endregion fields & properties

        #region methods
        private void OnEnable()
        {
            if (GameData.Data.WorldsData.CurrentWorld != WorldType.Portal)
                GameData.Data.WorldsData.CurrentWorld = WorldType.Portal;
            lastWorldType = WorldType.Portal;
        }
        private void OnDisable()
        {
            mazeCreator.OnCreateEnd -= OnMazeCreated;
        }

        private void OnMazeCreated()
        {
            mazeCreator.OnCreateEnd -= OnMazeCreated;
            ScreenFade.Fade(false);
            Vector3 spawnPosition = mazeCreator.CurrentRoom.SafePlayerOffset.position;
            InstancesProvider.Instance.PlayerMoving.TeleportToUnsafe(spawnPosition);
            InstancesProvider.Instance.PlayerInput.StartInput();
            OnAnyWorldLoaded();
        }
        private void OnReadyToLoadPortal()
        {
            ScreenFade.OnBlackScreenFadeDown -= OnReadyToLoadPortal;
            if (!SaveLastWorld) mazeCreator.ClearGeneratedData();
            Vector3 spawnPosition = portalRoom.SafePlayerOffset.position;
            InstancesProvider.Instance.PlayerMoving.TeleportToUnsafe(spawnPosition);
            InstancesProvider.Instance.PlayerInput.StartInput();
            OnAnyWorldLoaded();
        }
        public void LoadWorld(WorldType newWorld)
        {
            //save if loading from portal
            if (SavingUtils.CanSave()) SavingUtils.SaveGameData();

            WorldType currentWorld = GameData.Data.WorldsData.CurrentWorld;
            if (currentWorld != WorldType.Portal)
                lastWorldType = currentWorld;
            GameData.Data.WorldsData.CurrentWorld = newWorld;

            //save if loading to portal
            if (SavingUtils.CanSave()) SavingUtils.SaveGameData();
            StartCoroutine(WorldLoad(newWorld));
        }
        private IEnumerator WorldLoad(WorldType newWorld)
        {
            InstancesProvider.Instance.PlayerInput.StopInput();
            if (newWorld != WorldType.Portal)
            {
                ScreenFade.Fade(true);
                if (!SaveLastWorld || newWorld != lastWorldType)
                {
                    mazeCreator.OnCreateEnd += OnMazeCreated;
                    yield return new WaitForSeconds(1);
                    mazeCreator.CreateBraidMaze();
                }
                else
                {
                    yield return new WaitForSeconds(1);
                    OnMazeCreated();
                }
                SaveLastWorld = false;
            }
            else //portal
            {
                StartCoroutine(ScreenFade.DoCycle());
                ScreenFade.OnBlackScreenFadeDown += OnReadyToLoadPortal;
            }
        }
        private void OnAnyWorldLoaded()
        {
            horizontalMessages.ReceiveMessage($"{DB.Instance.WorldsInfo.Find(x => x.Data.WorldType == GameData.Data.WorldsData.CurrentWorld).Data.Language.Text}");
            WorldAudioData audio = worldsAudioData.Find(x => x.WorldType == GameData.Data.WorldsData.CurrentWorld);
            audio?.PlayThis();
            IsLoadingFromPortalObject = false;
            OnWorldLoaded?.Invoke(GameData.Data.WorldsData.CurrentWorld);
            TryOpenWorldData(GameData.Data.WorldsData.CurrentWorld);
        }

        private void TryOpenWorldData(WorldType worldType)
        {
            WorldsData worldsData = GameData.Data.WorldsData;
            switch (worldType)
            {
                case WorldType.CrystalClockwork:
                    if (worldsData.GetWorldStats(WorldType.Factory).IsCompleted) return;
                    worldsData.TryCompleteWorld(WorldType.Factory);
                    worldsData.TryOpenWorld(WorldType.CrystalClockwork);
                    SavingUtils.SaveGameData();
                    break;
                case WorldType.MetalRecycling:
                    if (worldsData.GetWorldStats(WorldType.CrystalClockwork).IsCompleted) return;
                    worldsData.TryCompleteWorld(WorldType.CrystalClockwork);
                    worldsData.TryOpenWorld(WorldType.MetalRecycling);
                    SavingUtils.SaveGameData();
                    break;
                case WorldType.Chaos:
                    if (worldsData.GetWorldStats(WorldType.MetalRecycling).IsCompleted) return;
                    worldsData.TryCompleteWorld(WorldType.MetalRecycling);
                    worldsData.TryOpenWorld(WorldType.Chaos);
                    SavingUtils.SaveGameData();
                    break;
            }
        }
        #endregion methods

        [System.Serializable]
        private class WorldAudioData
        {
            [SerializeField] private AudioClip music;
            public WorldType WorldType => worldType;
            [SerializeField] private WorldType worldType;
            public void PlayThis() => AudioManager.PlayMusic(music);
        }

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField] private WorldType worldToLoad = WorldType.CrystalClockwork;
        [Button(nameof(LoadPortal))]
        private void LoadPortal() => LoadWorld(WorldType.Portal);
        [Button(nameof(LoadChoosedWorld))]
        private void LoadChoosedWorld() => LoadWorld(worldToLoad);

#endif //UNITY_EDITOR
    }
}