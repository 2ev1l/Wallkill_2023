using UnityEngine;
using UnityEditor;
using EditorCustom.Attributes;

namespace EditorCustom
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelAttribute labelAttribute = attribute as LabelAttribute;
            label.text = labelAttribute.Label;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}