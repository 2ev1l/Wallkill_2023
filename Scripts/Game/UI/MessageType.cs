using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Game.UI
{
    #region enum
    public enum MessageType
    {
        OutOfAmmo,
        StaminaIsOver,
    }
    #endregion enum

    public static class MessageTypeExtension
    {
        #region methods
        public static string GetMessage(this MessageType type) => type switch
        {
            MessageType.OutOfAmmo => LanguageLoader.GetTextByType(TextType.Game, 6),
            MessageType.StaminaIsOver => LanguageLoader.GetTextByType(TextType.Game, 7),
            _ => throw new System.NotImplementedException($"{type} message")
        };
        #endregion methods
    }
}