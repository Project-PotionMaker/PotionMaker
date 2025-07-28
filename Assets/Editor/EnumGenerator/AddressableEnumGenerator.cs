using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public static class GenerateAddressableEnums
{
    private const string OutputFolder = "Assets/02. Scripts/02-01. Common/Enum";
    private const string OutputFileName = "EAddressableKeys.cs";
    private const string EnumName = "EAddressableKeys";

    [MenuItem("Tools/Generate Addressable Enums")]
    public static void Generate()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found.");
            return;
        }

        // 1. 기존 enum 키 리스트 (Sanitize된 상태)
        var existingKeys = LoadExistingEnumKeys();

        // 2. 현재 Addressable에서 등록된 키 리스트를 Sanitize해서 집합으로 만듦
        var currentEntries = GetValidAddressableEntries(settings);
        var currentKeysSanitizedSet = new HashSet<string>(
            currentEntries.Select(e => SanitizeKey(e.address))
        );

        // 3. 기존 키 리스트에 현재 키 중 없는 새 키 추가
        var finalKeys = new List<string>(existingKeys);
        foreach (var key in currentKeysSanitizedSet)
        {
            if (!finalKeys.Contains(key))
            {
                finalKeys.Add(key);
            }
        }

        // 4. enum 코드 생성 (Obsolete 표시 포함)
        var code = BuildEnumFile(finalKeys, currentKeysSanitizedSet);

        // 5. 파일 저장 및 에디터 리프레시
        SaveEnumFile(code);
        Debug.Log($"[AddressableEnum] {EnumName} generated successfully.");
    }

    // 기존 enum 파일에서 이미 Sanitize된 키 목록 추출
    private static List<string> LoadExistingEnumKeys()
    {
        string path = Path.Combine(OutputFolder, OutputFileName);
        var keys = new List<string>();

        if (!File.Exists(path))
        {
            return keys;
        }

        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Obsolete 속성, 주석, 빈 줄 무시
            if (trimmed.StartsWith("[Obsolete"))
            {
                continue;
            }
            if (trimmed.StartsWith("//"))
            {
                continue;
            }
            if (string.IsNullOrEmpty(trimmed))
            {
                continue;
            }

            // "KeyName = 숫자," 형태에서 KeyName만 추출
            if (trimmed.Contains("=") && trimmed.EndsWith(","))
            {
                var key = trimmed.Split('=')[0].Trim();
                keys.Add(key);
            }
        }

        return keys;
    }

    private static List<AddressableAssetEntry> GetValidAddressableEntries(AddressableAssetSettings settings)
    {
        return settings.groups
            .Where(g => g != null)
            .SelectMany(g => g.entries)
            .Where(entry => entry != null && !string.IsNullOrEmpty(entry.address))
            .ToList();
    }

    // 최종 enum 코드 생성 (Obsolete 처리 포함)
    private static string BuildEnumFile(List<string> allKeys, HashSet<string> currentKeysSanitizedSet)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// Auto-generated. Do not modify manually.");
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"public enum {EnumName}");
        sb.AppendLine("{");

        for (int i = 0; i < allKeys.Count; i++)
        {
            string key = allKeys[i];

            // 현재 Addressable에 없으면 Obsolete 붙임
            if (!currentKeysSanitizedSet.Contains(key))
            {
                sb.AppendLine("    [Obsolete(\"Removed from Addressables\")]");
            }

            sb.AppendLine($"    {key} = {i},");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    // 주소값 → 유효한 enum 식별자 이름 변환
    private static string SanitizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return "_INVALID_KEY";
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(key
            .Where(c => !invalidChars.Contains(c) && (char.IsLetterOrDigit(c) || c == '_'))
            .ToArray());

        if (cleaned.Length == 0)
        {
            cleaned = "_EMPTY";
        }

        if (char.IsDigit(cleaned[0]))
        {
            cleaned = "_" + cleaned;
        }

        return cleaned;
    }

    private static void SaveEnumFile(string code)
    {
        if (!Directory.Exists(OutputFolder))
        {
            Directory.CreateDirectory(OutputFolder);
        }

        string path = Path.Combine(OutputFolder, OutputFileName);
        File.WriteAllText(path, code);
        AssetDatabase.Refresh();
    }
}
