using EditorCustom.Attributes;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace EditorCustom
{
    [CustomPropertyDrawer(typeof(DontDrawAttribute))]
    public class DontDrawPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
        }
    }
}