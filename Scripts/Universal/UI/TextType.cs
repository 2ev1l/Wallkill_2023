using Data.Settings;

namespace Universal.UI
{
    #region enum
    public enum TextType
    {
        Help,
        MainMenu,
        Game,
        Items,
        None,
        Tasks,
        Memories
    }
    #endregion enum

    public static class TextTypeExtension
    {
        #region methods
        public static string GetRawText(this TextType textType, int id)
        {
            if (id < 0) return "";

            LanguageData data = TextData.Instance.LoadedData;
            return (textType) switch
            {
                TextType.Help => data.HelpData[id],
                TextType.MainMenu => data.MenuData[id],
                TextType.Game => data.GameData[id],
                TextType.Items => data.ItemsData[id],
                TextType.Tasks => data.TasksData[id],
                TextType.Memories => data.MemoriesData[id],
                TextType.None => "",
                _ => throw new System.NotImplementedException("Text Type"),
            };
        }
        #endregion methods
    }
}