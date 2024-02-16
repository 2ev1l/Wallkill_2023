using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal
{
    [System.Serializable]
    public static class Extensions
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public static bool Exists<TSource>(this IEnumerable<TSource> collection, System.Predicate<TSource> match, out TSource exist)
        {
            exist = default;
            foreach (TSource el in collection)
            {
                if (match.Invoke(el))
                {
                    exist = el;
                    return true;
                }
            }
            return false;
        }

        const float EPSILON = 0.00001f;

        public static bool Approximately(this float a, float b, float epsilon = EPSILON) { return Mathf.Abs(a - b) < epsilon; }
        public static bool Approximately(this Vector2 a, Vector2 b, float epsilon = EPSILON) { return Vector2.SqrMagnitude(a - b) < epsilon; }
        public static bool Approximately(this Vector3 a, Vector3 b, float epsilon = EPSILON) { return Vector3.SqrMagnitude(a - b) < epsilon; }
        public static bool Approximately(this Vector4 a, Vector4 b, float epsilon = EPSILON) { return Vector4.SqrMagnitude(a - b) < epsilon; }
        #endregion methods
    }
}