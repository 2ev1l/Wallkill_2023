using Data;
using Game.Rooms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal
{
    public static class CustomMath
    {
        #region fields & properties
        #endregion fields & properties

        #region methods
        public static IEnumerator WaitAFrame()
        {
            //yield return Application.isBatchMode ? null : new WaitForEndOfFrame();
            yield return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Result with math quarters 1 => (1, 1); 2 => (-1, 1); 3 => (-1, -1); 4 => (1, -1)</returns>
        public static Vector2 GetScreenSquare(Vector3 cameraPosition, Vector3 currentPosition) => (currentPosition - cameraPosition) switch
        {
            Vector3 vec when vec.x >= 0 && vec.y >= 0 => Vector2.right + Vector2.up,
            Vector3 vec when vec.x <= 0 && vec.y >= 0 => -Vector2.right + Vector2.up,
            Vector3 vec when vec.x <= 0 && vec.y <= 0 => -Vector2.right - Vector2.up,
            Vector3 vec when vec.x >= 0 && vec.y <= 0 => Vector2.right - Vector2.up,
            _ => -Vector2.right - Vector2.up
        };
        public static Vector3 Project(Vector3 direction, Vector3 surface) => direction - Vector3.Dot(direction, surface) * surface;

        /// <summary>
        /// True if chance gained.
        /// </summary>
        /// <param name="chancePercent">0..100%</param>
        /// <returns></returns>
        public static bool GetRandomChance(float chancePercent) => UnityEngine.Random.Range(0f, 100f) < chancePercent;
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Value (0..100) </returns>
        public static float GetRandomChance() => Mathf.Clamp(UnityEngine.Random.Range(0f, 100f), 0.001f, 99.999f);
        /// <summary>
        /// True if chance gained.
        /// </summary>
        /// <param name="chancePercent">0..100%</param>
        /// <returns></returns>
        public static bool GetRandomChance(int chancePercent) => UnityEngine.Random.Range(0, 100) < chancePercent;
        public static int GetRandomFromChancesArray(params float[] chances)
        {
            int finalIndex = chances.Length - 1;
            float maxChance = 100;
            for (int i = 0; i < chances.Length; i++)
            {
                float rnd = UnityEngine.Random.Range(0, maxChance);
                if (rnd <= chances[i])
                {
                    finalIndex = i;
                    return finalIndex;
                }
                else
                {
                    maxChance -= chances[i];
                }
            }
            return -1;
        }
        /// <summary>
        /// Used to optimize work with different resolutions in <see cref="Canvas"/>
        /// </summary>
        /// <returns>0.5..1f</returns>
        public static float GetOptimalScreenScale()
        {
            int width = Screen.currentResolution.width;
            int height = Screen.currentResolution.height;
            float scale = (width / (float)height) switch
            {
                float i when i < 1.01f => 0.5f,
                float i when i < 1.26f => 0.6f,
                float i when i < 1.46f => 0.7f,
                float i when i < 1.61f => 0.8f,
                float i when i < 1.75f => 0.9f,
                _ => 1f,
            };
            return scale;
        }
        /// <summary>
        /// Calculating rounded value with percent multiplier. e.g: 10 * 50(%) = 5
        /// </summary>
        /// <param name="value"></param>
        /// <param name="multiplier">0..any%</param>
        /// <returns></returns>
        public static int Multiply(int value, float multiplier) => Mathf.RoundToInt(value * multiplier / 100f);
        /// <summary>
        /// Calculating with percent multiplier. e.g: 10 * 50(%) = 5
        /// </summary>
        /// <param name="value"></param>
        /// <param name="multiplier">0..any%</param>
        /// <returns></returns>
        public static float Multiply(float value, float multiplier) => value * multiplier / 100f;
        /// <summary>
        /// E.g. 10 * 5 = 50, returns 40
        /// </summary>
        /// <param name="value"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static int GetMultipliedIncrease(int value, float multiplier) => Multiply(value, multiplier) - value;

        /// <summary>
        /// The same as <see cref="Mathf.Lerp"/> but simplified with vector
        /// </summary>
        /// <param name="length"></param>
        /// <param name="t">0..1f</param>
        /// <returns></returns>
        public static float LerpVector(Vector2 length, float t) => Mathf.Lerp(length.x, length.y, t);
        /// <summary>
        /// The same as <see cref="Mathf.InverseLerp"/> but simplified with vector
        /// </summary>
        /// <param name="length"></param>
        /// <param name="vectorPosition">length.x..length.y</param>
        /// <returns></returns>
        public static float InverseLerpVector(Vector2 length, float vectorPosition) => Mathf.InverseLerp(length.x, length.y, vectorPosition);
        /// <summary>
        /// Transform value from old length to new, e.g. old:Vector(4, 1), old:2, new:Vector(0, 10) = 6.66f,
        /// </summary>
        /// <param name="oldLength"></param>
        /// <param name="oldLengthValue"></param>
        /// <param name="newLength"></param>
        /// <returns></returns>
        public static float ConvertValueFromTo(Vector2 oldLength, float oldLengthValue, Vector2 newLength)
        {
            float lerpedValue = InverseLerpVector(oldLength, oldLengthValue);
            return LerpVector(newLength, lerpedValue);
        }
        public static Vector2 ConvertVectorFromTo(Vector2 oldMinMax, Vector2 scaleable, Vector2 newMinMax)
        {
            return ConvertVectorFromTo(oldMinMax, oldMinMax, scaleable, newMinMax, newMinMax);
        }
        public static Vector2 ConvertVectorFromTo(Vector2 oldMinMaxX, Vector2 oldMinMaxY, Vector2 scaleable, Vector2 newMinMaxX, Vector2 newMinMaxY)
        {
            float x = ConvertValueFromTo(oldMinMaxX, scaleable.x, newMinMaxX);
            float y = ConvertValueFromTo(oldMinMaxY, scaleable.y, newMinMaxY);
            return new(x, y);
        }

        public static bool GetLogicalResult(float value, LogicalOperation @is, float thanValue) => @is switch
        {
            LogicalOperation.Less => value < thanValue,
            LogicalOperation.Greater => value > thanValue,
            _ => throw new System.NotImplementedException($"Logical operation {@is}")
        };

        public static float ConvertToFloat(double value) => System.Convert.ToSingle(value);
        public static float ConvertToFloat(string value) => ConvertToFloat(System.Convert.ToDouble(value.Replace(".", ",")));
        
        public static float Max(this Vector2 vector2) => Mathf.Max(vector2.x, vector2.y);
        public static float Min(this Vector2 vector2) => Mathf.Min(vector2.x, vector2.y);
        public static int Max(this Vector2Int vector2) => Mathf.Max(vector2.x, vector2.y);
        public static int Min(this Vector2Int vector2) => Mathf.Min(vector2.x, vector2.y);
        public static float Max(this Vector3 vector3) => Mathf.Max(vector3.x, vector3.y, vector3.z);
        public static float Min(this Vector3 vector3) => Mathf.Min(vector3.x, vector3.y, vector3.z);

        public static Vector3 Abs(this Vector3 vector3) => new(Mathf.Abs(vector3.x), Mathf.Abs(vector3.y), Mathf.Abs(vector3.z));
        public static Vector2 Abs(this Vector2 vector2) => new(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y));
        public static float ClampPosition(this Vector2 vector2, float position) => Mathf.Clamp(position, vector2.Min(), vector2.Max());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="max"></param>
        /// <returns>Vector with heighest components</returns>
        public static Vector3 MaxClamp(this Vector3 vector3, Vector3 max) => new(Mathf.Max(vector3.x, max.x), Mathf.Max(vector3.y, max.y), Mathf.Max(vector3.z, max.z));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="min"></param>
        /// <returns>Vector with smallest components</returns>
        public static Vector3 MinClamp(this Vector3 vector3, Vector3 min) => new(Mathf.Min(vector3.x, min.x), Mathf.Min(vector3.y, min.y), Mathf.Min(vector3.z, min.z));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>Vector with components clamped in range</returns>
        public static Vector3 Clamp(this Vector3 vector3, Vector3 min, Vector3 max)
        {
            return new(Mathf.Clamp(vector3.x, min.x, max.x), Mathf.Clamp(vector3.y, min.y, max.y), Mathf.Clamp(vector3.z, min.z, max.z));
        }
        public static bool ContainLayer(this LayerMask layerMask, int layer)
        {
            return (layerMask & 1 << layer) != 0;
        }
        public static LayerMask AddLayer(this LayerMask layerMask, int layer)
        {
            return layerMask |= (1 << layer);
        }
        public static LayerMask RemoveLayer(this LayerMask layerMask, int layer)
        {
            return layerMask &= ~(1 << layer);
        }
        #endregion methods
    }
    public enum LogicalOperation { Greater, Less };
}