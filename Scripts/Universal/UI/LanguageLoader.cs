using Data;
using Data.Settings;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Universal.UI
{
    [DisallowMultipleComponent]
    public class LanguageLoader : TextUpdater
    {
        #region fields & properties
        public int Id
        {
            get => id;
            set => SetId(value);
        }
        [SerializeField] private int id;
        public TextType TextType
        {
            get => textType;
            set => SetTextType(value);
        }
        [SerializeField] private TextType textType;
        private string additionalTextBefore;
        private string additionalTextAfter;

        private static string commandStart = "<!";
        private static string commandEnd = "!>";
        #endregion fields & properties

        #region methods
        public void Start()
        {
            Load();
        }
        public void Load()
        {
            UpdateText();
            if (id < 0)
            {
                Text.text = "";
                return;
            }
            string text = GetTextByType(textType, id);
            text = TryReplaceText(text, textType);

            Text.text = additionalTextBefore + text + additionalTextAfter;
        }
        /// <summary>
        /// Old added text will be deleted
        /// </summary>
        /// <param name="text"></param>
        public void AddTextAfter(string text)
        {
            additionalTextAfter = text;
            Load();
        }
        /// <summary>
        /// Old added text will be deleted
        /// </summary>
        /// <param name="text"></param>
        public void AddTextBefore(string text)
        {
            additionalTextBefore = text;
            Load();
        }
        public void RemoveAdditionalText()
        {
            additionalTextBefore = "";
            additionalTextAfter = "";
            Load();
        }
        /// <summary>
        /// Removes any tags <see cref="{}"/>
        /// </summary>
        public void RemoveTextTags()
        {
            string text = Text.text;
            Text.text = RemoveXMLTags(text);
        }
        public static string RemoveXMLTags(string text)
        {
            int startIndex = text.IndexOf("<");
            int endIndex = text.IndexOf(">");
            while (startIndex > -1)
            {
                text = text.Remove(startIndex, endIndex - startIndex + 1);
                startIndex = text.IndexOf("<");
                endIndex = text.IndexOf(">");
            }
            return text;
        }
        public void Replace(string oldValue, string newValue) => Text.text = Text.text.Replace(oldValue, newValue);
        private static string TryReplaceText(string text, TextType textType)
        {
            text = TryReplaceEquationTag(text, textType);
            text = TryReplaceArrowTags(text, textType);
            return text;
        }
        private static string TryReplaceArrowTags(string text, TextType textType)
        {
            while (text.IndexOf(commandStart) > -1)
            {
                GetArrowCommands(text, out string replaceableText, out string command, out int value);
                string commandResult = DecryptArrowCommand(command, value, textType);
                text = text.Replace(replaceableText, commandResult);
            }
            return text;
        }
        private static void GetArrowCommands(string text, out string replacebaleText, out string command, out int value)
        {
            value = 0;
            command = "";
            replacebaleText = "";
            int arrowStartIndex = text.IndexOf(commandStart);
            int arrowEndIndex = text.IndexOf(commandEnd);
            if (arrowStartIndex < 0) return;

            replacebaleText = text.Substring(arrowStartIndex, arrowEndIndex - arrowStartIndex + 2);
            string subText = text.Substring(arrowStartIndex + 2, arrowEndIndex - arrowStartIndex - 2);
            int subTextLength = subText.Length;
            int equationPos = subText.IndexOf("=");
            if (equationPos < 0) return;

            command = subText[..equationPos];
            command = command.ToLower();
            try { value = System.Convert.ToInt32(subText.Substring(equationPos + 1, subTextLength - equationPos - 1)); }
            catch { }
        }
        private static string DecryptArrowCommand(string command, int value, TextType textType) => command switch
        {
            "get" => textType.GetRawText(value),
            _ => throw new System.NotImplementedException($"Wrong arrow command: '{command}' at {textType} {value}")
        };
        private static string TryReplaceEquationTag(string text, TextType textType)
        {
            if (text.Length > 0 && text[..1].Equals("="))
            {
                try
                {
                    int id = System.Convert.ToInt32(text[1..text.Length]);
                    text = GetTextByType(textType, id);
                }
                catch { }
            }
            return text;
        }
        public static string GetTextByType(TextType textType, int id)
        {
            string text = textType.GetRawText(id);
            text = TryReplaceText(text, textType);
            return text;
        }

        private void SetId(int value)
        {
            value = Mathf.Max(value, -1);
            id = value;
            Load();
        }
        private void SetTextType(TextType value)
        {
            textType = value;
            Load();
        }
        public void ChangeValues(int id, TextType textType)
        {
            this.textType = textType;
            SetId(id);
        }

        public static string GetTextByKeyCode(KeyCode key) => key switch
        {
            KeyCode.Alpha0 => ".0",
            KeyCode.Alpha1 => ".1",
            KeyCode.Alpha2 => ".2",
            KeyCode.Alpha3 => ".3",
            KeyCode.Alpha4 => ".4",
            KeyCode.Alpha5 => ".5",
            KeyCode.Alpha6 => ".6",
            KeyCode.Alpha7 => ".7",
            KeyCode.Alpha8 => ".8",
            KeyCode.Alpha9 => ".9",
            KeyCode.LeftShift => "LShift",
            KeyCode.RightShift => "RShift",
            KeyCode.LeftAlt => "LAlt",
            KeyCode.RightAlt => "RAlt",
            KeyCode.LeftControl => "LCtrl",
            KeyCode.RightControl => "RCtrl",
            KeyCode.UpArrow => "U <|",
            KeyCode.DownArrow => "D >|",
            KeyCode.RightArrow => "R >",
            KeyCode.LeftArrow => "L <",
            KeyCode.Mouse0 => "LMB",
            KeyCode.Mouse1 => "RMB",
            KeyCode.Mouse2 => "MMB",
            KeyCode.Mouse3 => "4MB",
            KeyCode.Mouse4 => "5MB",
            _ => key.ToString(),
        };
        #endregion methods
    }
}