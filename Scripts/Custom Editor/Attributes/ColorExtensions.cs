using UnityEngine;

namespace EditorCustom.Attributes
{
    public static class ColorExtensions
    {
        // Convert the TitleColor enum to an actual Color32
        public static Color32 ToColor(this TitleColor color)
        {
            return color switch
            {
                TitleColor.Aqua => new Color32(127, 219, 255, 255),
                TitleColor.Beige => new Color32(245, 245, 220, 255),
                TitleColor.Black => new Color32(0, 0, 0, 255),
                TitleColor.Blue => new Color32(31, 133, 221, 255),
                TitleColor.BlueVariant => new Color32(67, 110, 238, 255),
                TitleColor.DarkBlue => new Color32(41, 41, 225, 255),
                TitleColor.Bright => new Color32(196, 196, 196, 255),
                TitleColor.Brown => new Color32(148, 96, 59, 255),
                TitleColor.Cyan => new Color32(0, 255, 255, 255),
                TitleColor.DarkGray => new Color32(36, 36, 36, 255),
                TitleColor.Fuchsia => new Color32(240, 18, 190, 255),
                TitleColor.Gray => new Color32(88, 88, 88, 255),
                TitleColor.Green => new Color32(98, 200, 79, 255),
                TitleColor.Indigo => new Color32(75, 0, 130, 255),
                TitleColor.LightGray => new Color32(128, 128, 128, 255),
                TitleColor.Lime => new Color32(1, 255, 112, 255),
                TitleColor.Navy => new Color32(15, 35, 86, 255),
                TitleColor.Olive => new Color32(61, 153, 112, 255),
                TitleColor.DarkOlive => new Color32(47, 79, 79, 255),
                TitleColor.Orange => new Color32(255, 128, 0, 255),
                TitleColor.OrangeVariant => new Color32(255, 135, 62, 255),
                TitleColor.Pink => new Color32(255, 152, 203, 255),
                TitleColor.Red => new Color32(234, 42, 42, 255),
                TitleColor.LightRed => new Color32(217, 71, 71, 255),
                TitleColor.RedVariant => new Color32(232, 10, 10, 255),
                TitleColor.DarkRed => new Color32(144, 20, 39, 255),
                TitleColor.Tan => new Color32(210, 180, 140, 255),
                TitleColor.Teal => new Color32(27, 126, 126, 255),
                TitleColor.Violet => new Color32(181, 93, 237, 255),
                TitleColor.White => new Color32(255, 255, 255, 255),
                TitleColor.Yellow => new Color32(255, 211, 0, 255),
                _ => new Color32(0, 0, 0, 0),
            };
        }
    }
}