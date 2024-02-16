using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Data.Enums
{
    #region enum
    public enum AnimatorPropertyType
    {
        Bool, Int, Float
    }
    #endregion enum

    public static class AnimatorPropertyTypeExtension
    {
        #region methods
        public static void ExposeToAnimator(this AnimatorPropertyType propertyType, Animator animator, string propertyName, string value)
        {
            object newValue = ConvertAnimatorValue(propertyType, value);
            switch (propertyType)
            {
                case AnimatorPropertyType.Bool: animator.SetBool(propertyName, (bool)newValue); break;
                case AnimatorPropertyType.Int: animator.SetInteger(propertyName, (int)newValue); break;
                case AnimatorPropertyType.Float: animator.SetFloat(propertyName, (float)newValue); break;
                default: Debug.LogError($"Undefined animator property type: [{propertyType}]"); break;
            }
        }
        public static object ConvertAnimatorValue(this AnimatorPropertyType propertyType, string value) => propertyType switch
        {
            AnimatorPropertyType.Bool => value != "0",
            AnimatorPropertyType.Int => System.Convert.ToInt32(value),
            AnimatorPropertyType.Float => CustomMath.ConvertToFloat(value),
            _ => throw new System.NotImplementedException($"Undefined animator property type: [{propertyType}]")
        };
        #endregion methods

    }
}