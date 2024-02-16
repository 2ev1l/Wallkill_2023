using Data.Settings;
using System.Collections.Generic;
using UnityEngine;
using EditorCustom.Attributes;

namespace Menu
{
    public class AudioPanel : SettingsPanel<Data.Settings.AudioSettings>
    {
        #region fields & properties
        [SerializeField] private IntItemList audioList;
        [SerializeField] private IntItemList musicList;
        [SerializeField] private IntItemList soundList;
        protected override Data.Settings.AudioSettings Context => SettingsData.Data.AudioSettings;
        #endregion fields & properties

        #region methods
        [SerializedMethod]
        public override void SaveSettings()
        {
            Data.Settings.AudioSettings a = GetSettings();
            Context.MusicData.Volume = a.MusicData.Volume;
            Context.SoundData.Volume = a.SoundData.Volume;
            Context.AudioData.Volume = a.AudioData.Volume;
        }

        protected override Data.Settings.AudioSettings GetSettings()
        {
            AudioData ad = new(audioList.CurrentPageItems[0].Value / 100f);
            AudioData md = new(musicList.CurrentPageItems[0].Value / 100f);
            AudioData sd = new(soundList.CurrentPageItems[0].Value / 100f);
            Data.Settings.AudioSettings a = new(sd, md, ad);
            return a;
        }

        protected override void UpdateAllLists()
        {
            List<int> list = new() { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100 };
            audioList.UpdateListData(list);
            audioList.ItemsList.ShowAt(Mathf.RoundToInt(Context.AudioData.Volume * 100));
            musicList.UpdateListData(list);
            musicList.ItemsList.ShowAt(Mathf.RoundToInt(Context.MusicData.Volume * 100));
            soundList.UpdateListData(list);
            soundList.ItemsList.ShowAt(Mathf.RoundToInt(Context.SoundData.Volume * 100));
        }
        #endregion methods
    }
}