#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.SettingsGenerator
{
    public static class UIToolkitHeaderAttributeFixer
    {
        public static void AddHeaderLabels(VisualElement root, SerializedObject serializedObject)
        {
            var targetType = serializedObject.targetObject.GetType();
            var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var headerAttr = field.GetCustomAttribute<HeaderAttribute>();
                if (headerAttr == null) continue;

                SerializedProperty prop = serializedObject.FindProperty(field.Name);
                if (prop == null)
                    continue;

                var propertyField = root.Query<PropertyField>().ToList().FirstOrDefault(pf => pf.bindingPath == prop.propertyPath);
                if (propertyField == null)
                    continue;

                // Create a fake header label
                var headerLabel = new Label(headerAttr.header);
                headerLabel.name = "Header_" + Regex.Replace(headerAttr.header, "[^A-za-z_-]", "");
                headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                headerLabel.style.marginTop = 10;
                headerLabel.style.marginBottom = 2;

                // Insert the header before the PropertyField
                var parent = propertyField.parent;
                if (parent != null)
                {
                    int index = parent.IndexOf(propertyField);
                    parent.Insert(index, headerLabel); 
                }
            }
        }
    }
}
#endif