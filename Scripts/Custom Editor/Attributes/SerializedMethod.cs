using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EditorCustom.Attributes
{
    /// <summary>
    /// Do nothing but indicates which methods are used outside the IDE. 
    /// For example, in serialized <see cref="UnityEvent"/>.
    /// These methods are highly not recommended to refactoring due to data loss.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SerializedMethod : Attribute
    {

    }
}