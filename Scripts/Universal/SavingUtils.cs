using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Data;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Data.Settings;
using Data.Stored;
using Data.Interfaces;
using Universal.UI;
using EditorCustom.Attributes;
using DebugStuff;

namespace Universal
{
    public class SavingUtils : MonoBehaviour, IInitializable, IStartInitializable
    {
        #region fields & properties
        public static SavingUtils Instance { get; private set; }
        public static UnityAction OnBeforeSave;
        public static UnityAction OnAfterSave;
        public static UnityAction OnDataReset;
        public static UnityAction OnSettingsReset;
        public static string StreamingAssetsPath => Application.dataPath + "/StreamingAssets";
        public static string LanguagePath => Application.dataPath + "/StreamingAssets/Language";
        #endregion fields & properties

        #region methods
        public void Init()
        {
            Instance = this;

            CheckSaves();
            LoadAll();
        }
        private void CheckSaves()
        {
            if (!IsFileExists(Application.persistentDataPath, GameData.SaveName + GameData.SaveExtension)) ResetTotalProgress();
            if (!IsFileExists(Application.persistentDataPath, SettingsData.SaveName + SettingsData.SaveExtension)) ResetSettings();
        }
        private bool IsFileExists(string dataPath, string fullFileName)
        {
            return File.Exists(Path.Combine(dataPath, fullFileName));
        }
        public void Start()
        {
            SaveAll();
        }

        public static List<string> GetLanguageNames()
        {
            var diInfo = new DirectoryInfo(LanguagePath);
            var filesInfo = diInfo.GetFiles("*.json");
            List<string> list = new();
            for (int i = 0; i < filesInfo.Length; i++)
                list.Add(filesInfo[i].Name.Remove(filesInfo[i].Name.Length - 5));
            return list;
        }
        public static LanguageData GetLanguage() => GetLanguage(SettingsData.Data.LanguageSettings.ChoosedLanguage);
        public static LanguageData GetLanguage(string lang)
        {
            string json = File.ReadAllText(Path.Combine(LanguagePath, $"{lang}.json"));
            LanguageData data = JsonUtility.FromJson<LanguageData>(json);
            return data;
        }
        private void SaveAll()
        {
            if (CanSave())
                SaveGameData();
            SaveSettings();
        }
        public static bool CanSave()
        {
            if (GameData.Data.WorldsData.CurrentWorld != WorldType.Portal && SceneLoader.CurrentScene.name.Equals("Game")) return false;
            if (!GameData.Data.TutorialData.IsCompleted) return false;
            
            return true;
        }
        /// <summary>
        /// Probably you also might want to use <see cref="CanSave"/> to avoid save bugs.
        /// </summary>
        public static void SaveGameData()
        {
            OnBeforeSave?.Invoke();
            string rawJson = JsonUtility.ToJson(GameData.Data);
            string json = Cryptography.Encrypt(rawJson);
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, GameData.SaveName + GameData.SaveExtension), FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, json);
                fs.Close();
            }
            OnAfterSave?.Invoke();
        }
        public static void SaveSettings()
        {
            SaveJson(SettingsData.Data, SettingsData.SaveName + SettingsData.SaveExtension);
        }
        public static void SaveJson<T>(T data, string saveName)
        {
            string json = JsonUtility.ToJson(data);
            string path = Path.Combine(Application.persistentDataPath, saveName);
            File.WriteAllText(path, json);
        }
        public static T LoadJson<T>(string saveName)
        {
            string path = Path.Combine(Application.persistentDataPath, saveName);
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        private void LoadAll()
        {
            LoadGameData();
            LoadSettings();
        }
        private static void LoadGameData()
        {
            string json;
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, GameData.SaveName + ".data"), FileMode.Open))
            {
                var bf = new BinaryFormatter();
                json = bf.Deserialize(fs).ToString();
                json = Cryptography.Decrypt(json);
                fs.Close();
            }
            GameData.Data = JsonUtility.FromJson<GameData>(json);
        }
        private static void LoadSettings()
        {
            SettingsData.Data = LoadJson<SettingsData>(SettingsData.SaveName + SettingsData.SaveExtension);
        }
        private void ResetSettings()
        {
            SettingsData.Data = new SettingsData();
            SaveSettings();
            OnSettingsReset?.Invoke();
            Debug.Log("Settings reset");
        }
        public static void ResetTotalProgress(bool doAction = true)
        {
            GameData.Data = new GameData();
            SaveGameData();
            if (doAction)
                OnDataReset?.Invoke();
            Debug.Log("Progress reset");
        }
        private void OnApplicationQuit()
        {
            SaveAll();
        }

        #endregion methods

#if UNITY_EDITOR
        [Title("Tests")]
        [SerializeField][DontDraw] private bool ___testBool;
        [Button(nameof(TestSaveGameData))]
        private void TestSaveGameData()
        {
            if (!DebugCommands.IsApplicationPlaying()) return;
            SaveGameData();
        }
#endif //UNITY_EDITOR
    }
}