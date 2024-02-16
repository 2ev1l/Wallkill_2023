using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.UI;

namespace Data.Stored
{
    #region enum
    public enum TutorialStage
    {
        WakeUp,
        Moving,
        HandsAttack,
        CameraRotation,
        SprintJump,
        Crouch,
        Weapon,
        Damage,
        Info
    }
    #endregion enum

    public static class TutorialStageExtension
    {
        #region methods
        public static bool TryGetMessage(this TutorialStage ts, out string message)
        {
            message = ts switch
            {
                TutorialStage.WakeUp => LanguageLoader.GetTextByType(TextType.Game, 8),
                TutorialStage.Moving => LanguageLoader.GetTextByType(TextType.Game, 9),
                TutorialStage.HandsAttack => LanguageLoader.GetTextByType(TextType.Game, 11),
                TutorialStage.CameraRotation => LanguageLoader.GetTextByType(TextType.Game, 13),
                TutorialStage.SprintJump => LanguageLoader.GetTextByType(TextType.Game, 15),
                TutorialStage.Crouch => LanguageLoader.GetTextByType(TextType.Game, 16),
                TutorialStage.Weapon => LanguageLoader.GetTextByType(TextType.Game, 18),
                TutorialStage.Damage => LanguageLoader.GetTextByType(TextType.Game, 21),
                TutorialStage.Info => LanguageLoader.GetTextByType(TextType.Game, 22),
                _ => null,
            };
            return message != null;
        }
        #endregion methods
    }
}