using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
#endif

namespace Kamgam.SettingsGenerator
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DisableIfAttribute : PropertyAttribute
    {
        public enum BehaviourType
        {
            Disable = 0,
            Hide = 1
        }

        public string PropertyName { get; private set; }
        public object CompareValue { get; private set; }

        public string PropertyName2 { get; private set; }
        public object CompareValue2 { get; private set; }

        public BehaviourType Behaviour { get; private set; }
        public bool InvertBehaviour { get; private set; }

        /// <summary>
        /// Draws the field/property ONLY if the referenced property is not empty/null/false/0.
        /// </summary>
        /// <param name="propertyName">The name of the property that is being compared (case sensitive).</param>
        /// <param name="comparedValue">Will be disabled if the value equals this. If null then it will be interpreted as "empty".</param>
        /// <param name="behaviour">How it should behave.</param>
        /// <param name="invertBehaviour">Invert the comparison result.</param>
        public DisableIfAttribute(string propertyName, object comparedValue = null, BehaviourType behaviour = BehaviourType.Disable, bool invertBehaviour = false, string propertyName2 = null, object comparedValue2 = null)
        {
#if UNITY_EDITOR
            PropertyName = propertyName;
            CompareValue = comparedValue;
            PropertyName2 = propertyName2;
            CompareValue2 = comparedValue2;
            Behaviour = behaviour;
            InvertBehaviour = invertBehaviour;
#endif
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfAttributeDrawer : PropertyDrawer
    {
        DisableIfAttribute _attribute;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (DisableMe(property) && _attribute.Behaviour == DisableIfAttribute.BehaviourType.Hide)
                return 0f;

            return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
        }

        /// <summary>
        /// Errors default to showing the property.
        /// </summary>
        private bool DisableMe(SerializedProperty property)
        {
            _attribute = attribute as DisableIfAttribute;

            // Replace propertyname to the value from the parameter
            var parentProperty = FindParentProperty(property);
            if (parentProperty == null)
                return false;

            var comparedProperty = parentProperty.FindPropertyRelative(_attribute.PropertyName);
            if (comparedProperty == null)
                return false;

            // Skip if property can not be found.
            if (comparedProperty == null)
                return false;

            var comparedProperty2 = parentProperty.FindPropertyRelative(_attribute.PropertyName2);

            bool result;

            if (comparedProperty2 != null) 
            {
                var resultA = checkProperty(property, comparedProperty, _attribute.CompareValue);
                var resultB = checkProperty(property, comparedProperty2, _attribute.CompareValue2);
                if (_attribute.InvertBehaviour)
                {
                    resultA = !resultA;
                    resultB = !resultB;
                }
                result = resultA && resultB; 
            }
            else
            {
                result = checkProperty(property, comparedProperty, _attribute.CompareValue);
                if (_attribute.InvertBehaviour)
                    result = !result;
            }

            return result;
        }

        private bool checkProperty(SerializedProperty property, SerializedProperty comparedProperty, object compareValue)
        {
            bool result;
            switch (comparedProperty.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    result = comparedProperty.objectReferenceValue == (object)compareValue;
                    break;

                case SerializedPropertyType.Boolean:
                    result = comparedProperty.boolValue == (bool)compareValue;
                    break;

                case SerializedPropertyType.String:
                    if (compareValue == null && string.IsNullOrEmpty(comparedProperty.stringValue))
                        result = true;
                    else
                        result = comparedProperty.stringValue == (string)compareValue;
                    break;

                case SerializedPropertyType.Integer:
                    if (compareValue == null && comparedProperty.intValue == 0)
                        result = true;
                    else
                        result = comparedProperty.intValue == (int)compareValue;
                    break;

                case SerializedPropertyType.Float:
                    if (compareValue == null && property.floatValue == 0f)
                        result = true;
                    else
                        result = comparedProperty.floatValue == (float)compareValue;
                    break;

                case SerializedPropertyType.Enum:
                    result = comparedProperty.enumValueIndex == (int)compareValue;
                    break;

                default:
                    Debug.LogError("Error: Property '" + _attribute.PropertyName + "' is of unsupported type " + comparedProperty.propertyType.ToString() + ".");
                    result = false;
                    break;
            }

            return result;
        }

        public static SerializedProperty FindParentProperty(SerializedProperty serializedProperty)
        {
            // Thanks to: https://gist.github.com/monry/9de7009689cbc5050c652bcaaaa11daa
            // n2h: Investigate: does return "data" if serializedProperty is an array.

            var propertyPaths = serializedProperty.propertyPath.Split('.');
            if (propertyPaths.Length <= 1)
            {
                return default;
            }

            var parentSerializedProperty = serializedProperty.serializedObject.FindProperty(propertyPaths.First());
            for (var index = 1; index < propertyPaths.Length - 1; index++)
            {
                if (propertyPaths[index] == "Array")
                {
                    if (index + 1 == propertyPaths.Length - 1)
                    {
                        // reached the end
                        break;
                    }
                    if (propertyPaths.Length > index + 1 && Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
                    {
                        var match = Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                        var arrayIndex = int.Parse(match.Groups[1].Value);
                        parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                        index++;
                    }
                }
                else
                {
                    parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
                }
            }

            return parentSerializedProperty;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (DisableMe(property))
            {
                if (_attribute.Behaviour == DisableIfAttribute.BehaviourType.Disable)
                {
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property, label);
                    GUI.enabled = true;
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, includeChildren: true);
            }
        }
    }
#endif
}
