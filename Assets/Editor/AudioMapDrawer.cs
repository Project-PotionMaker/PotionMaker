#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AudioMapBase), true)]
public class AudioMapDrawer : PropertyDrawer
{
    static Type GetEnumArgType(Type t)
    {
        // 현 필드 타입에서 제네릭 인자를 추적 (PlayerAudioMap -> EnumAudioMap<T>)
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
        return EditorGUIUtility.singleLineHeight * (len + 1) + 6;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var enumType = GetEnumArgType(fieldInfo.FieldType);
        var names = enumType != null ? Enum.GetNames(enumType) : Array.Empty<string>();
        int len = names.Length;

        var clipsProp = property.FindPropertyRelative(AudioMap<EPlayerAudioType>.ClipsFieldName);
        if (clipsProp != null && clipsProp.arraySize != len)
            clipsProp.arraySize = len;

        var r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(r, label);
        r.y += EditorGUIUtility.singleLineHeight;

        for (int i = 0; i < len; i++)
        {
            var elem = clipsProp.GetArrayElementAtIndex(i);

            int prevIndent = EditorGUI.indentLevel;   // 안전하게 복구용
            EditorGUI.indentLevel = prevIndent + 1;   // ← 들여쓰기 한 단계

            EditorGUI.PropertyField(r, elem, new GUIContent(names[i]));

            EditorGUI.indentLevel = prevIndent;       // ← 복구
            r.y += EditorGUIUtility.singleLineHeight;
        }

        EditorGUI.EndProperty();
    }
}
#endif