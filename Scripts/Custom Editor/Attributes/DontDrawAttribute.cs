using UnityEngine;
using System;

namespace EditorCustom.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DontDrawAttribute : PropertyAttribute
    {
        public DontDrawAttribute() { }
    }
}