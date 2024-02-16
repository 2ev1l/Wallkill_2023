using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorCustom.Attributes
{
    /// <summary>
    /// Hides property input field and label but other UI attributes will be active
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class HideFieldAttribute : PropertyAttribute
    {
        public HideFieldAttribute()
        {
            
        }
    }
}