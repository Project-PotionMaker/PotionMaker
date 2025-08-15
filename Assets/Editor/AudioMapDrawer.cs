#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AudioMapBase), true)]
public class AudioMapDrawer : PropertyDrawer
{
    private const int BLANK = 2;
    static Type GetEnumArgType(Type t)
    {
        var cur = t;
        while (cur != null && cur != typeof(object))
        {
            if (cur.IsGenericType && cur.GetGenericTypeDefinition() == typeof(AudioMap<>))
                return cur.GetGenericArguments()[0];
            cur = cur.BaseType;
        }
        return null;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var enumType = GetEnumArgType(fieldInfo.FieldType);
        int len = enumType != null ? Enum.GetNames(enumType).Length : 0;
        return (EditorGUIUtility.singleLineHeight + BLANK) * (len + 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var enumType = GetEnumArgType(fieldInfo.FieldType);
        var names = enumType != null ? Enum.GetNames(enumType) : Array.Empty<string>();
        int len = names.Length;

        var clipsProp = property.FindPropertyRelative(AudioMapBase.ClipsFieldName);
        if (clipsProp != null && clipsProp.arraySize != len)
            clipsProp.arraySize = len;

        var r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(r, label);
        r.y += (EditorGUIUtility.singleLineHeight + BLANK);

        for (int i = 0; i < len; i++)
        {
            var elem = clipsProp.GetArrayElementAtIndex(i);

            int prevIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = prevIndent + 1;

            EditorGUI.PropertyField(r, elem, new GUIContent(names[i]));

            EditorGUI.indentLevel = prevIndent;
            r.y += (EditorGUIUtility.singleLineHeight + BLANK);
        }

        EditorGUI.EndProperty();
    }
}
#endif