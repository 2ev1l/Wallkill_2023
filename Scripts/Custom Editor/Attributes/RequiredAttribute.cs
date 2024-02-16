using System;
using UnityEngine;

namespace EditorCustom.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class RequiredAttribute : PropertyAttribute
    {

    }
}