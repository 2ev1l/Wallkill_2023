using Data.Settings;
using Data.Stored;
using Game.Labyrinth;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DebugStuff
{
    public class ConsoleToGUI : MonoBehaviour
    {
#if false
        #region fields & properties
        private const int maxChars = 1000;
        private string logInfo = "*Press [Tab] to open/close developer window";
        private string fileName = "";
        string startTimeFormatted = "";

        [SerializeField] private bool drawField = true;
        [SerializeField] private bool enableFileLog = false;
        [SerializeField] private bool ignoreEditor = true;
        private static string SaveDirectory => System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/WALLKILL_LOGS";
        #endregion fields & properties

        #region methods
        private void Awake()
        {
            Log("Start Log", null, LogType.Log);
        }
        private void OnEnable()
        {
            Application.logMessageReceived += Log;
        }
        private void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                drawField = !drawField;
            }
        }
        private void Log(string logString, string stackTrace, LogType type)
        {
#if UNITY_EDITOR
            if (ignoreEditor) return;
#endif
            logInfo = logInfo + "\n" + logString;

            FixField();
            TryDraw(type);

            if (!enableFileLog) return;
            SaveFileLog(logString, stackTrace, type);
        }
        private void FixField()
        {
            if (logInfo.Length > maxChars)
                logInfo = logInfo.Substring(logInfo.Length - maxChars);
        }
        private void TryDraw(LogType type)
        {
            if (type == LogType.Assert || type == LogType.Exception || type == LogType.Error)
                drawField = true;
        }

        private void SaveFileLog(string logString, string stackTrace, LogType type)
        {
            string currentTime = $"{Time.realtimeSinceStartup:F3}";
            logString += $" [Trace: {stackTrace}]";
            logString += $" - {currentTime} s. -";

            TrySetFileName();
            try { System.IO.File.AppendAllText(fileName, logString + "\n"); }
            catch { }

            if (type == LogType.Log || type == LogType.Warning) return;
            string json = "";
            try { json = JsonUtility.ToJson(GetDebugData()); }
            catch
            {
                Debug.Log("Can't save debug data");
                return;
            }

            string path = SaveDirectory + "/save data-" + startTimeFormatted + "-" + currentTime + ".json";
            File.WriteAllText(path, json);
        }

        private void TrySetFileName()
        {
            if (fileName == "")
            {
                System.IO.Directory.CreateDirectory(SaveDirectory);
                startTimeFormatted = DateTime.Now.ToString("MM.dd.yyyy HH.mm.ss");
                fileName = SaveDirectory + "/log-" + startTimeFormatted + ".txt";
            }
        }
        private void DrawField()
        {
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
            GUI.TextArea(new Rect(10, 10, 540, 370), logInfo);
        }
        private void OnGUI()
        {
            if (!drawField) return;
#if UNITY_EDITOR
            if (ignoreEditor) return;
#endif
            DrawField();
        }

        private DebugData GetDebugData()
        {
            int currentMazeRoom = -1;
            List<int> totalMazeRooms = new();
            if (MazeCreator.Instance != null)
            {
                totalMazeRooms = MazeCreator.Instance.GeneratedRoomsId;
                currentMazeRoom = MazeCreator.Instance.CurrentRoom == null ? -1 : MazeCreator.Instance.CurrentRoom.Id;
            }

            DebugData data = new(GameData.Data, SettingsData.Data, GameData.Data.WorldsData.CurrentWorld, currentMazeRoom, totalMazeRooms);

            return data;
        }
        #endregion methods
        [System.Serializable]
        private class DebugData
        {
            [SerializeField] private GameData gameData = null;
            [SerializeField] private SettingsData settingsData = null;
            [SerializeField] private WorldType currentWorld = WorldType.Portal;
            [SerializeField] private int currentMazeRoom = -1;
            [SerializeField] private List<int> totalMazeRooms = new();

            public DebugData(GameData gameData, SettingsData settingsData, WorldType currentWorld, int currentMazeRoom, List<int> totalMazeRooms)
            {
                this.gameData = gameData;
                this.settingsData = settingsData;
                this.currentWorld = currentWorld;
                this.currentMazeRoom = currentMazeRoom;
                this.totalMazeRooms = totalMazeRooms;
            }
        }
#endif
    }
}