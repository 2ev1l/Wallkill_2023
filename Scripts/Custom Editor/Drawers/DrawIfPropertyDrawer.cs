using EditorCustom.Attributes;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace EditorCustom
{
    [CustomPropertyDrawer(typeof(DrawIfAttribute))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        private DrawIfAttribute _drawIf;
        private SerializedProperty _comparedField;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!ShowMe(property) && _drawIf.DisablingType == DisablingType.DontDraw)
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
        }

        private bool ShowMe(SerializedProperty property)
        {
            _drawIf = attribute as DrawIfAttribute;

            _comparedField = TryToFindSerializableProperty(_drawIf.ComparedPropertyName, property);

            // We check that exist a Field with the parameter name
            if (_comparedField == null)
            {
                Debug.Log($"Error getting the condition Field. Check the name. {_drawIf.ComparedPropertyName} ?");
                return true;
            }

            // get the value & compare based on types
            switch (_comparedField.type)
            { // Possible extend cases to support your own type
                case "bool":
                    return _comparedField.boolValue.Equals(_drawIf.ComparedValue);
                case "Enum":
                    return (_comparedField.intValue) == (int)_drawIf.ComparedValue;
                case "string":
                    return _comparedField.stringValue.Equals(_drawIf.ComparedValue);
                default:
                    Debug.LogError("Error: " + _comparedField.type + " is not supported");
                    return true;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // If the condition is met, simply draw the field.
            if (ShowMe(property))
            {
                EditorGUI.PropertyField(position, property, label, true);

            } //...check if the disabling type is read only. If it is, draw it disabled
            else if (_drawIf.DisablingType == DisablingType.ReadOnly)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
        }

        /// <summary>
        /// Return SerializedProperty by it's name if it exists, it works for nested objects and arrays
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private SerializedProperty TryToFindSerializableProperty(string propertyName, SerializedProperty property)
        {
            var serializedProperty = property.serializedObject.FindProperty(propertyName);

            // Find relative
            if (serializedProperty == null)
            {
                string propertyPath = property.propertyPath;
                int idx = propertyPath.LastIndexOf('.');
                if (idx != -1)
                {
                    propertyPath = propertyPath.Substring(0, idx);
                    return property.serializedObject.FindProperty(propertyPath).FindPropertyRelative(propertyName);
                }
            }

            return serializedProperty;
        }
    }
}